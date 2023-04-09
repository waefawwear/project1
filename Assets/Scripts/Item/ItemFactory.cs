using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 생성용 클래스(생성만)
/// </summary>
public class ItemFactory
{
    static int itemCount = 0;   // 이때까지 생성된 총 아이템 갯수. (각 아이템별 ID)

    /// <summary>
    /// 아이템 생성
    /// </summary>
    /// <param name="code">생성할 아이템의 종류</param>
    /// <returns>생성한 게임 오브젝트</returns>

    public static GameObject MakeItem(ItemIDCode code)
    {
        GameObject obj = new GameObject();              // 빈 오브젝트 만들기
        Item item = obj.AddComponent<Item>();           // Item 컴포넌트 추가

        item.data = GameManager.Inst.ItemData[code];    // ItemData 설정
        string[] itemName = item.data.name.Split("_");  // 내가 생성하는 종류에 맞게 이름 변경
        obj.name = $"{itemName[1]}_{itemCount}";        // 고유 아이디도 추가
        obj.layer = LayerMask.NameToLayer("Item");      // Layer 설정
        SphereCollider col = obj.AddComponent<SphereCollider>();    // 코드로 Collider 추가
        col.radius = 0.5f;
        col.isTrigger = true;
        itemCount++;    // 생성할 떄마다 값을 증가시켜서 중복이 없도록 처리

        GameObject indicator = Resources.Load<GameObject>("MinimapItemIndicator_Item");
        if(indicator != null)
        {
            GameObject.Instantiate(indicator, obj.transform.position, obj.transform.rotation * Quaternion.Euler(90, 0, 0), obj.transform);
        }

        return obj;     // 생성완료된 것 리턴
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
