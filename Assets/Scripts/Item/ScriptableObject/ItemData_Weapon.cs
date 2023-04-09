using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Weapon Item Data", menuName = "Scriptable Object/Item Data - Weapon", order = 3)]
public class ItemData_Weapon : ItemData, IEquipItem
{
    public bool isWeapon = true;
    [Header("무기 데이터")]
    public float attackPower = 10.0f;
    public float attackSpeed = 1.0f;

    bool IEquipItem.isWeapon => isWeapon;
}
