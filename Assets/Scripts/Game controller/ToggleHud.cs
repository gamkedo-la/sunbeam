using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class ToggleHud : MonoBehaviour
{
    private Canvas m_hud;


    void Awake()
    {
        m_hud = GetComponent<Canvas>();
    }


	void Update()
    {
        if (!GameController.AllowCheatMode)
            return;

        if (Input.GetKeyDown(KeyCode.H))
            m_hud.enabled = !m_hud.enabled;
	}
}
