using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using System;

/// <summary>
/// �ӽ÷� �ѹ��� ���̴� ����
/// </summary>
public class TempItemSlotUI : ItemSlotUI
{
    PointerEventData eventData;
    /// <summary>
    /// Awake�� override�ؼ� �θ��� Awake ����ȵǰ� �����(base.Awake ����)
    /// </summary>
    protected override void Awake()
    {
        itemImage = GetComponent<Image>();      // �̹��� ã�ƿ���
        countText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        equipMark = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        eventData = new PointerEventData(EventSystem.current);
    }

    private void Update()
    {
        transform.position = Mouse.current.position.ReadValue();    // ���콺 ��ġ�� ���缭 �ӽ� ���� �̵�
    }

    /// <summary>
    /// �ӽ� ������ ����
    /// </summary>
    /// <param name="itemSlot">�ӽ� ���Կ� �Ҵ��� �������� ����ִ� ����</param>
    public void Open()
    {
        if (!ItemSlot.IsEmpty())    // ���Կ� �������� ������� ���� ����
        {
            transform.position = Mouse.current.position.ReadValue();    // ���̱� ���� ��ġ ����
            gameObject.SetActive(true);     // ������ ���̰� �����(Ȱ��ȭ��Ű��)
        }
    }

    /// <summary>
    /// �ӽ� ������ ������ �ʰ� �ݱ�
    /// </summary>
    public void Close()
    {
        itemSlot.ClearSlotItem();       // ���Կ� ����ִ� �����۰� ���� ����
        gameObject.SetActive(false);    // ������ ������ �ʰ� �����(��Ȱ��ȭ��Ű��)
    }

    /// <summary>
    /// ������ ������� Ȯ��
    /// </summary>
    /// <returns>true�� ������ ����ִ�</returns>
    public bool IsEmpty() => ItemSlot.IsEmpty();

    public void OnDrop(InputAction.CallbackContext obj)
    {
        if (GameManager.Inst.InvenUI.Canvas.alpha == 1)
        {
            IEquipItem equipItem = itemSlot.SlotItemData as IEquipItem;

            Vector2 mousePos = Mouse.current.position.ReadValue();
            eventData.position = mousePos;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            if (results.Count < 1 && !IsEmpty())
            {
                Ray ray = Camera.main.ScreenPointToRay(mousePos);
                if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ground")))
                {
                    Vector3 pos = GameManager.Inst.MainPlayer.ItemDropPosition(hit.point);
                    ItemFactory.MakeItems(itemSlot.SlotItemData.id, pos, itemSlot.ItemCount);

                    if (itemSlot.ItemEquiped)
                    {
                        if (equipItem.isWeapon)
                        {
                            GameManager.Inst.MainPlayer.UnEquipWeapon();
                        }
                        else
                        {
                            GameManager.Inst.MainPlayer.UnEquipShield();
                        }

                    }

                    Close();
                }
            }
        }
    }
}
