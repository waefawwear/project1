using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMana  // ������ �ִ� ����� ��
{
    Transform transform { get; }
    public float MP { get; set; }
    public float MaxMP { get; }

    System.Action onManaChange { get; set; }
}