using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] GameObject m_pauseManu;
    [SerializeField] GameObject m_controlsScreen;
    [SerializeField] GameObject m_creditsScreen;


    void Awake()
    {
        if (m_pauseManu != null)
            m_pauseManu.SetActive(false);

        if (m_controlsScreen != null)
            m_controlsScreen.SetActive(false);

        if (m_creditsScreen != null)
            m_creditsScreen.SetActive(false);
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


    public void ShowControls()
    {

    }


    public void ShowCredits()
    {

    }


    private void OnPause()
    {
        m_pauseManu.SetActive(true);
        //print("Pause menu activated");
    }


    private void OnUnpause()
    {
        m_pauseManu.SetActive(false);
        //print("Pause menu deactivated");
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
