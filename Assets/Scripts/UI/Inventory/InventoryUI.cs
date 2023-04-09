using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // �⺻ ������ -----------------------------------------------------------------------------------------------------------------
    Inventory inven;
    Player player;
    public GameObject slotPrefab;
    Transform slotParent;
    ItemSlotUI[] slotUIs;
    CanvasGroup canvasGroup;
    PlayerInputActions inputActions;

    public CanvasGroup Canvas => canvasGroup;
    // Item���� -------------------------------------------------------------------------------------------------------------------
    const uint InvalidID = uint.MaxValue;
    uint dragStartID = InvalidID;
   

    TempItemSlotUI tempItemSlotUI;

    public TempItemSlotUI TempSlotUI => tempItemSlotUI;
    // �� ���� UI-----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// ������ ������ â
    /// </summary>
    DetailInfoUI detail;
    public DetailInfoUI Detail => detail;

    // ������ ���� UI---------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// ������ ���� â
    /// </summary>
    ItemSpliterUI itemSpliterUI;
    public ItemSpliterUI SpliterUI => itemSpliterUI;

    // �� UI-----------------------------------------------------------------------------------------------------------------------
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
    /// �κ��丮 �ʱ�ȭ
    /// </summary>
    /// <param name="newInven">�ʱ�ȭ��ų �κ��丮</param>
    public void InitializeInventory(Inventory newInven)
    {
        inven = newInven;
        if (Inventory.Default_Inventory_Size != newInven.SlotCount)      // �⺻ ������� �ٸ��� �⺻ ���� ����
        {
            // ���� ���� ���� ����
            ItemSlotUI[] slots = GetComponentsInChildren<ItemSlotUI>();
            foreach (var slot in slots)
            {
                Destroy(slot.gameObject);
            }
            // ���� �����
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

        // TempSlot �ʱ�ȭ
        tempItemSlotUI.Initialize(Inventory.TempSlotID, inven.TempSlot);    // TempItemSlotUI�� TempSlot ����
        tempItemSlotUI.Close(); // ����ü�� �����ϱ�
        inputActions.UI.ItemDrop.canceled += tempItemSlotUI.OnDrop;

        // ItemSpliterUI �ʱ�ȭ(������ ��� ����)
        itemSpliterUI.Initialize();
        itemSpliterUI.OnOkClick += OnSpliterOk;     // itemSpliterUI�� OK��ư�� �������� �� ������ �Լ� ���

        RefreshAllSlots();      // ��ü ����UI ����
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

    // ��������Ʈ�� �Լ� -------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// SpliterUI�� OK���� �� ���� �� �Լ�
    /// </summary>
    /// <param name="slotID">�������� ������ ID</param>
    /// <param name="count">���� ����</param>
    private void OnSpliterOk(uint slotID, uint count)
    {
        inven.TempRemoveItem(slotID, count);        // slotID���� count��ŭ ����� TempSlot�� �ű��
        tempItemSlotUI.Open();      // tempItemSlotUI ��� �����ֱ�
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
                        inven.TempRemoveItem(dragStartID, slotUI.ItemSlot.ItemCount, slotUI.ItemSlot.ItemEquiped);       // �巡�� ������ ��ġ�� �������� TempSlot���� �ű�
                        tempItemSlotUI.Open();  // �巡�� ������ �� TempSlot ����
                        detail.Close();         // ������â �ݱ�
                        detail.IsPause = true;  // ������â �ȿ����� �ϱ�
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

                        inven.MoveItem(Inventory.TempSlotID, slotUI.ID);    // TempSlot�� �������� �巡�װ� ���� ���Կ� �ű��
                        inven.MoveItem(Inventory.TempSlotID, dragStartID);  // �巡�װ� ���� ���Կ� �ִ� �������� dragStartID �������� �ű��

                        detail.IsPause = false; // ������â �ٽ� ������ �ϱ�
                        detail.Open(slotUI.ItemSlot.SlotItemData);
                        dragStartID = InvalidID;
                    }
                }
                if (tempItemSlotUI.IsEmpty())
                {
                    tempItemSlotUI.Close();     // ���������� ������ �ݱ�
                    detail.IsPause = false;
                }
            }
        }
    }
}
