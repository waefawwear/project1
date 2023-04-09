using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSpliterUI : MonoBehaviour
{
    uint itemSplitCount = 1;

    ItemSlotUI targetSlotUI;

    TMP_InputField inputField;

    /// <summary>
    /// OK��ư�� ������ �� ���� �� ��������Ʈ
    /// </summary>
    public Action<uint, uint> OnOkClick;

    /// <summary>
    /// ������ ���� ������ ������Ƽ
    /// </summary>
    uint ItemSplitCount
    {
        get => itemSplitCount;
        set
        {
            // ���� �Էµ� �� �ּҰ��� 1, �ִ밪�� (��󽽷��� ������ �ִ� ������ ���� - 1)�� �����ϴ� �ڵ�
            itemSplitCount = value;
            itemSplitCount = (uint)Mathf.Max(1, itemSplitCount);    // 1�� �ּҰ�
            if(targetSlotUI != null)
            {
                itemSplitCount = (uint)Mathf.Min(itemSplitCount, targetSlotUI.ItemSlot.ItemCount - 1);
            }
            inputField.text = itemSplitCount.ToString();
        }
    }

    public void Initialize()
    {
        inputField = GetComponentInChildren<TMP_InputField>();
        inputField.onValueChanged.AddListener(OnInputChange);
        inputField.text = itemSplitCount.ToString();

        Button increase = transform.Find("IncreaseButton").GetComponent<Button>();
        increase.onClick.AddListener(OnIncrease);
        Button decrease = transform.Find("DecreaseButton").GetComponent<Button>();
        decrease.onClick.AddListener(OnDecrease);
        Button ok = transform.Find("OK_Button").GetComponent<Button>();
        ok.onClick.AddListener(OnOK);
        Button cancel = transform.Find("Cancel_Button").GetComponent<Button>();
        cancel.onClick.AddListener(OnCancelClick);

        Close();
    }

    /// <summary>
    /// ������ ����â ����
    /// </summary>
    /// <param name="target">�������� ������ ��� ����</param>
    public void Open(ItemSlotUI target)
    {
        if(target.ItemSlot.ItemCount > 1)
        {
            targetSlotUI = target;
            ItemSplitCount = 1;
            transform.position = target.transform.position;
            gameObject.SetActive(true);
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void OnIncrease()
    {
        ItemSplitCount++;
    }

    private void OnDecrease()
    {
        ItemSplitCount--;
    }

    private void OnOK()
    {
        //targetSlotUI.ItemSlot.DecreaseSlotItem(ItemSplitCount);
        //ItemSlot tempSlot = new(targetSlotUI.ItemSlot.SlotItemData, ItemSplitCount);
        //tempSlot.onSlotItemChange = GameManager.Inst.InvenUI.TempSlotUI.Refresh;
        //GameManager.Inst.InvenUI.TempSlotUI.Open(tempSlot);

        OnOkClick?.Invoke(targetSlotUI.ID, ItemSplitCount);     // ��������Ʈ�� ����� �Լ� ����

        Close();    // �ݱ�
    }

    /// <summary>
    /// Cancel�������� ���� �� �Լ�
    /// </summary>
    private void OnCancelClick()
    {
        targetSlotUI = null;    // ������ �ʱ�ȭ�ϰ�
        Close();                // �ݱ�
    }

    /// <summary>
    /// InputField���� ���� ����� �� ����� �Լ�
    /// </summary>
    /// <param name="input">����� ��</param>
    private void OnInputChange(string input)
    {
        if( input.Length == 0)
        {
            ItemSplitCount = 0;
        }
        else
        {
            ItemSplitCount = uint.Parse(input);
        }
    }
}
