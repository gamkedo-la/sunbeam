using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldOfViewSliderControl : MonoBehaviour
{
    private Camera m_camera;

	
    void Awake()
    {
        m_camera = Camera.main;
    }


    public void ChangeCameraFoV(float fov)
    {
        m_camera.fieldOfView = fov;
    }
}
