using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DetailInfoUI : MonoBehaviour
{
    // ����ϴ� ������Ʈ�� ------------------------------------------------------------------------------------------------
    TextMeshProUGUI itemName;
    TextMeshProUGUI itemPrice;
    Image itemImage;
    CanvasGroup canvasGroup;

    // �⺻ ������ -------------------------------------------------------------------------------------------------------

    /// <summary>
    /// ǥ���� ������
    /// </summary>
    ItemData itemData;

    /// <summary>
    /// ������â ���� �ݴ� ����� �Ͻ� �����ϱ� ���� �÷���(true�� ������ �ʴ´�)
    /// </summary>
    public bool IsPause;

    // �Լ��� ------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// ������â ����
    /// </summary>
    /// <param name="data">ǥ���� ������</param>
    public void Open(ItemData data)
    {
        if (!IsPause)   // pause ���°� �ƴ� ���� ����
        {
            itemData = data;    // ������ �ְ�
            Refresh();          // ȭ�� ����
            canvasGroup.alpha = 1;  // ���İ� ������ on/off ����
        }
    }

    /// <summary>
    /// �� ����â �ݱ�
    /// </summary>
    public void Close()
    {
        if (!IsPause)       // pause���°� �ƴ� ���� �ݱ�
        {
            itemData = null;        // ������ ����
            canvasGroup.alpha = 0;  // ���İ� �����ؼ� ������ �ʰ� �����
        }
    }

    /// <summary>
    /// ������ �ִ� ������ ����� ȭ�� ����
    /// </summary>
    public void Refresh()
    {
        if(itemData != null)    // �����Ͱ� ������ ������ ����
        {
            itemName.text = itemData.itemName;
            itemPrice.text = itemData.value.ToString();
            itemImage.sprite = itemData.itemIcon;
        }
    }

    // ����Ƽ �̺�Ʈ �Լ� -------------------------------------------------------------------------------------------------
    private void Awake()
    {
        itemName = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        itemPrice = transform.Find("Value").GetComponent<TextMeshProUGUI>();
        itemImage = transform.Find("Icon").GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
}
