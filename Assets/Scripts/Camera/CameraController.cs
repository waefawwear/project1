using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform cam_Player;

    private void Update()
    {
        RotateCam();
    }

    /// <summary>
    /// 마우스 움직임에 따라 카메라 회전
    /// </summary>
    private void RotateCam()
    {
        Vector2 mouseDel = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); // 마우스 움직임 받기
        Vector3 camAngle = cam_Player.rotation.eulerAngles;

        float x = camAngle.x + mouseDel.y;

        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }

        cam_Player.rotation = Quaternion.Euler(x, camAngle.y + mouseDel.x, camAngle.z);

    }
}
