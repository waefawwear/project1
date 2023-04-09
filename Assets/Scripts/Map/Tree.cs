using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    Animator anim;

    int life = 3;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon")) // ���⿡ �¾��� ��
        {
            anim.SetTrigger("GetHit");
            life--;
            if (life == 0)
            {
                Player player = other.GetComponentInParent<Player>();
                if (player != null)
                {
                    DropItem(player);   // �÷��̾�� ������ ���
                }
                Destroy(this.gameObject);
            }
        }
    }

    void DropItem(Player player)
    {
        player.Inven.AddItem(ItemIDCode.WoodBlock);
    }
}
