using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectButtonOnEnable : MonoBehaviour
{
    [SerializeField] Button m_buttonToSelectOnEnable;
	

    void OnEnable()
    {
        if (m_buttonToSelectOnEnable != null)
            m_buttonToSelectOnEnable.Select();
    }
}
