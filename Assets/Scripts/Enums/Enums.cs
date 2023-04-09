using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState  // 적의 상태
{
    Idle = 0,
    Patrol,
    Chase,
    Attack,
    Dead
}

public enum PlayerWeaponMode    // 플레이어 무기 장착 상태
{
    Noweapons = 0,
    OneHandWeapon
}

public enum PlayerShieldMode    // 플레이어 방패 장착 상태
{
    NoShield = 0,
    Shield
}

public enum ItemIDCode  // 아이템 고유코드
{
    OneHandSword1 = 0,
    OneHandAxe,
    Shield,
    WoodBlock,
    BearMeat
}
