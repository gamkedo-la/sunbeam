﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GateSaveStateManager : MonoBehaviour
{
    [SerializeField] bool m_saveStateOfGate = true;
    [SerializeField] string m_animationTriggerOnLoad = "Open";

    private Animator m_anim;


    void Awake()
    {
        m_anim = GetComponent<Animator>();
    }


	public void SaveOpenGateState()
    {
        if (m_saveStateOfGate)
        {
            print("Save door open: " + name);
            PlayerPrefs.SetInt(name, 1);
        }
    }


    private void LoadGateState()
    {
        int open = PlayerPrefs.GetInt(name, 0);

        if (open == 1)
        {
            print("Load door open: " + name);
            m_anim.SetTrigger(m_animationTriggerOnLoad);
        }
    }


    private void DeleteGateState()
    {
        PlayerPrefs.DeleteKey(name);
    }


    void OnEnable()
    {
        EventManager.StartListening(StandardEventName.LoadSaveGame, LoadGateState);
        EventManager.StartListening(StandardEventName.DeleteSaveData, DeleteGateState);
    }


    void OnDisable()
    {
        EventManager.StopListening(StandardEventName.LoadSaveGame, LoadGateState);
        EventManager.StopListening(StandardEventName.DeleteSaveData, DeleteGateState);
    }
}
