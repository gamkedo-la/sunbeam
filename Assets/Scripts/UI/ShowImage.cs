using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ShowImage : MonoBehaviour
{
    [SerializeField] BooleanEventName m_eventName;
    [SerializeField] bool m_enabledAtStart;

    private Image m_image;


    void Awake()
    {
        m_image = GetComponent<Image>();
        m_image.enabled = m_enabledAtStart;
    }
	

    private void SetImageEnabled(bool enable)
    {
        m_image.enabled = enable;
    }


    void OnEnable()
    {
        EventManager.StartListening(m_eventName, SetImageEnabled);
    }


    void OnDisable()
    {
        EventManager.StopListening(m_eventName, SetImageEnabled);
    }
}
