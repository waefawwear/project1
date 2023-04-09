using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ �����͸� �����ϴ� ������ ������ ����� ���ִ� ��ũ��Ʈ
/// </summary>
[CreateAssetMenu(fileName = "New Item Data", menuName = "Scriptable Object/Item Data", order = 1)]
public class ItemData : ScriptableObject    // ���� ���ϴ� �����͸� ������ �� �ִ� ������ ������ ������ �� �ְ� ���ִ� Ŭ��
{
    [Header("�⺻ ������")]
    public uint id = 0;                 // ������ ID
    public string itemName = "������";  // �����۸�
    public Sprite itemIcon;
    public GameObject prefab;          // �������� ������ 
    public uint value;                 // �������� ��ġ
    public uint maxStackCount = 1;         // ������ �ִ� ���� ��
}
