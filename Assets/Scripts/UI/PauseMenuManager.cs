﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] GameObject m_pauseMenu;
    [SerializeField] GameObject m_controlsScreen;
    [SerializeField] GameObject m_creditsScreen;

    private GameController m_gameController;


    void Awake()
    {
        m_gameController = GameObject.FindObjectOfType<GameController>();
        DeactivateAllPanels();
    }


    public void Resume()
    {
        EventManager.TriggerEvent(StandardEventName.Unpause);
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    private void DeactivateAllPanels()
    {
        if (m_pauseMenu != null)
            m_pauseMenu.SetActive(false);

        if (m_controlsScreen != null)
            m_controlsScreen.SetActive(false);

        if (m_creditsScreen != null)
            m_creditsScreen.SetActive(false);
    }


    public void ShowPauseMenu(bool active)
    {
        DeactivateAllPanels();

        if (m_pauseMenu != null)
            m_pauseMenu.SetActive(active);
    }


    public void ShowControls()
    {
        DeactivateAllPanels();

        if (m_controlsScreen != null)
            m_controlsScreen.SetActive(true);
    }


    public void ShowCredits()
    {
        DeactivateAllPanels();

        if (m_creditsScreen != null)
            m_creditsScreen.SetActive(true);
    }


    private void OnPause()
    {
        ShowPauseMenu(true);
    }


    private void OnUnpause()
    {
        ShowPauseMenu(false);
    }


    void OnEnable()
    {
        EventManager.StartListening(StandardEventName.Pause, OnPause);
        EventManager.StartListening(StandardEventName.Unpause, OnUnpause);
    }


    void OnDisable()
    {
        EventManager.StopListening(StandardEventName.Pause, OnPause);
        EventManager.StopListening(StandardEventName.Unpause, OnUnpause);
    }
}