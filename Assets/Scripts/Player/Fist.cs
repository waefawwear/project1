using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : MonoBehaviour
{
    public GameObject hitEffect;
    public Collider weaponCol;

    private void Awake()
    {
        weaponCol = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Inst.MainPlayer.WeaponMode == PlayerWeaponMode.Noweapons)
        {
            if (other.isTrigger == false)
            {
                IBattle battle = other.GetComponent<IBattle>();
                if (battle != null)
                {
                    GameManager.Inst.MainPlayer.Attack(battle);

                    Vector3 hitPoint = transform.position + transform.up;
                    Vector3 effectPoint = other.ClosestPoint(hitPoint);
                    Instantiate(hitEffect, effectPoint, Quaternion.identity);
                }
            }
        }
    }
}
