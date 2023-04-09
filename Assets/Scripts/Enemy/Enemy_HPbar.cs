using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_HPbar : MonoBehaviour
{
    IHealth target;
    Image fill;

    private void Awake()
    {
        target = GetComponentInParent<IHealth>();
        target.onHealthChange += SetHP_Value;
        fill = transform.Find("Fill").GetComponent<Image>();
    }

    void SetHP_Value()
    {
        if (target != null)
        {
            float ratio = target.HP / target.MaxHP;
            fill.fillAmount = ratio;
        }
    }

    private void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
