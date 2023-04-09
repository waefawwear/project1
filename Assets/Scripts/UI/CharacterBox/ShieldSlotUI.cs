using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShieldSlotUI : MonoBehaviour
{
    protected Image itemImage;

    [SerializeField]
    private Sprite shieldImage;

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
        GameManager.Inst.MainPlayer.OnShieldChange = Refresh;
    }
    

    public void Refresh()
    {
        if (GameManager.Inst.MainPlayer.EquipShieldSlot != null)
        {
            itemImage.sprite = GameManager.Inst.MainPlayer.EquipShieldSlot.SlotItemData.itemIcon;
            itemImage.color = Color.white;
        }
        else
        {
            itemImage.sprite = shieldImage;
        }
    }
}
