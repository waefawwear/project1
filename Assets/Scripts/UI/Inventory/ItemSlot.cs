using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot
{
    ItemData slotItemData;

    uint itemCount = 0;

    bool itemEquiped = false;

    public uint ItemCount   // 아이템 수량
    {
        get => itemCount;

        private set
        {
            itemCount = value;
            onSlotItemChange?.Invoke();
        }
    }

    public ItemData SlotItemData    // 아이템 데이터
    {
        get => slotItemData;
        private set
        {
            if(slotItemData != value)
            {
                slotItemData = value;
                onSlotItemChange?.Invoke();
            }
        }
    }

    public bool ItemEquiped     // 아이템 사용여부
    {
        get => itemEquiped;
        set
        {
            itemEquiped = value;
            onSlotItemChange?.Invoke();
        }
    }

    public System.Action onSlotItemChange;

    public ItemSlot () { }

    public ItemSlot (ItemData data, uint count)
    {
        slotItemData = data;
        ItemCount = count;
    }

    public ItemSlot(ItemSlot other)
    {
        slotItemData = other.SlotItemData;
        ItemCount = other.ItemCount;
    }

    /// <summary>
    /// 아이템 배정
    /// </summary>
    /// <param name="itemData">아이템 데이터</param>
    /// <param name="count">아이템 수량</param>
    public void AssignSlotItem(ItemData itemData, uint count = 1)
    {
        ItemCount = count;
        SlotItemData = itemData;
    }

    /// <summary>
    /// 같은 종류의 아이템이 추가되어 아이템 갯수가 증가하는 상황에 사용
    /// </summary>
    /// <param name="count">증가시킬 갯수</param>
    /// <returns>최대치를 넘어선 갯수, 0이면 다 증가시킨 상황</returns>
    public uint IncreaseSlotItem(uint count = 1)
    {
        uint newCount = ItemCount + count;
        int overCount = (int)newCount - (int)SlotItemData.maxStackCount;
        if(overCount > 0)
        {
            ItemCount = SlotItemData.maxStackCount;
        }
        else
        {
            ItemCount = newCount;
            overCount = 0;
        }
        return (uint)overCount;
    }

    /// <summary>
    /// 같은 종류의 아이템 갯수 감소
    /// </summary>
    /// <param name="count">감소시킬 갯수</param>
    public void DecreaseSlotItem(uint count = 1)
    {
        int newCount = (int)ItemCount - (int)count;
        if( newCount < 1)   // 최종적으로 갯수가 0이 되면 비우기
        {
            ClearSlotItem();
        }
        else
        {
            ItemCount = (uint)newCount;
        }
    }

    /// <summary>
    /// 슬롯 비우기
    /// </summary>
    public void ClearSlotItem()
    {
        SlotItemData = null;
        ItemCount = 0;
        ItemEquiped = false;
    }

    /// <summary>
    /// 아이템을 사용하는 함수
    /// </summary>
    /// <param name="target">아이템의 효과를 받을 대상(보통 플레이어)</param>
    public void UseSlotItem(GameObject target = null)
    {
        IUsable usable = SlotItemData as IUsable;   // 이 아이템이 사용가능한 아이템인지 확인
        if (usable != null)
        {
            // 아이템이 사용가능하면 
            usable.Use(target);     // 아이템 사용하고
            DecreaseSlotItem();     // 아이템 갯수 하나 줄이기
        }
    }

    /// <summary>
    /// 아이템을 장비하는 함수
    /// </summary>
    /// <param name="target">아이템을 장비할 대상</param>
    public bool EquipSlotItem(GameObject target = null)
    {
        bool result = false;
        IEquipItem equipItem = SlotItemData as IEquipItem;      // 이 슬롯의 아이템이 장비 가능한 아이템인지 확인
        if( equipItem != null)
        {
            // 이 아이템은 장비 가능 
            IEquipTarget equipTarget = target.GetComponent<IEquipTarget>();     // 아이템을 장비할 대상이 아이템을 장비할 수 있는지 확인

            if( equipTarget != null)
            {
                // 대상은 아이템을 장비할 수 있다.
                if( equipTarget.EquipWeaponSlot != null && equipTarget.EquipShieldSlot != null)   // 무기와 방패를 모두 장착 하고 있다.
                {
                    if (equipItem.isWeapon)     // 장착할 아이템이 무기일 때
                    {
                        if(equipTarget.EquipWeaponSlot != this)   // 장비하고 있는 무기의 슬롯을 클릭하지 않았을 때
                        {
                            equipTarget.UnEquipWeapon();      // 장착 무기를 해제한다.
                            equipTarget.EquipWeapon(this);    // 다른 무기를 장착한다.
                            result = true;
                        }
                        else  // 장비하고 있는 무기의 슬롯을 클릭 했을 때
                        {
                            equipTarget.UnEquipWeapon();    // 장착한 무기를 해제한다.
                        }
                    }
                    else   // 장착할 아이템이 방패일 때
                    {
                        if (equipTarget.EquipShieldSlot != this)   // 장비하고 있는 방패의 슬롯을 클릭하지 않았을 때
                        {
                            equipTarget.UnEquipShield();      // 장착 방패를 해제한다.
                            equipTarget.EquipShield(this);    // 다른 방패를 장착한다.
                            result = true;
                        }
                        else  // 장비하고 있는 무기의 슬롯을 클릭 했을 때
                        {
                            equipTarget.UnEquipShield();    // 장착한 방패를 해제한다.
                        }
                    }
                }
                else if(equipTarget.EquipWeaponSlot != null && equipTarget.EquipShieldSlot == null)   // 무기만 장착 하고 있다.
                {
                    if (equipItem.isWeapon) // 장착할 아이템이 무기일 때
                    {
                        if (equipTarget.EquipWeaponSlot != this)   // 장비하고 있는 무기의 슬롯을 클릭하지 않았을 때
                        {
                            equipTarget.UnEquipWeapon();      // 장착 무기를 해제한다.
                            equipTarget.EquipWeapon(this);    // 다른 무기를 장착한다.
                            result = true;
                        }
                        else  // 장비하고 있는 무기의 슬롯을 클릭 했을 때
                        {
                            equipTarget.UnEquipWeapon();    // 장착한 무기를 해제한다.
                        }
                    }
                    else  // 장착할 아이템이 방패일 때
                    {
                        equipTarget.EquipShield(this);    // 방패를 장착한다.
                        result = true;
                    }
                }
                else if (equipTarget.EquipWeaponSlot == null && equipTarget.EquipShieldSlot != null)   // 방패만 장착 하고 있다.
                {
                    if (equipItem.isWeapon) // 장착할 아이템이 무기일 때
                    {
                        equipTarget.EquipWeapon(this);    // 무기를 장착한다.
                        result = true;
                    }
                    else  // 장착할 아이템이 방패일 때
                    {
                        if (equipTarget.EquipShieldSlot != this)   // 장비하고 있는 방패의 슬롯을 클릭하지 않았을 때
                        {
                            equipTarget.UnEquipShield();      // 장착 방패를 해제한다.
                            equipTarget.EquipShield(this);    // 다른 방패를 장착한다.
                            result = true;
                        }
                        else  // 장비하고 있는 방패의 슬롯을 클릭 했을 때
                        {
                            equipTarget.UnEquipShield();    // 장착한 방패를 해제한다.
                        }
                    }
                }
                else    // 무기와 방패 둘 다 장착하고 있지 않다.
                {
                    if (equipItem.isWeapon) // 장착할 아이템이 무기일 때
                    {
                        equipTarget.EquipWeapon(this);    // 무기를 장착한다.
                        result = true;
                    }
                    else
                    {
                        equipTarget.EquipShield(this);    // 방패를 장착한다.
                        result = true;
                    }
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 슬롯이 비어있는지 확인
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return SlotItemData == null;
    }
}
