using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CheckForSavedGame : MonoBehaviour
{
    private Button m_button;

	
    void Awake()
    {
        m_button = GetComponent<Button>();
        string checkPoint = PlayerPrefs.GetString("Checkpoint", "None");

        m_button.interactable = checkPoint != "None";
    }
}
