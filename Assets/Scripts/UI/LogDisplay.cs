using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LogDisplay : MonoBehaviour
{
    private Text m_text;


    void Awake()
    {
        m_text = GetComponent<Text>();

        if (!GameController.AllowCheatMode)
            m_text.text = "";
    }
	

	void Update()
    {
        if (!GameController.AllowCheatMode)
            return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        m_text.text = string.Format("Mouse: [{0,5:0.00;-0.00}, {1,5:0.00;-0.00}]", mouseX, mouseY);
    }
}
