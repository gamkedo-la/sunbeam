using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MessagePodMenuDisplay : MonoBehaviour
{
    [SerializeField] GameObject m_messagePod;

    private Button m_button;
    private MessagePodFoundManager m_foundManager;
    private string m_message = "No pod assigned";


    void Awake()
    {
        m_button = GetComponent<Button>();

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


    void OnEnable()
    {
        m_button.interactable = m_foundManager != null && m_foundManager.Found;
    }
}
