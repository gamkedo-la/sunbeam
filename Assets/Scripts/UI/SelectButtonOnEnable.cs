using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectButtonOnEnable : MonoBehaviour
{
    [SerializeField] Button m_buttonToSelectOnEnable;
	

    void OnEnable()
    {
        if (m_buttonToSelectOnEnable != null && GameController.UseJoystickLook)
        {
            m_buttonToSelectOnEnable.Select();
            StartCoroutine(SetSelectButtonLater(m_buttonToSelectOnEnable));
        }
    }


    IEnumerator SetSelectButtonLater(Button selectedButton)
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(selectedButton.gameObject);
    }
}
