using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 데이터를 저장하는 데이터 파일을 만들게 해주는 스크립트
/// </summary>
[CreateAssetMenu(fileName = "New Coin Item Data", menuName = "Scriptable Object/Item Data - Coin", order = 2)]
public class ItemData_Coin : ItemData, IConsumable   // 내가 원하는 데이터를 저장할 수 있는 데이터 파일을 설계힐 수 있게 해주는 클라스
{
    public void Consume(Player player)
    {
        player.Money += (int)value;
    }
}
