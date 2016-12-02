using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SundialController : PropControllerBase
{
    [SerializeField] Transform m_objectToRotate;
    [SerializeField] float m_rotationSpeed = 20f;


    void Awake()
    {
        if (m_objectToRotate == null)
        {
            var skyObject = GameObject.FindGameObjectWithTag(Tags.Sky);

            if (skyObject != null)
                m_objectToRotate = skyObject.transform;
        }
    }


    protected override void Update()
    {
        base.Update();

        if (!m_active)
            return;

        float h1 = Input.GetAxis("Horizontal");
        float h2 = Input.GetAxis("Horizontal look");

        float h = Mathf.Clamp(h1 + h2, -1f, 1f);

        m_objectToRotate.Rotate(Vector3.up, h * m_rotationSpeed * Time.deltaTime, Space.Self);
    }
}
