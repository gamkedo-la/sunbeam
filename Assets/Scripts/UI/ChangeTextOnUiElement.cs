using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ChangeTextOnUiElement : MonoBehaviour
{
    private Text m_text;

    void Awake()
    {
        m_text = GetComponent<Text>();
    }


    public void ChangeText(string newText)
    {
        m_text.text = newText;
    }
}
