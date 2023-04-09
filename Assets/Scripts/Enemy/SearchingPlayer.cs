using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchingPlayer : MonoBehaviour
{
    Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.Inst.MainPlayer.gameObject)
        {
            enemy.attackTarget = other.GetComponent<IBattle>();
            enemy.ChangeState(EnemyState.Attack);
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == GameManager.Inst.MainPlayer.gameObject)
        {
            enemy.ChangeState(EnemyState.Chase);
            return;
        }
    }
}
