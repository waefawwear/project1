using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bear : Enemy
{
    Weapon_Enemy weapon;
    public float dropExp = 20.0f;   // óġ �� ȹ�� ����ġ

    protected override void Awake()
    {
        base.Awake();
        weapon = GetComponentInChildren<Weapon_Enemy>();
    }

    protected override void Update()
    {
        base.Update();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))   // ���� �� ���� �ݶ��̴� Ȱ��ȭ
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
    /// ������ ���
    /// </summary>
    void DropItem()
    {
        GameManager.Inst.MainPlayer.Inven.AddItem(ItemIDCode.BearMeat);
    }
}
