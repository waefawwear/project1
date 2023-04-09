using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBar : MonoBehaviour
{
    IHealth target;
    Image health;

    private void Awake()
    {
        target = GameObject.Find("Player").GetComponent<IHealth>();
        target.onHealthChange += SetHP_Value;
        health = GetComponent<Image>();
    }

    void SetHP_Value()
    {
        if (target != null)
        {
            float ratio = target.HP / target.MaxHP;
            health.fillAmount = ratio;
        }
    }
}
