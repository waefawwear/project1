using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Enemy : MonoBehaviour
{
    public GameObject hitEffect;
    public Collider weaponCol;
    Enemy enemy;

    private void Awake()
    {
        weaponCol = GetComponent<Collider>();
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger == false)
        {
            IBattle battle = other.GetComponent<IBattle>();
            if (battle != null)
            {
                enemy.Attack(battle);

                Vector3 hitPoint = transform.position + transform.up;
                Vector3 effectPoint = other.ClosestPoint(hitPoint);
                Instantiate(hitEffect, effectPoint, Quaternion.identity);
            }
        }
    }
}
