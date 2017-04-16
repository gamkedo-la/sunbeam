﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] GameObject m_pauseMenu;
    [SerializeField] GameObject m_messagePodsScreen;
    [SerializeField] GameObject m_controlsScreen;
    [SerializeField] GameObject m_creditsScreen;
    [SerializeField] GameObject m_messageScreen;
    [SerializeField] GameObject m_thanksForPlayingText;
    [SerializeField] GameObject m_continueExploringButton;
    [SerializeField] Button m_firstSelectedButton;
    [SerializeField] Button m_firstSelectedButtonPodInventory;

    private GameController m_gameController;
    private Button m_lastSelectedButtonMainMenu;
    private Button m_lastSelectedButtonPodInventory;


    void Awake()
    {
        m_gameController = FindObjectOfType<GameController>();
        DeactivateAllPanels();
        DeactivateContinueExploring();
        StoreLastMainMenuButton();
        StoreLastPodInventoryButton();
    }


    public void Resume()
    {
        EventManager.TriggerEvent(StandardEventName.Unpause);
    }


    public void ContinueExploring()
    {
        EventManager.TriggerEvent(StandardEventName.ContinueExploring);
        DeactivateContinueExploring();
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    private void DeactivateContinueExploring()
    {
        if (m_thanksForPlayingText != null)
            m_thanksForPlayingText.SetActive(false);

        if (m_continueExploringButton != null)
            m_continueExploringButton.SetActive(false);
    }


    public void ShowContinueExploring()
    {
        if (m_thanksForPlayingText != null)
            m_thanksForPlayingText.SetActive(true);

        if (m_continueExploringButton != null)
            m_continueExploringButton.SetActive(true);
    }


    private void DeactivateAllPanels()
    {
        if (m_pauseMenu != null)
            m_pauseMenu.SetActive(false);

        if (m_messagePodsScreen != null)
            m_messagePodsScreen.SetActive(false);

        if (m_controlsScreen != null)
            m_controlsScreen.SetActive(false);

        if (m_creditsScreen != null)
            m_creditsScreen.SetActive(false);

        if (m_messageScreen != null)
            m_messageScreen.SetActive(false);
    }


    private void StoreLastMainMenuButton()
    {
        if (m_lastSelectedButtonMainMenu == null)
            m_lastSelectedButtonMainMenu = m_firstSelectedButton;
        else
            m_lastSelectedButtonMainMenu = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

        //print("Storing last selected main menu button: " + m_lastSelectedButtonMainMenu.name);
    }


    private void StoreLastPodInventoryButton()
    {
        if (m_lastSelectedButtonPodInventory == null)
            m_lastSelectedButtonPodInventory = m_firstSelectedButtonPodInventory;
        else
            m_lastSelectedButtonPodInventory = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

        //print("Storing last selected pod inventory button: " + m_lastSelectedButtonPodInventory.name);
    }


    IEnumerator SetSelectButtonLater(Button selectedButton)
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(selectedButton.gameObject);
    }


    public void ShowPauseMenu(bool active)
    {
        if (m_lastSelectedButtonMainMenu != null)
            m_lastSelectedButtonMainMenu.Select();

        DeactivateAllPanels();

        if (m_pauseMenu != null)
            m_pauseMenu.SetActive(active);

        StartCoroutine(SetSelectButtonLater(m_lastSelectedButtonMainMenu));
    }


    public void ShowMessagePods(bool returnFromMessageScreen)
    {
        //if (returnFromMessageScreen)
        //    print("Returning to pod inventory from message screen");
        //else
        //    print("Opening pod inventory from main menu");

        if (!returnFromMessageScreen)
            StoreLastMainMenuButton();

        if (returnFromMessageScreen && m_lastSelectedButtonPodInventory != null)
            m_lastSelectedButtonPodInventory.Select();
        else if (m_firstSelectedButtonPodInventory != null)
            m_firstSelectedButtonPodInventory.Select();

        DeactivateAllPanels(); 

        if (m_messagePodsScreen != null)
            m_messagePodsScreen.SetActive(true);

        if (returnFromMessageScreen && m_lastSelectedButtonPodInventory != null)
            StartCoroutine(SetSelectButtonLater(m_lastSelectedButtonPodInventory));
        else if(m_firstSelectedButtonPodInventory != null)
            StartCoroutine(SetSelectButtonLater(m_firstSelectedButtonPodInventory));
    }


    public void ShowControls()
    {
        DeactivateAllPanels();
        StoreLastMainMenuButton();

        if (m_controlsScreen != null)
            m_controlsScreen.SetActive(true);
    }


    public void ShowCredits()
    {
        DeactivateAllPanels();
        StoreLastMainMenuButton();

        if (m_creditsScreen != null)
            m_creditsScreen.SetActive(true);
    }


    public void ShowMessage(MessagePodMenuDisplay messagePodMenuDisplay)
    {
        DeactivateAllPanels();
        StoreLastPodInventoryButton();

        if (m_messageScreen != null)
        {
            var showMessage = m_messageScreen.GetComponent<ShowMessage>();
            showMessage.SetMessageText(messagePodMenuDisplay.Message);
            m_messageScreen.SetActive(true);
        }
    }


    public void DeleteSaveData()
    {
        EventManager.TriggerEvent(StandardEventName.DeleteSaveData);
        ShowMessagePods(true);
    }


    public void ToggleInvertedY(bool inverted)
    {
        EventManager.TriggerEvent(BooleanEventName.ToggleInvertedY, inverted);
    }


    private void OnPause()
    {
        m_lastSelectedButtonMainMenu = m_firstSelectedButton;
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
