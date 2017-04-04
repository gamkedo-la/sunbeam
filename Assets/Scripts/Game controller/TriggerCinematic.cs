using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class TriggerCinematic : MonoBehaviour
{
    [SerializeField] Transform m_cameraAnchor;
    [SerializeField] UnityEvent m_eventsToTrigger;

    private Transform m_camera;


    void Awake()
    {
        m_camera = Camera.main.transform;
    }


    void Update()
    {
        if (GameController.AllowCheatMode && Input.GetKeyDown(KeyCode.G))
            EventManager.TriggerEvent(StandardEventName.TriggerClosingCinematic);
    }


    private void TriggerClosingCinematic()
    {
        print("Closing cinematic triggered");
        if (m_cameraAnchor != null)
        {
            m_camera.parent = m_cameraAnchor;
            m_camera.localPosition = Vector3.zero;
            m_camera.localRotation = Quaternion.identity;
        }

        m_eventsToTrigger.Invoke();
    }


    void OnEnable()
    {
        EventManager.StartListening(StandardEventName.TriggerClosingCinematic, TriggerClosingCinematic);
    }


    void OnDisable()
    {
        EventManager.StopListening(StandardEventName.TriggerClosingCinematic, TriggerClosingCinematic);
    }
}
