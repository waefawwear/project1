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
    protected ItemSlot itemSlot;  // inventoryŬ������ ������ �ִ� ItemSlot�� �ϳ�

    protected Image itemImage;

    /// <summary>
    /// ������ ������ ǥ���� Text ������Ʈ
    /// </summary>
    protected TextMeshProUGUI countText;

    /// <summary>
    /// �������� ��� ���θ� Ȯ���� Text ������Ʈ
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

        RectTransform rect = (RectTransform)detailUI.transform;     // ������ â�� Rect Transform
        if((mousePos.x + rect.sizeDelta.x) > Screen.width)          // ��ũ�� ������ ������ ��ġ����
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

            if (Keyboard.current.leftShiftKey.ReadValue() > 0.0f && temp.IsEmpty()) // ���� ����ƮŰ�� ���� Ŭ���ϸ� temp������ ��� ������ �������� ������.
            {
                invenUI.SpliterUI.Open(this);
                detailUI.IsPause = true;
            }
            else
            {
                if(!temp.IsEmpty())     // Temp ������ ������� ���� ��
                {
                    bool isEquipItem = temp.ItemSlot.ItemEquiped;   

                    if (ItemSlot.IsEmpty())     // ������ ������ ������� ��
                    {
                        ItemSlot.AssignSlotItem(temp.ItemSlot.SlotItemData, temp.ItemSlot.ItemCount);   // ���Կ� temp���Կ� �ִ� ������ ������ �ִ´�.
                        (temp.ItemSlot.ItemEquiped, itemSlot.ItemEquiped) = (itemSlot.ItemEquiped, temp.ItemSlot.ItemEquiped);
                        temp.Close();
                    }
                    else if (temp.ItemSlot.SlotItemData == ItemSlot.SlotItemData)       // temp������ ������ ������ ������ �����۰� ���� ��
                    {
                        uint remains = ItemSlot.SlotItemData.maxStackCount - ItemSlot.ItemCount;

                        uint small = (uint)Mathf.Min((int)remains, (int)temp.ItemSlot.ItemCount);

                        ItemSlot.IncreaseSlotItem(small);
                        temp.ItemSlot.DecreaseSlotItem(small);
                        (temp.ItemSlot.ItemEquiped, itemSlot.ItemEquiped) = (itemSlot.ItemEquiped, temp.ItemSlot.ItemEquiped);  // temp ���Կ� �ִ� �������� �ű�鼭 ������ ������ ��ȭ��Ų��.

                        if (temp.ItemSlot.ItemCount < 1)
                        {
                            temp.Close();
                        }
                    }
                    else    // temp������ ������ ������ ������ �����۰� �ٸ� ��
                    {
                        ItemData tempData = temp.ItemSlot.SlotItemData;
                        uint tempCount = temp.ItemSlot.ItemCount;
                        temp.ItemSlot.AssignSlotItem(itemSlot.SlotItemData, itemSlot.ItemCount);
                        itemSlot.AssignSlotItem(tempData, tempCount);
                        (temp.ItemSlot.ItemEquiped, itemSlot.ItemEquiped) = (itemSlot.ItemEquiped, temp.ItemSlot.ItemEquiped);  // temp ���԰� Ŭ���� ������ ������ �ٲ۴�.
                    }         
                    
                    if (isEquipItem)    // ������� �������� �ű�� ��Ȳ�̸� �ϴ� �����ϰ� ���
                    {
                        if (equipItem.isWeapon) // �������� ������ ��
                        {
                            GameManager.Inst.MainPlayer.UnEquipWeapon();    // ���� ���� ����
                            GameManager.Inst.MainPlayer.EquipWeapon(ItemSlot);
                        }
                        else
                        {
                            GameManager.Inst.MainPlayer.UnEquipShield();    // ���� ���� ����
                            GameManager.Inst.MainPlayer.EquipShield(ItemSlot);
                        }

                    }

                    detailUI.IsPause = false;       // ������â ����
                }
                else
                {
                    // �׳� Ŭ���� ��Ȳ
                    if ( !itemSlot.IsEmpty() )  // ������ �� ���� ��
                    {
                        // ������ ��� �õ�
                        itemSlot.UseSlotItem(GameManager.Inst.MainPlayer.gameObject);
                        if (itemSlot.IsEmpty()) // ������ ������� ��
                        {
                            detailUI.Close();
                        }
                        // ������ ��� �õ�
                        bool isEquiped = itemSlot.EquipSlotItem(GameManager.Inst.MainPlayer.gameObject);

                        ItemSlot.ItemEquiped = isEquiped;
                    }
                }
            }
        }
    }
    public void ClearEquipMark()    // ������ũ ���ֱ�
    {
        equipMark.gameObject.SetActive(false);
    }
}
