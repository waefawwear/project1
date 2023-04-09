using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    /// <summary>
    /// �κ��丮�� ������ �� ������ ĭ
    /// </summary>
    ItemSlot[] slots = null;

    /// <summary>
    /// �������� �ű�ų� ��� �� ����� �ӽ� ����
    /// </summary>
    ItemSlot tempSlot = null;

    /// <summary>
    /// �κ��丮 �⺻ ũ��
    /// </summary>
    public const int Default_Inventory_Size = 12;

    /// <summary>
    /// TempSlot�� ID
    /// </summary>
    public const uint TempSlotID = 99999;   // ���ڴ� �ǹ̰� ����. Slot Index�� �������� ���� ���̸� �ȴ�.

    // ������Ƽ ------------------------------------------------------------------------------------------------------

    /// <summary>
    /// �κ��丮�� ũ��
    /// </summary>
    public int SlotCount => slots.Length;

    /// <summary>
    /// �ӽ� ����(�б� ����)
    /// </summary>
    public ItemSlot TempSlot => tempSlot;

    /// <summary>
    /// �ε���. �κ��丮���� ���� ��������
    /// </summary>
    /// <param name="index">������ ������ ��</param>
    /// <returns>index��°�� ������ ����</returns>
    public ItemSlot this[int index]  => slots[index];

    /// <summary>
    /// �κ��丮 ������
    /// </summary>
    /// <param name="size">�κ��丮�� ���� ��</param>
    public Inventory(int size = Default_Inventory_Size)
    {
        slots = new ItemSlot[size];
        for(int i=0; i<size; i++)
        {
            slots[i] = new ItemSlot();
        }
        tempSlot = new ItemSlot();     // �ӽ� �뵵�� ����ϴ� ������ ���� ����
    }

    // AddItem�� �Լ� �����ε�(overloading)�� ���� �̸��� ������ �پ��� ������ �Ķ���͸� �Է� ���� �� �ְ� �ߴ�.

    /// <summary>
    /// ������ �߰��ϱ� (������ ��ĭ�� �ֱ�)
    /// </summary>
    /// <param name="id">�߰��� ������ id</param>
    /// <returns></returns>
    public bool AddItem(uint id)
    {
        return AddItem(GameManager.Inst.ItemData[id]);
    }

    public bool AddItem(ItemIDCode code)
    {
        return AddItem(GameManager.Inst.ItemData[code]);
    }

    public bool AddItem(ItemData data)
    {
        bool result = false;

        //Debug.Log($"�κ��丮�� {data.itemName}�� �߰��մϴ�.");
        ItemSlot target = FindSameItem(data);       // ���� ������ ������ ã��
        if(target != null)
        {
            target.IncreaseSlotItem();
            result = true;
            //Debug.Log($"{data.itemName}�� �ϳ� ������ŵ�ϴ�.");
        }
        else
        {
            ItemSlot empty = FindEmptySlot();        //������ �󽽷� ã��

            if (empty != null)
            {
                empty.AssignSlotItem(data);
                result = true;
                //Debug.Log($"������ ���Կ� {data.itemName}�� �Ҵ��մϴ�.");
            }
            else
            {
                //Debug.Log("���� : �κ��丮�� ������ �����߽��ϴ�.");
            }
        }
        return result;
    }

    public bool AddItem(uint id, uint index)
    {
        return AddItem(GameManager.Inst.ItemData[id], index);
    }

    public bool AddItem(ItemIDCode code, uint index)
    {
        return AddItem(GameManager.Inst.ItemData[code], index);
    }

    /// <summary>
    /// ������ �߰��ϱ� (Ư���� ���Կ� �ֱ�)
    /// </summary>
    /// <param name="data">�߰��� ������ ������</param>
    /// <param name="index">�������� �߰��� �κ��丮 ���� �ε���</param>
    /// <returns>�������� �߰��ϴµ� �����ϸ� true, �ƴϸ� false</returns>
    public bool AddItem(ItemData data, uint index)
    {
        bool result = false;

        Debug.Log($"�κ��丮�� {index}���Կ� {data.itemName}�� �߰��մϴ�.");
        ItemSlot slot = slots[index];       // index��°�� ���� ��������
        if (slot.IsEmpty())                 // ã�� ������ ����ִ��� Ȯ��
        {
            slot.AssignSlotItem(data);      // ��������� ������ �߰�
            result = true;
            Debug.Log("�߰��� �����߽��ϴ�.");
        }
        else
        {
            if(slot.SlotItemData == data)
            {
                if(slot.IncreaseSlotItem() == 0)
                {
                    result = true;
                    Debug.Log("������ ���� ������ �����߽��ϴ�.");
                }
                else
                {
                    Debug.Log("���� : ������ ���� á���ϴ�.");
                }
            }
            else
            {
                Debug.Log($"���� : {index} ���Կ��� �ٸ� �������� ����ֽ��ϴ�.");
            }
        }

        return result;
    }

    /// <summary>
    /// Ư�� ������ �������� ������ �Լ�
    /// </summary>
    /// <param name="slotIndex">�������� ���� ������ �ε���</param>
    /// <param name="decreaseCount">������ ������ ����</param>
    /// <returns>�����µ� �����ϸ� true, �ƴϸ� false</returns>
    public bool RemoveItem(uint slotIndex, uint decreaseCount = 1)
    {
        bool result = false;

        //Debug.Log($"�κ��丮�� {slotIndex} ������ �������� {decreaseCount}�� ���ϴ�.");

        if (IsValidSlotIndex(slotIndex))            // slotIndex�� ������ �������� Ȯ��
        {
            ItemSlot slot = slots[slotIndex];
            slot.DecreaseSlotItem(decreaseCount);
            //Debug.Log($"������ �����߽��ϴ�.");
            result = true;
        }
        else
        {
            //Debug.Log("���� : �߸��� �ε��� �Դϴ�.");
        }
        return result;
    }

    /// <summary>
    /// Ư�� ������ ��� �������� ������ �Լ�
    /// </summary>
    /// <param name="slotIndex">�������� ���� ������ �ε���</param>
    /// <returns>�����µ� �����ϸ� true, �ƴϸ� false</returns>
    public bool ClearItem(uint slotIndex)
    {
        bool result = false;

        //Debug.Log($"�κ��丮�� {slotIndex} ������ ���ϴ�.");

        if (IsValidSlotIndex(slotIndex))            // slotIndex�� ������ �������� Ȯ��
        {
            ItemSlot slot = slots[slotIndex];
            //Debug.Log($"{slot.SlotItemData.itemName}�� �����մϴ�.");
            slot.ClearSlotItem();                   // ������ �����̸� ���� ó��
            //Debug.Log($"������ �����߽��ϴ�.");
            result = true;
        }
        else
        {
            //Debug.Log("���� : �߸��� �ε��� �Դϴ�.");
        }
        return result;
    }

    /// <summary>
    /// ��� ������ ������ ���� �Լ�
    /// </summary>
    public void ClearInventory()
    {
        foreach(var slot in slots)
        {
            slot.ClearSlotItem();       // ��ü ���Ե��� ���鼭 �ϳ��� ����
        }
    }

    /// <summary>
    /// ������ �̵���Ű��
    /// </summary>
    /// <param name="from">���� ������ ID</param>
    /// <param name="to">���� ������ ID</param>
    public void MoveItem(uint from, uint to)
    {
        // from�� �븮���� �ε�����, ������ ������� �ʴ�. �׸��� to�� �븮���� ���� �ε�����.
        if(IsValidAndNotEmptySlot(from) && IsValidSlotIndex(to))
        {
            ItemSlot fromSlot = null;
            ItemSlot toSlot = null;

            // �ε����� ����ã��
            if( from == TempSlotID)
            {
                fromSlot = TempSlot;    // temp������ ������ �ε��� Ȯ��
            }
            else
            {
                fromSlot = slots[from]; // �ٸ� ������ �ε����� �״�� Ȱ��
            }

            if (to == TempSlotID)
            {
                toSlot = TempSlot;      // temp������ ������ �ε��� Ȯ��
            }
            else
            {
                toSlot = slots[to];     // �ٸ� ������ �ε����� �״�� Ȱ��
            }

            // �� ���Կ� ����ִ� ������ Ȯ��
            if (fromSlot.SlotItemData == toSlot.SlotItemData)
            {
                // ���� ������ �������̴�. => to�� �ִ��� ä��� ��ġ�� temp�� �״�� �����.
                uint overCount = toSlot.IncreaseSlotItem(fromSlot.ItemCount);
                fromSlot.DecreaseSlotItem(fromSlot.ItemCount - overCount);
            }
            else
            {
                // �ٸ� ������ �������̴�. => �����۰� ������ ������ ���� �����Ѵ�.
                ItemData tempItemData = toSlot.SlotItemData;    // �ӽ� ����
                uint tempItemCount = toSlot.ItemCount;
                toSlot.AssignSlotItem(fromSlot.SlotItemData, fromSlot.ItemCount);   // to���� from�� ���� �ֱ�
                fromSlot.AssignSlotItem(tempItemData, tempItemCount);               // from���ٰ� �ӽ÷� ������ to�� ���� �ֱ�
            }
            (toSlot.ItemEquiped, fromSlot.ItemEquiped) = (fromSlot.ItemEquiped, toSlot.ItemEquiped);
        }
    }

    /// <summary>
    /// �������� ������ �ӽ� ���Կ� ����
    /// </summary>
    /// <param name="from">�������� ���� ����</param>
    /// <param name="count">���� ������ ����</param>
    public void TempRemoveItem(uint from, uint count = 1, bool equiped = false)
    {
        if( IsValidAndNotEmptySlot(from))       // from�� ������ �����̸�
        {
            ItemSlot slot = slots[from];
            tempSlot.AssignSlotItem(slot.SlotItemData, count);      // temp ���Կ� ����� ������ ������ �Ҵ�
            slot.DecreaseSlotItem(count);       // from ���Կ��� �ش� ������ŭ ����
            tempSlot.ItemEquiped = equiped;
        }
    }

    /// <summary>
    /// �� ������ ã���ִ� �Լ�
    /// </summary>
    /// <returns>�� ����</returns>
    ItemSlot FindEmptySlot()
    {
        ItemSlot result = null;

        foreach(var slot in slots)
        {
            if (slot.IsEmpty())
            {
                result = slot;
                break;
            }
        }

        return result;
    }

    private ItemSlot FindSameItem(ItemData itemdata)
    {
        ItemSlot slot = null;
        for(int i=0; i<SlotCount; i++)
        {
            if(slots[i].SlotItemData == itemdata && slots[i].ItemCount < slots[i].SlotItemData.maxStackCount)
            {
                slot = slots[i];
                break;
            }
        }
        return slot;
    }

    /// <summary>
    /// index���� ������ �������� Ȯ�����ִ� �Լ�
    /// </summary>
    /// <param name="index">Ȯ���� �ε���</param>
    /// <returns>true�� ������ ���� , �ƴϸ� false</returns>
    private bool IsValidSlotIndex(uint index) => (index < SlotCount) || (index == TempSlotID);
    //{
    //    return index < SlotCount;
    //}

    private bool IsValidAndNotEmptySlot(uint index)
    {
        ItemSlot testSlot = null;

        if( index != TempSlotID )
        {
            testSlot = slots[index];    // index�� tempSlot�� �ƴϸ� �ε����� ã��
        }
        else
        {
            testSlot = TempSlot;        // index�� tempSlot�� ��� TempSlot ����
        }

        return (IsValidSlotIndex(index) && !testSlot.IsEmpty());
    }

    /// <summary>
    /// �κ��丮 ������ ������ִ� �Լ�
    /// </summary>
    public void PrintInventory()
    {
        string printText = "[";
        for (int i = 0; i < SlotCount - 1; i++)         // ������ ��ü6���� ��� 0~4������ �ϴ� �߰�(5���߰�)
        {
            if (slots[i].SlotItemData != null)
            {
                printText += $"{slots[i].SlotItemData.itemName}({slots[i].ItemCount})";
            }
            else
            {
                printText += "(��ĭ)";
            }
            printText += ",";
        }
        ItemSlot slot = slots[SlotCount - 1];   // ������ ���Ը� ���� ó��
        if (!slot.IsEmpty())
        {
            printText += $"{slot.SlotItemData.itemName}({slot.ItemCount})]";
        }
        else
        {
            printText += "(��ĭ)]";
        }
        Debug.Log(printText);
    }
}
