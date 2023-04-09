using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IUsable   // 사용할 수 있는 아이템
{
    void Use( GameObject target = null);
}
