using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IPointerClickHandler
{
    uint id;
    protected ItemSlot itemSlot;  // inventory클래스가 가지고 있는 ItemSlot중 하나

    protected Image itemImage;

    /// <summary>
    /// 아이템 갯수를 표시할 Text 컴포넌트
    /// </summary>
    protected TextMeshProUGUI countText;

    /// <summary>
    /// 아이템의 장비 여부를 확인할 Text 컴포넌트
    /// </summary>
    protected TextMeshProUGUI equipMark;

    InventoryUI invenUI;
    DetailInfoUI detailUI;

    public ItemSlot ItemSlot { get => itemSlot; }

    public uint ID { get => id; }

    protected virtual void Awake()
    {
        itemImage = transform.GetChild(0).GetComponent <Image>();
        countText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        equipMark = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        equipMark.gameObject.SetActive(false);
    }

    public void Initialize(uint newID, ItemSlot targetSlot)
    {
        invenUI = GameManager.Inst.InvenUI;
        detailUI = invenUI.Detail;
        id = newID;
        itemSlot = targetSlot;
        itemSlot.onSlotItemChange = Refresh;
    }

    public void Refresh()
    {
        if (itemSlot.SlotItemData != null)
        {
            itemImage.sprite = itemSlot.SlotItemData.itemIcon;
            itemImage.color = Color.white;
            countText.text = itemSlot.ItemCount.ToString();
            equipMark.gameObject.SetActive((itemSlot.SlotItemData is ItemData_Weapon) && itemSlot.ItemEquiped);
        }
        else
        {
            itemImage.sprite = null;
            itemImage.color = Color.clear;
            countText.text = "";
            equipMark.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(itemSlot.SlotItemData != null)
        {
            detailUI.Open(itemSlot.SlotItemData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        detailUI.Close();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        Vector2 mousePos = eventData.position;

        RectTransform rect = (RectTransform)detailUI.transform;     // 디테일 창의 Rect Transform
        if((mousePos.x + rect.sizeDelta.x) > Screen.width)          // 스크린 밖으로 나가면 위치조정
        {
            mousePos.x -= rect.sizeDelta.x;
        }
        detailUI.transform.position = mousePos;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            TempItemSlotUI temp = invenUI.TempSlotUI;
            IEquipItem equipItem = ItemSlot.SlotItemData as IEquipItem;      

            if (Keyboard.current.leftShiftKey.ReadValue() > 0.0f && temp.IsEmpty()) // 왼쪽 쉬프트키와 같이 클릭하면 temp슬롯이 비어 있을때 아이템을 나눈다.
            {
                invenUI.SpliterUI.Open(this);
                detailUI.IsPause = true;
            }
            else
            {
                if(!temp.IsEmpty())     // Temp 슬롯이 비어있지 않을 때
                {
                    bool isEquipItem = temp.ItemSlot.ItemEquiped;   

                    if (ItemSlot.IsEmpty())     // 아이템 슬롯이 비어있을 때
                    {
                        ItemSlot.AssignSlotItem(temp.ItemSlot.SlotItemData, temp.ItemSlot.ItemCount);   // 슬롯에 temp슬롯에 있던 아이템 정보를 넣는다.
                        (temp.ItemSlot.ItemEquiped, itemSlot.ItemEquiped) = (itemSlot.ItemEquiped, temp.ItemSlot.ItemEquiped);
                        temp.Close();
                    }
                    else if (temp.ItemSlot.SlotItemData == ItemSlot.SlotItemData)       // temp슬롯의 아이템 정보가 슬롯의 아이템과 같을 때
                    {
                        uint remains = ItemSlot.SlotItemData.maxStackCount - ItemSlot.ItemCount;

                        uint small = (uint)Mathf.Min((int)remains, (int)temp.ItemSlot.ItemCount);

                        ItemSlot.IncreaseSlotItem(small);
                        temp.ItemSlot.DecreaseSlotItem(small);
                        (temp.ItemSlot.ItemEquiped, itemSlot.ItemEquiped) = (itemSlot.ItemEquiped, temp.ItemSlot.ItemEquiped);  // temp 슬롯에 있던 아이템을 옮기면서 아이템 수량을 변화시킨다.

                        if (temp.ItemSlot.ItemCount < 1)
                        {
                            temp.Close();
                        }
                    }
                    else    // temp슬롯의 아이템 정보가 슬롯의 아이템과 다를 때
                    {
                        ItemData tempData = temp.ItemSlot.SlotItemData;
                        uint tempCount = temp.ItemSlot.ItemCount;
                        temp.ItemSlot.AssignSlotItem(itemSlot.SlotItemData, itemSlot.ItemCount);
                        itemSlot.AssignSlotItem(tempData, tempCount);
                        (temp.ItemSlot.ItemEquiped, itemSlot.ItemEquiped) = (itemSlot.ItemEquiped, temp.ItemSlot.ItemEquiped);  // temp 슬롯과 클릭한 슬롯의 정보를 바꾼다.
                    }         
                    
                    if (isEquipItem)    // 장비중인 아이템을 옮기는 상황이면 일단 해제하고 장비
                    {
                        if (equipItem.isWeapon) // 아이템이 무기일 때
                        {
                            GameManager.Inst.MainPlayer.UnEquipWeapon();    // 무기 장착 해제
                            GameManager.Inst.MainPlayer.EquipWeapon(ItemSlot);
                        }
                        else
                        {
                            GameManager.Inst.MainPlayer.UnEquipShield();    // 방패 장착 해제
                            GameManager.Inst.MainPlayer.EquipShield(ItemSlot);
                        }

                    }

                    detailUI.IsPause = false;       // 디테일창 열기
                }
                else
                {
                    // 그냥 클릭한 상황
                    if ( !itemSlot.IsEmpty() )  // 슬롯이 차 있을 때
                    {
                        // 아이템 사용 시도
                        itemSlot.UseSlotItem(GameManager.Inst.MainPlayer.gameObject);
                        if (itemSlot.IsEmpty()) // 슬롯이 비어있을 떄
                        {
                            detailUI.Close();
                        }
                        // 아이템 장비 시도
                        bool isEquiped = itemSlot.EquipSlotItem(GameManager.Inst.MainPlayer.gameObject);

                        ItemSlot.ItemEquiped = isEquiped;
                    }
                }
            }
        }
    }
    public void ClearEquipMark()    // 장착마크 없애기
    {
        equipMark.gameObject.SetActive(false);
    }
}
