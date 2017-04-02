using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagePodFoundManager : MonoBehaviour
{
    private bool m_found;


    void Awake()
    {
        int found = PlayerPrefs.GetInt(name, 0);
        m_found = found == 1;
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
