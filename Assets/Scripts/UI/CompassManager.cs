using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CompassManager : MonoBehaviour
{
    private Transform m_planet;
    private Transform m_player;
    private Image m_compassImage;


    void Awake()
    {
        m_compassImage = GetComponent<Image>();
        m_player = GameObject.FindGameObjectWithTag(Tags.Player).transform;
        var planetObject = GameObject.FindGameObjectWithTag(Tags.Planet);

        if (planetObject != null)
            m_planet = planetObject.transform;   
    }


    void Start()
    {
        StartCoroutine(CheckForAxisInput("Toggle compass", ToggleCompass));
    }


    private void ToggleCompass()
    {
        m_compassImage.enabled = !m_compassImage.enabled;
    }


    void Update()
    {
        float theta = 0;

        if (m_planet == null)
        {
            theta = m_player.rotation.eulerAngles.y;
        }
        else
        {
            // http://www.movable-type.co.uk/scripts/latlong-vectors.html

            var n = m_planet.up;
            var a = m_player.position;
            var b = m_player.position + m_player.forward;

            var c1 = Vector3.Cross(a, b);
            var c2 = Vector3.Cross(a, n);
            var c1CrossC2 = Vector3.Cross(c1, c2);

            float sinTheta = c1CrossC2.magnitude * Mathf.Sign(Vector3.Dot(c1CrossC2, a));
            float cosTheta = Vector3.Dot(c1, c2);

            theta = -Mathf.Atan2(sinTheta, cosTheta) * Mathf.Rad2Deg;
        }

        m_compassImage.rectTransform.rotation = Quaternion.Euler(0, 0, theta);
	}



    private IEnumerator CheckForAxisInput(string axisName, Action action)
    {
        bool buttonPressedPreviously = false;

        while (true)
        {
            bool buttonPressed = Input.GetAxisRaw(axisName) == 1f;

            if (buttonPressed && !buttonPressedPreviously)
            {
                action.Invoke();
            }

            buttonPressedPreviously = buttonPressed;

            yield return null;
        }
    }
}
