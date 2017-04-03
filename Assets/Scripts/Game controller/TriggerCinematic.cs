using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class TriggerCinematic : MonoBehaviour
{
    [SerializeField] UnityEvent m_eventsToTrigger;


    void Update()
    {
        if (GameController.AllowCheatMode && Input.GetKeyDown(KeyCode.G))
            TriggerClosingCinematic();

    }


    private void TriggerClosingCinematic()
    {
        m_eventsToTrigger.Invoke();
    }


    void OnEnable()
    {
        EventManager.StartListening(StandardEventName.Pause, TriggerClosingCinematic);
    }


    void OnDisable()
    {
        EventManager.StopListening(StandardEventName.Pause, TriggerClosingCinematic);
    }
}
