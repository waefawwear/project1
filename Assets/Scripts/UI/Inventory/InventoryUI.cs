using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // 기본 데이터 -----------------------------------------------------------------------------------------------------------------
    Inventory inven;
    Player player;
    public GameObject slotPrefab;
    Transform slotParent;
    ItemSlotUI[] slotUIs;
    CanvasGroup canvasGroup;
    PlayerInputActions inputActions;

    public CanvasGroup Canvas => canvasGroup;
    // Item관련 -------------------------------------------------------------------------------------------------------------------
    const uint InvalidID = uint.MaxValue;
    uint dragStartID = InvalidID;
   

    TempItemSlotUI tempItemSlotUI;

    public TempItemSlotUI TempSlotUI => tempItemSlotUI;
    // 상세 정보 UI-----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// 아이템 상세정보 창
    /// </summary>
    DetailInfoUI detail;
    public DetailInfoUI Detail => detail;

    // 아이템 분할 UI---------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// 아이템 분할 창
    /// </summary>
    ItemSpliterUI itemSpliterUI;
    public ItemSpliterUI SpliterUI => itemSpliterUI;

    // 돈 UI-----------------------------------------------------------------------------------------------------------------------
    TextMeshProUGUI goldText;

    // ----------------------------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        goldText = transform.Find("Gold").Find("GoldText").GetComponent<TextMeshProUGUI>();
        slotParent = transform.Find("ItemSlots");
        tempItemSlotUI = GetComponentInChildren<TempItemSlotUI>();
        detail = transform.Find("Detail").GetComponent<DetailInfoUI>();
        itemSpliterUI = GetComponentInChildren<ItemSpliterUI>();

        Button closeButton = transform.Find("CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(Close);

        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        player = GameManager.Inst.MainPlayer;
        player.OnMoneyChange += ReFreshMoney;
        ReFreshMoney(player.Money);

        Close();
    }

    /// <summary>
    /// 인벤토리 초기화
    /// </summary>
    /// <param name="newInven">초기화시킬 인벤토리</param>
    public void InitializeInventory(Inventory newInven)
    {
        inven = newInven;
        if (Inventory.Default_Inventory_Size != newInven.SlotCount)      // 기본 사이즈와 다르면 기본 슬롯 삭제
        {
            // 기존 슬롯 전부 삭제
            ItemSlotUI[] slots = GetComponentsInChildren<ItemSlotUI>();
            foreach (var slot in slots)
            {
                Destroy(slot.gameObject);
            }
            // 새로 만들기
            slotUIs = new ItemSlotUI[inven.SlotCount];
            for (int i = 0; i < inven.SlotCount; i++)
            {
                GameObject obj = Instantiate(slotPrefab, slotParent);
                obj.name = $"{slotPrefab.name}_{i}";
                slotUIs[i] = obj.GetComponent<ItemSlotUI>();
                slotUIs[i].Initialize((uint)i, inven[i]);
            }
        }
        else
        {
            slotUIs = slotParent.GetComponentsInChildren<ItemSlotUI>();
            for (int i = 0; i < inven.SlotCount; i++)
            {
                slotUIs[i].Initialize((uint)i, inven[i]);
            }
        }

        // TempSlot 초기화
        tempItemSlotUI.Initialize(Inventory.TempSlotID, inven.TempSlot);    // TempItemSlotUI와 TempSlot 연결
        tempItemSlotUI.Close(); // 닫은체로 시작하기
        inputActions.UI.ItemDrop.canceled += tempItemSlotUI.OnDrop;

        // ItemSpliterUI 초기화(순서는 상관 없음)
        itemSpliterUI.Initialize();
        itemSpliterUI.OnOkClick += OnSpliterOk;     // itemSpliterUI의 OK버튼이 눌러졌을 때 실행할 함수 등록

        RefreshAllSlots();      // 전체 슬롯UI 갱신
    }

    private void RefreshAllSlots()
    {
        foreach(var slotUI in slotUIs)
        {
            slotUI.Refresh();
        }
    }

    private void ReFreshMoney(int money)
    {
        goldText.text = money.ToString("N0");
    }

    public void InventoyOnOffSwitch()
    {
        if (canvasGroup.blocksRaycasts)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    void Open()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        GameManager.Inst.OnUiMode = true;
    }

    void Close()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        GameManager.Inst.OnUiMode = false;
    }

    public void ClearAllEquipMark()
    {
        foreach(var slot in slotUIs)
        {
            slot.ClearEquipMark();
        }
    }

    // 델리게이트용 함수 -------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// SpliterUI가 OK됐을 때 실행 될 함수
    /// </summary>
    /// <param name="slotID">나누려는 슬롯의 ID</param>
    /// <param name="count">나눈 갯수</param>
    private void OnSpliterOk(uint slotID, uint count)
    {
        inven.TempRemoveItem(slotID, count);        // slotID에서 count만큼 덜어내서 TempSlot에 옮기기
        tempItemSlotUI.Open();      // tempItemSlotUI 열어서 보여주기
    }

    public void OnDrag(PointerEventData eventData)
    {
        //if (eventData.button == PointerEventData.InputButton.Left)
        //{
        //    Detail.isPause = true;
        //}
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if ( TempSlotUI.IsEmpty() && !SpliterUI.isActiveAndEnabled )
            {
                GameObject startobj = eventData.pointerCurrentRaycast.gameObject;
                if (startobj != null)
                {
                    ItemSlotUI slotUI = startobj.GetComponent<ItemSlotUI>();
                    if (slotUI != null)
                    {
                        dragStartID = slotUI.ID;
                        inven.TempRemoveItem(dragStartID, slotUI.ItemSlot.ItemCount, slotUI.ItemSlot.ItemEquiped);       // 드래그 시작한 위치의 아이템을 TempSlot으로 옮김
                        tempItemSlotUI.Open();  // 드래그 시작할 때 TempSlot 열기
                        detail.Close();         // 상세정보창 닫기
                        detail.IsPause = true;  // 상세정보창 안열리게 하기
                    }
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (dragStartID != InvalidID)
            {
                GameObject endobj = eventData.pointerCurrentRaycast.gameObject;
                if (endobj != null)
                {
                    ItemSlotUI slotUI = endobj.GetComponent<ItemSlotUI>();
                    if (slotUI != null)
                    {

                        inven.MoveItem(Inventory.TempSlotID, slotUI.ID);    // TempSlot의 아이템을 드래그가 끝난 슬롯에 옮기기
                        inven.MoveItem(Inventory.TempSlotID, dragStartID);  // 드래그가 끝난 슬롯에 있던 아이템을 dragStartID 슬롯으로 옮기기

                        detail.IsPause = false; // 디테일창 다시 열리게 하기
                        detail.Open(slotUI.ItemSlot.SlotItemData);
                        dragStartID = InvalidID;
                    }
                }
                if (tempItemSlotUI.IsEmpty())
                {
                    tempItemSlotUI.Close();     // 정상적으로 끝나면 닫기
                    detail.IsPause = false;
                }
            }
        }
    }
}
