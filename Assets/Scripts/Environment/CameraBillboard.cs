using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBillboard : MonoBehaviour
{
    private Transform m_camera;
    private Vector3 m_up;

    void Awake()
    {
        m_camera = Camera.main.transform;
        m_up = transform.up;
    }


    void Update()
    {
        var direction = transform.position - m_camera.position;
        var positionOnPlane = Vector3.ProjectOnPlane(direction, m_up);
        var targetPosition = transform.position + positionOnPlane;
        transform.LookAt(targetPosition, m_up);
    }
}
