using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IGrowth   // 레벨업 할 수 있는 대상일 때(플레이어)
{
    Transform transform { get; }
    public float EXP { get; set; }
    public float MaxEXP { get; set; }
    public int Level { get; set; }
    public int MaxLevel { get; }

    System.Action onEXPChange { get; set; }
    System.Action onLevelChange { get; set; }
}
