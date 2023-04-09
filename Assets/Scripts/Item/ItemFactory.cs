using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ������ Ŭ����(������)
/// </summary>
public class ItemFactory
{
    static int itemCount = 0;   // �̶����� ������ �� ������ ����. (�� �����ۺ� ID)

    /// <summary>
    /// ������ ����
    /// </summary>
    /// <param name="code">������ �������� ����</param>
    /// <returns>������ ���� ������Ʈ</returns>

    public static GameObject MakeItem(ItemIDCode code)
    {
        GameObject obj = new GameObject();              // �� ������Ʈ �����
        Item item = obj.AddComponent<Item>();           // Item ������Ʈ �߰�

        item.data = GameManager.Inst.ItemData[code];    // ItemData ����
        string[] itemName = item.data.name.Split("_");  // ���� �����ϴ� ������ �°� �̸� ����
        obj.name = $"{itemName[1]}_{itemCount}";        // ���� ���̵� �߰�
        obj.layer = LayerMask.NameToLayer("Item");      // Layer ����
        SphereCollider col = obj.AddComponent<SphereCollider>();    // �ڵ�� Collider �߰�
        col.radius = 0.5f;
        col.isTrigger = true;
        itemCount++;    // ������ ������ ���� �������Ѽ� �ߺ��� ������ ó��

        GameObject indicator = Resources.Load<GameObject>("MinimapItemIndicator_Item");
        if(indicator != null)
        {
            GameObject.Instantiate(indicator, obj.transform.position, obj.transform.rotation * Quaternion.Euler(90, 0, 0), obj.transform);
        }

        return obj;     // �����Ϸ�� �� ����
    }

    public static GameObject MakeItem(ItemIDCode code, Vector3 position, bool randomNoise = false)
    {
        GameObject obj = MakeItem(code);
        if (randomNoise)
        {
            Vector2 noise = Random.insideUnitCircle * 0.5f;
            position.x += noise.x;
            position.z += noise.y;
        }
        obj.transform.position = position;
        return obj;
    }
    public static void MakeItems(ItemIDCode code, Vector3 position, uint count)
    {
        for(int i=0; i<count; i++)
        {
            MakeItem(code, position, true);
        }
    }

    public static GameObject MakeItem(uint id)
    {
        return MakeItem((ItemIDCode)id);
    }

    public static GameObject MakeItem(uint id, Vector3 position, bool randomNoise = false)
    {
        return MakeItem((ItemIDCode)id, position, randomNoise);
    }

    public static void MakeItems(uint id, Vector3 position, uint count)
    {
        MakeItems((ItemIDCode)id, position, count);
    }
}
