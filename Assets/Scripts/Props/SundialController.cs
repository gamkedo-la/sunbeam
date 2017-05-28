using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SundialController : PropControllerBase
{
    [SerializeField] Transform m_objectToRotate;
    [SerializeField] float m_rotationSpeed = 20f;


    protected override void Awake()
    {
        base.Awake();

        if (m_objectToRotate == null)
        {
            var skyObject = GameObject.FindGameObjectWithTag(Tags.Sky);

            if (skyObject != null)
                m_objectToRotate = skyObject.transform;
        }
    }


    void Update()
    {
        //base.Update();

        if (!m_active)
            return;

        float h1;
        float h2;

        if (GameController.UseJoystick)
        {
            h1 = Input.GetAxis("Horizontal joystick");
            h2 = Input.GetAxis("Horizontal look joystick");
        }
        else
        {
            h1 = Input.GetAxis("Horizontal");
            h2 = Input.GetAxis("Horizontal look");
        }

        float h = Mathf.Clamp(h1 + h2, -1f, 1f);

        m_objectToRotate.Rotate(Vector3.up, h * m_rotationSpeed * Time.deltaTime, Space.Self);
    }
}
