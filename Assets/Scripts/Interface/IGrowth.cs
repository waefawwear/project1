using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IGrowth   // ������ �� �� �ִ� ����� ��(�÷��̾�)
{
    Transform transform { get; }
    public float EXP { get; set; }
    public float MaxEXP { get; set; }
    public int Level { get; set; }
    public int MaxLevel { get; }

    System.Action onEXPChange { get; set; }
    System.Action onLevelChange { get; set; }
}
