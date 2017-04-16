using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class PrefsForInvertedY : MonoBehaviour
{
    [SerializeField] BooleanEventName m_eventToTriggerOnToggle = BooleanEventName.ToggleInvertedJoypadLookY;

    private Toggle m_toggle;


    void Awake()
    {
        m_toggle = GetComponent<Toggle>();
        int toggleState = PlayerPrefs.GetInt(name, 0);

        m_toggle.isOn = toggleState == 1;
    }


    public void ToggleInvertedY(bool inverted)
    {
        int state = inverted ? 1 : 0;
        PlayerPrefs.SetInt(name, state);

        EventManager.TriggerEvent(m_eventToTriggerOnToggle, inverted);
    }
}
