using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HitDamage : MonoBehaviour
{
    float hitDamage;
    TextMeshProUGUI damageText;

    private void Start()
    {
        damageText = GetComponentInChildren<TextMeshProUGUI>();

        hitDamage = GameManager.Inst.MainPlayer.Damage;
        damageText.text = hitDamage.ToString();
    }

    private void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
