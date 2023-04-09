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
        GameManager.Inst.MainPlayer.OnWeaponChange = Refresh;
    }
    

    public void Refresh()   // ���⸦ �����ϸ� �̹��� ����
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
