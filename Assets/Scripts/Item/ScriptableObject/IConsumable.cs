using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IConsumable   // 먹을 수 있는 아이템
{
    void Consume(Player player);
}
