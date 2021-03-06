﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropControllerBase : MonoBehaviour, IActivatable
{
    [Header("Camera view point")]
    [SerializeField] Transform m_cameraPoint;


    protected bool m_active;
    protected bool m_activationTiggered;

    
    protected virtual void Awake()
    {
        var camera = GetComponentInChildren<Camera>();

        if (camera != null)
        {
            camera.enabled = false;
            camera.gameObject.SetActive(false);
        }

        if (m_cameraPoint == null)
        {
            if (camera != null)
                m_cameraPoint = camera.transform;
        }
    }
	

    public void TriggerActivatation()
    {
        //print("Trigger activation called");
        if (!m_activationTiggered)
        {
            //print("Trigger activation");
            m_activationTiggered = true;
            EventManager.TriggerEvent(TransformEventName.PropActivated, m_cameraPoint, this);
        }
    }


    public virtual void TriggerDeactivation()
    {
        //print("Trigger deactivation called");
        if (m_activationTiggered)
        {
            //print("Trigger deactivation");
            m_active = false;
            m_activationTiggered = false;
            EventManager.TriggerEvent(StandardEventName.PropDeactivated);
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


    protected virtual void TriggerClosingCinematic()
    {
        m_active = false;
        m_activationTiggered = false;
    }


    void OnEnable()
    {
        EventManager.StartListening(StandardEventName.TriggerClosingCinematic, TriggerClosingCinematic);
        //EventManager.StartListening(StandardEventName.ContinueExploring, ContinueExploring);
    }


    void OnDisable()
    {
        EventManager.StopListening(StandardEventName.TriggerClosingCinematic, TriggerClosingCinematic);
        //EventManager.StopListening(StandardEventName.ContinueExploring, ContinueExploring);
    }
}
