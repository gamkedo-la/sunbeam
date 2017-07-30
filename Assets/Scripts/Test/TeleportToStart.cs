using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToStart : MonoBehaviour
{
    [SerializeField] float m_jumpBackDistance = 50f;

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

            if (m_trail != null)
                m_trail.Clear();
        }
	}


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Respawn))
            transform.position -= transform.forward * m_jumpBackDistance;
    }
}
