using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class TurnOffCanvasAtStart : MonoBehaviour
{

    private Canvas m_canvas;


    void Start()
    {
        m_canvas = GetComponent<Canvas>();
        m_canvas.enabled = false;
    }
}
