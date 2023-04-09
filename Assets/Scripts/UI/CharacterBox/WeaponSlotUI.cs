using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WeaponSlotUI : MonoBehaviour
{
    protected Image itemImage;

    [SerializeField]
    private Sprite weaponImage;

    /// <summary>
    /// 아이템 갯수를 표시할 Text 컴포넌트
    /// </summary>
    protected TextMeshProUGUI countText;

    /// <summary>
    /// 아이템의 장비 여부를 확인할 Text 컴포넌트
    /// </summary>
    protected TextMeshProUGUI equipMark;

    protected virtual void Start()
    {
        itemImage = transform.GetChild(0).GetComponent<Image>();
        GameManager.Inst.MainPlayer.OnWeaponChange = Refresh;
    }
    

    public void Refresh()   // 무기를 장착하면 이미지 변경
    {
        if (GameManager.Inst.MainPlayer.EquipWeaponSlot != null)
        {
            itemImage.sprite = GameManager.Inst.MainPlayer.EquipWeaponSlot.SlotItemData.itemIcon;
            itemImage.color = Color.white;
        }
        else
        {
            itemImage.sprite = weaponImage;
        }
    }
}
