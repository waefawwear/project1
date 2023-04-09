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
    /// ������ ������ ǥ���� Text ������Ʈ
    /// </summary>
    protected TextMeshProUGUI countText;

    /// <summary>
    /// �������� ��� ���θ� Ȯ���� Text ������Ʈ
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
