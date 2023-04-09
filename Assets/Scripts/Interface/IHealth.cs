using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IHealth // ü���� �ִ� ����� ��
{
    Transform transform { get; }
    float HP { get; set; }
    float MaxHP { get; }

    System.Action onHealthChange { get; set; }
}
