using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour, IActivatable
{
    [SerializeField] bool m_startActive;
    [SerializeField] float m_rotationRate = 10f;
    [SerializeField] float m_accelerationRate = 1f;


    private float m_currentRotationRate;
    private bool m_active;


    void Awake()
    {
        if (m_startActive)
        {
            m_active = true;
            m_currentRotationRate = m_rotationRate;
        }
    }


	void Update()
    {
        if (!m_active)
            return;

        transform.Rotate(Vector3.up, Time.deltaTime * m_currentRotationRate);
	}


    public void Activate()
    {
        StopAllCoroutines();
        StartCoroutine(Accelerate());
    }


    public void Deactivate()
    {
        StopAllCoroutines();
        StartCoroutine(Decelerate());
    }


    private IEnumerator Accelerate()
    {
        m_active = true;

        while (m_currentRotationRate < m_rotationRate)
        {
            m_currentRotationRate += Time.deltaTime * m_accelerationRate;
            yield return null;
        }

        m_currentRotationRate = m_rotationRate;
    }


    private IEnumerator Decelerate()
    {
        m_active = true;

        while (m_currentRotationRate > 0)
        {
            m_currentRotationRate -= Time.deltaTime * m_accelerationRate;
            yield return null;
        }

        m_currentRotationRate = 0;
        m_active = false;
    }
}
