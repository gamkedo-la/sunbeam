using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class PrefsForUseJoypad : SwitchToggle
{
    public static string UseJoypadPrefs = "Use joypad";

    private Toggle m_toggle;


    void Awake()
    {
        m_toggle = GetComponent<Toggle>();
    }


    public override void Load()
    {
        m_toggle = GetComponent<Toggle>();
        int toggleState = PlayerPrefs.GetInt(UseJoypadPrefs, 0);

        m_toggle.isOn = toggleState == 1;

        Toggle(m_toggle.isOn);
    }


    public override void Toggle(bool on)
    {
        int state = on ? 1 : 0;
        PlayerPrefs.SetInt(name, state);

        EventManager.TriggerEvent(BooleanEventName.ToggleUseJoypad, on);
    }
}
