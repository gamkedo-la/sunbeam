using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class TurnOnLamp : MonoBehaviour
{
    private Light m_light;
	

    void Awake()
    {
        m_light = GetComponent<Light>();
    }


    public void TurnOnLampLight(float delay)
    {
        StartCoroutine(TurnOnLampAfterDelay(delay));
    }


    private IEnumerator TurnOnLampAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        m_light.enabled = true;
    }
}
