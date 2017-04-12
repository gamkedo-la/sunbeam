using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagePodFoundManager : MonoBehaviour
{
    [SerializeField] Collider m_playerTrigger;
    [SerializeField] Canvas m_messageCanvas;

    private bool m_found;


    void Awake()
    {
        int found = PlayerPrefs.GetInt(name, 0);
        m_found = found == 1;     
    }


    void Start()
    {
        if (m_found)
        {
            if (m_playerTrigger != null)
                m_playerTrigger.enabled = false;

            if (m_messageCanvas != null)
                m_messageCanvas.enabled = true;
        }
    }


    public void SetFoundToTrue()
    {
        m_found = true;
        PlayerPrefs.SetInt(name, 1);
    }


    private void SetFoundToFalse()
    {
        m_found = false;
        PlayerPrefs.SetInt(name, 0);

        if (m_playerTrigger != null)
            m_playerTrigger.enabled = true;

        if (m_messageCanvas != null)
            m_messageCanvas.enabled = false;
    }


    public bool Found
    {
        get { return m_found; }
    }


    void OnEnable()
    {
        EventManager.StartListening(StandardEventName.DeleteSaveData, SetFoundToFalse);
    }


    void OnDisable()
    {
        EventManager.StopListening(StandardEventName.DeleteSaveData, SetFoundToFalse);
    }
}
