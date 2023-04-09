using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ 1���� ��Ÿ�� Ŭ����
/// </summary>
public class Item : MonoBehaviour
{
    public ItemData data;       // �� ������ �������� ������ ������

    private void Start()
    {
        // ������ ����. Awake�� ���� data�� ���� ������ Start���� ����
        Instantiate(data.prefab, transform.position, transform.rotation, transform);
    }
}
