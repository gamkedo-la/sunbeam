using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagePodMenuDisplay : MonoBehaviour
{
    [SerializeField] GameObject m_messagePod;

    private MessagePodFoundManager m_foundManager;
    private string m_message = "No pod assigned";


    void Awake()
    {
        if (m_messagePod != null)
        {
            m_foundManager = m_messagePod.GetComponent<MessagePodFoundManager>();
            var text = m_messagePod.GetComponentInChildren<Text>();

            m_message = text.text;
        }
    }

	
    public string Message
    {
        get
        {
            return m_foundManager == null || m_foundManager.Found
                ? m_message 
                : "Pod not found";
        }
    }
}
