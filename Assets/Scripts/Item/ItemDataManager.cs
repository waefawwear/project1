using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameManager���� ������ ItemDataManager. ������ ������ �����͸� ������ ����
/// </summary>
public class ItemDataManager : MonoBehaviour
{
    /// <summary>
    /// ������ ������ ������
    /// </summary>
    public ItemData[] itemDatas;

    public ItemData this[uint i]     // �ε���
    {
        get => itemDatas[i];
    }

    public ItemData this[ItemIDCode code]   // �ε����� ���� ���ϰ� ������ ������ �����Ϳ� ����(enum���� �迭�����ϰ� ����)
    {
        get => itemDatas[(int)code];
    }

    /// <summary>
    /// ������ ���� ����
    /// </summary>
    public int Length
    {
        get => itemDatas.Length;
    }
}
