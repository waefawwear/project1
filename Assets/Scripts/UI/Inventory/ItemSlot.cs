using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot
{
    ItemData slotItemData;

    uint itemCount = 0;

    bool itemEquiped = false;

    public uint ItemCount   // ������ ����
    {
        get => itemCount;

        private set
        {
            itemCount = value;
            onSlotItemChange?.Invoke();
        }
    }

    public ItemData SlotItemData    // ������ ������
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

    public bool ItemEquiped     // ������ ��뿩��
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
    /// ������ ����
    /// </summary>
    /// <param name="itemData">������ ������</param>
    /// <param name="count">������ ����</param>
    public void AssignSlotItem(ItemData itemData, uint count = 1)
    {
        ItemCount = count;
        SlotItemData = itemData;
    }

    /// <summary>
    /// ���� ������ �������� �߰��Ǿ� ������ ������ �����ϴ� ��Ȳ�� ���
    /// </summary>
    /// <param name="count">������ų ����</param>
    /// <returns>�ִ�ġ�� �Ѿ ����, 0�̸� �� ������Ų ��Ȳ</returns>
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
    /// ���� ������ ������ ���� ����
    /// </summary>
    /// <param name="count">���ҽ�ų ����</param>
    public void DecreaseSlotItem(uint count = 1)
    {
        int newCount = (int)ItemCount - (int)count;
        if( newCount < 1)   // ���������� ������ 0�� �Ǹ� ����
        {
            ClearSlotItem();
        }
        else
        {
            ItemCount = (uint)newCount;
        }
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    public void ClearSlotItem()
    {
        SlotItemData = null;
        ItemCount = 0;
        ItemEquiped = false;
    }

    /// <summary>
    /// �������� ����ϴ� �Լ�
    /// </summary>
    /// <param name="target">�������� ȿ���� ���� ���(���� �÷��̾�)</param>
    public void UseSlotItem(GameObject target = null)
    {
        IUsable usable = SlotItemData as IUsable;   // �� �������� ��밡���� ���������� Ȯ��
        if (usable != null)
        {
            // �������� ��밡���ϸ� 
            usable.Use(target);     // ������ ����ϰ�
            DecreaseSlotItem();     // ������ ���� �ϳ� ���̱�
        }
    }

    /// <summary>
    /// �������� ����ϴ� �Լ�
    /// </summary>
    /// <param name="target">�������� ����� ���</param>
    public bool EquipSlotItem(GameObject target = null)
    {
        bool result = false;
        IEquipItem equipItem = SlotItemData as IEquipItem;      // �� ������ �������� ��� ������ ���������� Ȯ��
        if( equipItem != null)
        {
            // �� �������� ��� ���� 
            IEquipTarget equipTarget = target.GetComponent<IEquipTarget>();     // �������� ����� ����� �������� ����� �� �ִ��� Ȯ��

            if( equipTarget != null)
            {
                // ����� �������� ����� �� �ִ�.
                if( equipTarget.EquipWeaponSlot != null && equipTarget.EquipShieldSlot != null)   // ����� ���и� ��� ���� �ϰ� �ִ�.
                {
                    if (equipItem.isWeapon)     // ������ �������� ������ ��
                    {
                        if(equipTarget.EquipWeaponSlot != this)   // ����ϰ� �ִ� ������ ������ Ŭ������ �ʾ��� ��
                        {
                            equipTarget.UnEquipWeapon();      // ���� ���⸦ �����Ѵ�.
                            equipTarget.EquipWeapon(this);    // �ٸ� ���⸦ �����Ѵ�.
                            result = true;
                        }
                        else  // ����ϰ� �ִ� ������ ������ Ŭ�� ���� ��
                        {
                            equipTarget.UnEquipWeapon();    // ������ ���⸦ �����Ѵ�.
                        }
                    }
                    else   // ������ �������� ������ ��
                    {
                        if (equipTarget.EquipShieldSlot != this)   // ����ϰ� �ִ� ������ ������ Ŭ������ �ʾ��� ��
                        {
                            equipTarget.UnEquipShield();      // ���� ���и� �����Ѵ�.
                            equipTarget.EquipShield(this);    // �ٸ� ���и� �����Ѵ�.
                            result = true;
                        }
                        else  // ����ϰ� �ִ� ������ ������ Ŭ�� ���� ��
                        {
                            equipTarget.UnEquipShield();    // ������ ���и� �����Ѵ�.
                        }
                    }
                }
                else if(equipTarget.EquipWeaponSlot != null && equipTarget.EquipShieldSlot == null)   // ���⸸ ���� �ϰ� �ִ�.
                {
                    if (equipItem.isWeapon) // ������ �������� ������ ��
                    {
                        if (equipTarget.EquipWeaponSlot != this)   // ����ϰ� �ִ� ������ ������ Ŭ������ �ʾ��� ��
                        {
                            equipTarget.UnEquipWeapon();      // ���� ���⸦ �����Ѵ�.
                            equipTarget.EquipWeapon(this);    // �ٸ� ���⸦ �����Ѵ�.
                            result = true;
                        }
                        else  // ����ϰ� �ִ� ������ ������ Ŭ�� ���� ��
                        {
                            equipTarget.UnEquipWeapon();    // ������ ���⸦ �����Ѵ�.
                        }
                    }
                    else  // ������ �������� ������ ��
                    {
                        equipTarget.EquipShield(this);    // ���и� �����Ѵ�.
                        result = true;
                    }
                }
                else if (equipTarget.EquipWeaponSlot == null && equipTarget.EquipShieldSlot != null)   // ���и� ���� �ϰ� �ִ�.
                {
                    if (equipItem.isWeapon) // ������ �������� ������ ��
                    {
                        equipTarget.EquipWeapon(this);    // ���⸦ �����Ѵ�.
                        result = true;
                    }
                    else  // ������ �������� ������ ��
                    {
                        if (equipTarget.EquipShieldSlot != this)   // ����ϰ� �ִ� ������ ������ Ŭ������ �ʾ��� ��
                        {
                            equipTarget.UnEquipShield();      // ���� ���и� �����Ѵ�.
                            equipTarget.EquipShield(this);    // �ٸ� ���и� �����Ѵ�.
                            result = true;
                        }
                        else  // ����ϰ� �ִ� ������ ������ Ŭ�� ���� ��
                        {
                            equipTarget.UnEquipShield();    // ������ ���и� �����Ѵ�.
                        }
                    }
                }
                else    // ����� ���� �� �� �����ϰ� ���� �ʴ�.
                {
                    if (equipItem.isWeapon) // ������ �������� ������ ��
                    {
                        equipTarget.EquipWeapon(this);    // ���⸦ �����Ѵ�.
                        result = true;
                    }
                    else
                    {
                        equipTarget.EquipShield(this);    // ���и� �����Ѵ�.
                        result = true;
                    }
                }
            }
        }
        return result;
    }

    /// <summary>
    /// ������ ����ִ��� Ȯ��
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return SlotItemData == null;
    }
}
