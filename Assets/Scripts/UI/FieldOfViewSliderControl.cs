using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class FieldOfViewSliderControl : MonoBehaviour
{
    private Camera m_camera;
    private Slider m_slider;

	
    void Awake()
    {
        m_camera = Camera.main;
        m_slider = GetComponent<Slider>();

        float fov = PlayerPrefs.GetFloat(name, 60f);
        m_camera.fieldOfView = fov;
        m_slider.value = fov;
    }


    public void ChangeCameraFoV(float fov)
    {
        PlayerPrefs.SetFloat(name, fov);
        m_camera.fieldOfView = fov;
    }
}
