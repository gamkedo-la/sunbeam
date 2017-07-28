using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToStart : MonoBehaviour
{
    private Vector3 m_startPosition;
    private TrailRenderer m_trail;

	
    void Awake()
    {
        m_startPosition = transform.position;
        m_trail = GetComponentInChildren<TrailRenderer>();
    }
	

	void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.position = m_startPosition;
            m_trail.Clear();
        }
	}
}
