using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipTarget   // 착용할 수 있는 대상일때
{
    ItemSlot EquipWeaponSlot { get; }      // 장비한 아이템(무기)

    ItemSlot EquipShieldSlot { get; }   // 장비한 아이템(무기)

    void EquipWeapon(ItemSlot weaponSlot);   // 무기 장비하기

    void UnEquipWeapon();                   // 무기 해제하기

    void EquipShield(ItemSlot weaponSlot);   // 방패 장비하기

    void UnEquipShield();                   // 방패 해제하기
}

