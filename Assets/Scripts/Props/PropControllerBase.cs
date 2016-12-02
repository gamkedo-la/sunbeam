﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropControllerBase : MonoBehaviour, IActivatable
{
    [Header("Camera view point")]
    [SerializeField] Transform m_cameraPoint;


    protected bool m_active;
    protected bool m_canBeActivated;
    protected bool m_activationTiggered;

    
    void Awake()
    {
        var camera = GetComponentInChildren<Camera>();

        if (camera != null)
            camera.enabled = false;

        if (m_cameraPoint == null)
        {
            if (camera != null)
                m_cameraPoint = camera.transform;
        }
    }
	

	protected virtual void Update()
    {
        if (m_canBeActivated)
        {
            if (!m_activationTiggered && Input.GetAxisRaw("Submit") == 1)
            {
                m_activationTiggered = true;
                EventManager.TriggerEvent(TransformEventName.PropActivated, m_cameraPoint);
            }
            else if (m_activationTiggered && Input.GetAxisRaw("Cancel") == 1)
            {
                m_active = false;
                m_activationTiggered = false;
                EventManager.TriggerEvent(StandardEventName.PropDeactivated);
            }
        }
    }


    public void Activate()
    {
        m_active = true;
    }


    public void Deactivate()
    {
        m_active = false;
    }


    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Tags.Player))
            return;

        m_canBeActivated = true;
    }


    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(Tags.Player))
            return;

        m_canBeActivated = false;
    }
}
