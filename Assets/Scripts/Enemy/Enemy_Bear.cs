using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bear : Enemy
{
    Weapon_Enemy weapon;
    public float dropExp = 20.0f;   // 처치 시 획득 경험치

    protected override void Awake()
    {
        base.Awake();
        weapon = GetComponentInChildren<Weapon_Enemy>();
    }

    protected override void Update()
    {
        base.Update();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))   // 공격 시 무기 콜라이더 활성화
        {
            weapon.weaponCol.enabled = true;
        }
        else
        {
            weapon.weaponCol.enabled = false;
        }
    }

    protected override void DeadState()
    {
        base.DeadState();
        GameManager.Inst.MainPlayer.EXP += dropExp;
        DropItem();
    }

    /// <summary>
    /// 아이템 드랍
    /// </summary>
    void DropItem()
    {
        GameManager.Inst.MainPlayer.Inven.AddItem(ItemIDCode.BearMeat);
    }
}
