using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatPoints : MonoBehaviour
{
    TextMeshProUGUI stat;

    float statPoint = 0;

    public float StatPoint
    {
        get => statPoint;
        set
        {
            statPoint = value;
            stat.text = statPoint.ToString();
        }
    }

    private void Awake()
    {
        stat = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        statPoint = 0;
    }
}
