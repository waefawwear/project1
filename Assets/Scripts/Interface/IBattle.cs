using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattle    // 싸울 수 있는 대상이면
{
    Transform transform { get; }
    float AttackPower { get; set; }
    float DefencePower { get; set; }
    bool IsInvincibility { get; set; }

    void Attack(IBattle target);
    void TakeDamage(float damage);
}
