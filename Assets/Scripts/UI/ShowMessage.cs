using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowMessage : MonoBehaviour
{
    [SerializeField] Text m_text; 
	

    public void SetMessageText(string message)
    {
        if (m_text != null)
            m_text.text = message;
    }
}
