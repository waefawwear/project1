using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState  // ���� ����
{
    Idle = 0,
    Patrol,
    Chase,
    Attack,
    Dead
}

public enum PlayerWeaponMode    // �÷��̾� ���� ���� ����
{
    Noweapons = 0,
    OneHandWeapon
}

public enum PlayerShieldMode    // �÷��̾� ���� ���� ����
{
    NoShield = 0,
    Shield
}

public enum ItemIDCode  // ������ �����ڵ�
{
    OneHandSword1 = 0,
    OneHandAxe,
    Shield,
    WoodBlock,
    BearMeat
}
