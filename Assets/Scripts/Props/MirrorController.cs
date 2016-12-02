﻿using UnityEngine;
using System.Collections;

public class MirrorController : PropControllerBase
{
    [Header("Rotation")]
    [SerializeField] Transform m_rotationPoint;
    [SerializeField] float m_rotationSpeed = 20f;
    [SerializeField] Vector2 m_rotationMinMax = new Vector2(-30, 30);

    [Header("Pitch")]
    [SerializeField] Transform m_pitchPoint;
    [SerializeField] float m_pitchSpeed = 10f;
    [SerializeField] Vector2 m_pitchMinMax = new Vector2(20, 40);   

    [Header("Gizmos")]
    [SerializeField] float m_gizmoLineLength = 10f;

    private float m_rotation;
    private float m_pitch;


    void Awake()
    {
        m_pitch = m_pitchPoint.localEulerAngles.x;

        if (m_pitch > 180f)
            m_pitch = 360f - m_pitch;

        m_rotation = m_rotationPoint.localEulerAngles.y;

        if (m_rotation > 180f)
            m_rotation = 360f - m_rotation;

        ClampAngles();
    }


    protected override void Update()
    {
        base.Update();

        if (!m_active)
            return;

        float h1 = Input.GetAxis("Horizontal");
        float v1 = Input.GetAxis("Vertical");
        float h2 = Input.GetAxis("Horizontal look");
        float v2 = Input.GetAxis("Vertical look");

        float h = Mathf.Clamp(h1 + h2, -1f, 1f);
        float v = Mathf.Clamp(v1 + v2, -1f, 1f);

        m_rotation += h * m_rotationSpeed * Time.deltaTime;
        m_pitch += v * m_pitchSpeed * Time.deltaTime;

        ClampAngles();
    }


    private void ClampAngles()
    {
        m_rotation = Mathf.Clamp(m_rotation, m_rotationMinMax.x, m_rotationMinMax.y);
        m_rotationPoint.localEulerAngles = Vector3.up * m_rotation;

        m_pitch = Mathf.Clamp(m_pitch, m_pitchMinMax.x, m_pitchMinMax.y);
        m_pitchPoint.localEulerAngles = Vector3.left * m_pitch;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        var rotationMin = Quaternion.Euler(transform.up * m_rotationMinMax.x) * transform.forward;
        var rotationMax = Quaternion.Euler(transform.up * m_rotationMinMax.y) * transform.forward;

        Gizmos.DrawRay(m_rotationPoint.position + m_rotationPoint.up * 0.5f, rotationMin * m_gizmoLineLength);
        Gizmos.DrawRay(m_rotationPoint.position + m_rotationPoint.up * 0.5f, rotationMax * m_gizmoLineLength);

        Gizmos.color = Color.cyan;

        var pitchMin = Quaternion.Euler(-transform.right * m_pitchMinMax.x) * transform.forward;
        var pitchMax = Quaternion.Euler(-transform.right * m_pitchMinMax.y) * transform.forward;

        Gizmos.DrawRay(m_pitchPoint.position, pitchMin * m_gizmoLineLength);
        Gizmos.DrawRay(m_pitchPoint.position, pitchMax * m_gizmoLineLength);
    }
}
