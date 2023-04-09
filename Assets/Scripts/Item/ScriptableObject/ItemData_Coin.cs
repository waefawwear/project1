using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ �����͸� �����ϴ� ������ ������ ����� ���ִ� ��ũ��Ʈ
/// </summary>
[CreateAssetMenu(fileName = "New Coin Item Data", menuName = "Scriptable Object/Item Data - Coin", order = 2)]
public class ItemData_Coin : ItemData, IConsumable   // ���� ���ϴ� �����͸� ������ �� �ִ� ������ ������ ������ �� �ְ� ���ִ� Ŭ��
{
    public void Consume(Player player)
    {
        player.Money += (int)value;
    }
}
