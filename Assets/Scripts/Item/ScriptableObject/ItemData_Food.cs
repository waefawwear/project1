using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Weapon Item Data", menuName = "Scriptable Object/Item Data - Food", order = 5)]
public class ItemData_Food : ItemData, IUsable
{
    [Header("식량 데이터")]
    public float healPoint;

    public void Use(GameObject target = null)
    {
        IHealth health = target.GetComponent<IHealth>();
        if (health != null)
        {
            if (health.HP + healPoint <= health.MaxHP)
            {
                health.HP += healPoint;
            }
            else
            {
                health.HP = health.MaxHP;
            }
        }
    }
}
