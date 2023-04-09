using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Shield Item Data", menuName = "Scriptable Object/Item Data - Shield", order = 4)]
public class ItemData_Shield : ItemData, IEquipItem
{
    public bool isWeapon = true;
    [Header("방패 데이터")]
    public float defensePower = 10.0f;

    bool IEquipItem.isWeapon => isWeapon;
}
