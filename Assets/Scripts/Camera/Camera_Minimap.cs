using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Minimap : MonoBehaviour
{
    Transform target;
    Vector3 offset;

    private void Awake()
    {
        target = FindObjectOfType<Player>().transform;
        offset = transform.position - target.position;
    }
    private void LateUpdate()
    {
        transform.position = target.position + offset;
    }
}
