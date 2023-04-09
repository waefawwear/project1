using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    public GameObject hitEffect;    // Ÿ�� �� ��ƼŬ
    public Collider weaponCol;      // ���� �ݶ��̴�

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
            if (battle != null)     // IBattle�� Ÿ���� ������ ������
            {
                GameManager.Inst.MainPlayer.Attack(battle);     // ����� ����� �����Ѵ�

                Vector3 hitPoint = transform.position + transform.up;   // Ÿ�� ������ ����
                Vector3 effectPoint = other.ClosestPoint(hitPoint);    
                Instantiate(hitEffect, effectPoint, Quaternion.identity);    // Ÿ�������� ��ƼŬ ����
            }
        }
    }
}
