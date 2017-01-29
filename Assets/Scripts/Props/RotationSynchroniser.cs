using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSynchroniser : MonoBehaviour
{
    [SerializeField] Transform m_transformToSyncWith;
    [SerializeField] float m_rotationMultiplier = 1f;

    private float m_startRotation;
    private float m_startSyncRotation;
    private float m_previousSyncRotation;


    void Awake()
    {
        m_startRotation = transform.rotation.y;
        m_startSyncRotation = m_transformToSyncWith != null ? m_transformToSyncWith.rotation.y : 0f;
        m_previousSyncRotation = m_startSyncRotation;
    }


	void Update()
    {
        if (m_transformToSyncWith == null)
            return;

        float syncRotation = m_transformToSyncWith.rotation.eulerAngles.y;
        float rotateAmount = m_rotationMultiplier * (syncRotation - m_previousSyncRotation);

        transform.Rotate(0f, rotateAmount, 0f);

        m_previousSyncRotation = syncRotation;
    }
}
