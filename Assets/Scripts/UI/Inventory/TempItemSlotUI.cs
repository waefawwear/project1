using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using System;

/// <summary>
/// 임시로 한번씩 보이는 슬롯
/// </summary>
public class TempItemSlotUI : ItemSlotUI
{
    PointerEventData eventData;
    /// <summary>
    /// Awake를 override해서 부모의 Awake 실행안되게 만들기(base.Awake 제거)
    /// </summary>
    protected override void Awake()
    {
        itemImage = GetComponent<Image>();      // 이미지 찾아오기
        countText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        equipMark = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        eventData = new PointerEventData(EventSystem.current);
    }

    private void Update()
    {
        transform.position = Mouse.current.position.ReadValue();    // 마우스 위치에 맞춰서 임시 슬롯 이동
    }

    /// <summary>
    /// 임시 슬롯을 열기
    /// </summary>
    /// <param name="itemSlot">임시 슬롯에 할당할 아이템이 들어있는 슬롯</param>
    public void Open()
    {
        if (!ItemSlot.IsEmpty())    // 슬롯에 아이템이 들어있을 때만 열기
        {
            transform.position = Mouse.current.position.ReadValue();    // 보이기 전에 위치 조정
            gameObject.SetActive(true);     // 실제로 보이게 만들기(활성화시키기)
        }
    }

    /// <summary>
    /// 임시 슬롯이 보이지 않게 닫기
    /// </summary>
    public void Close()
    {
        itemSlot.ClearSlotItem();       // 슬롯에 들어있는 아이템과 갯수 비우기
        gameObject.SetActive(false);    // 실제로 보이지 않게 만들기(비활성화시키기)
    }

    /// <summary>
    /// 슬롯이 비었는지 확인
    /// </summary>
    /// <returns>true면 슬롯이 비어있다</returns>
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
