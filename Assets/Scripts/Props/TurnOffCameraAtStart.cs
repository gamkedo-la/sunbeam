using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TurnOffCameraAtStart : MonoBehaviour
{
    private Camera m_camera;

	
	void Start()
    {
        m_camera = GetComponent<Camera>();
        m_camera.enabled = false;
	}
}
