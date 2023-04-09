using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipTarget   // ������ �� �ִ� ����϶�
{
    ItemSlot EquipWeaponSlot { get; }      // ����� ������(����)

    ItemSlot EquipShieldSlot { get; }   // ����� ������(����)

    void EquipWeapon(ItemSlot weaponSlot);   // ���� ����ϱ�

    void UnEquipWeapon();                   // ���� �����ϱ�

    void EquipShield(ItemSlot weaponSlot);   // ���� ����ϱ�

    void UnEquipShield();                   // ���� �����ϱ�
}

