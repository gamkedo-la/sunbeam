using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class TriggerCinematic : MonoBehaviour
{
    [SerializeField] float m_delay = 3f;
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
            TriggerClosingCinematicDelayed();
    }


    public void TriggerClosingCinematicDelayed()
    {
        StartCoroutine(TriggerAfterDelay(m_delay));
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


    private IEnumerator TriggerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        EventManager.TriggerEvent(StandardEventName.TriggerClosingCinematic);
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
