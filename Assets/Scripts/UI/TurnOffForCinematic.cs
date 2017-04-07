using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TurnOffForCinematic : MonoBehaviour
{
    [SerializeField] bool m_turnOnForContinueExploring;

    private Image m_image;


    void Awake()
    {
        m_image = GetComponent<Image>();
    }


    private void TriggerClosingCinematic()
    {
        m_image.enabled = false;
    }


    private void ContinueExploring()
    {
        if (m_turnOnForContinueExploring)
            m_image.enabled = true;
    }


    void OnEnable()
    {
        EventManager.StartListening(StandardEventName.TriggerClosingCinematic, TriggerClosingCinematic);
        EventManager.StartListening(StandardEventName.ContinueExploring, ContinueExploring);
    }


    void OnDisable()
    {
        EventManager.StopListening(StandardEventName.TriggerClosingCinematic, TriggerClosingCinematic);
        EventManager.StopListening(StandardEventName.ContinueExploring, ContinueExploring);
    }
}
