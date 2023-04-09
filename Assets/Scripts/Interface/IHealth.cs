using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IHealth // 체력이 있는 대상일 때
{
    Transform transform { get; }
    float HP { get; set; }
    float MaxHP { get; }

    System.Action onHealthChange { get; set; }
}
