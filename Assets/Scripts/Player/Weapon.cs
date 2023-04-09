using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    public GameObject hitEffect;    // 타격 시 파티클
    public Collider weaponCol;      // 무기 콜라이더

    PlayerInput playerInput;

    private void Awake()
    {
        weaponCol = GetComponent<Collider>();
        playerInput = GetComponentInParent<PlayerInput>();
        playerInput.Weapon = GetComponent<Weapon>();

        gameObject.tag = "Weapon";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger == false)
        {
            IBattle battle = other.GetComponent<IBattle>();     
            if (battle != null)     // IBattle을 타겟이 가지고 있으면
            {
                GameManager.Inst.MainPlayer.Attack(battle);     // 무기로 대상을 가격한다

                Vector3 hitPoint = transform.position + transform.up;   // 타격 지점을 저장
                Vector3 effectPoint = other.ClosestPoint(hitPoint);    
                Instantiate(hitEffect, effectPoint, Quaternion.identity);    // 타격지점에 파티클 생성
            }
        }
    }
}
