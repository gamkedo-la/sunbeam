using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LampStateSaveManager : MonoBehaviour
{
    private Light m_light;


    void Awake()
    {
        m_light = GetComponent<Light>();
    }


    public void SaveLampOnState()
    {
        print("Save lamp on: " + name);
        PlayerPrefs.SetInt(name, 1);
    }


    private void LoadLampState()
    {
        int lampOn = PlayerPrefs.GetInt(name, 0);

        if (lampOn == 1)
        {
            print("Load lamp on: " + name);
            m_light.enabled = true;
        }
    }


    private void DeleteLampState()
    {
        PlayerPrefs.DeleteKey(name);
    }


    void OnEnable()
    {
        EventManager.StartListening(StandardEventName.LoadSaveGame, LoadLampState);
        EventManager.StartListening(StandardEventName.DeleteSaveData, DeleteLampState);
    }


    void OnDisable()
    {
        EventManager.StopListening(StandardEventName.LoadSaveGame, LoadLampState);
        EventManager.StopListening(StandardEventName.DeleteSaveData, DeleteLampState);
    }
}
