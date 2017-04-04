using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TurnOffForCinematic : MonoBehaviour
{
    private Image m_image;


    void Awake()
    {
        m_image = GetComponent<Image>();
    }


    private void TriggerClosingCinematic()
    {
        m_image.enabled = false;
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
