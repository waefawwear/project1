using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEXPBar : MonoBehaviour
{
    IGrowth target;
    Image value;

    private void Awake()
    {
        target = GameObject.Find("Player").GetComponent<IGrowth>();
        target.onEXPChange += SetEXP_Value;
        value = GetComponent<Image>();
    }

    /// <summary>
    /// ����ġ�� ��
    /// </summary>
    void SetEXP_Value()
    {
        if (target != null)
        {
            float ratio = target.EXP / target.MaxEXP;   // ���� ����ġ / �ִ� ����ġ
            value.fillAmount = ratio;
        }
    }
}
