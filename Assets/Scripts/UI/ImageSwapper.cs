using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageSwapper : MonoBehaviour
{
    [SerializeField] Sprite m_mouseControlsSprite;
    [SerializeField] Sprite m_joypadControlsSprite;

    private Image m_image;


    void Awake()
    {
        m_image = GetComponent<Image>();
    }


    private void SetMouseControlsSprite()
    {
        //print("Set mouse control sprites");
        if (m_mouseControlsSprite != null)
            m_image.sprite = m_mouseControlsSprite;
    }


    private void SetJoypadControlsSprite()
    {
        //print("Set joypad control sprites");
        if (m_joypadControlsSprite != null)
            m_image.sprite = m_joypadControlsSprite;
    }


    void OnEnable()
    {
        EventManager.StartListening(StandardEventName.ActivateMouseControls, SetMouseControlsSprite);
        EventManager.StartListening(StandardEventName.ActivateJoypadControls, SetJoypadControlsSprite);
    }


    void OnDisable()
    {
        EventManager.StopListening(StandardEventName.ActivateMouseControls, SetMouseControlsSprite);
        EventManager.StopListening(StandardEventName.ActivateJoypadControls, SetJoypadControlsSprite);
    }
}
