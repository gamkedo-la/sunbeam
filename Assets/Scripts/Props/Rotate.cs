using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] float m_rotationRate = 10f;
	
	void Update()
    {
        transform.Rotate(Vector3.forward, Time.deltaTime * m_rotationRate);
	}
}
