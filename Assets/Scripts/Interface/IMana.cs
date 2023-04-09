using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMana  // 마나가 있는 대상일 때
{
    Transform transform { get; }
    public float MP { get; set; }
    public float MaxMP { get; }

    System.Action onManaChange { get; set; }
}