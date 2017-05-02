using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShowIfCheatModeActive : MonoBehaviour
{
    private Button m_button;
    private Image m_buttonImage;
    private Text m_buttonText;

    void Awake()
    {
        m_button = GetComponent<Button>();
        m_buttonImage = GetComponent<Image>();
        m_buttonText = GetComponentInChildren<Text>();
    }

	
    void OnEnable()
    {
        m_button.enabled = GameController.AllowCheatMode;

        if (m_buttonImage != null)
            m_buttonImage.enabled = GameController.AllowCheatMode;

        if (m_buttonText != null)
            m_buttonText.enabled = GameController.AllowCheatMode;
    }
}
