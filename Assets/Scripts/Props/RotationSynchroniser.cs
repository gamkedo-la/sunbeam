using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSynchroniser : MonoBehaviour
{
    [SerializeField] Transform m_transformToSyncWith;
    [SerializeField] float m_rotationMultiplier = 1f;

    private float m_startSyncRotation;
    private float m_previousSyncRotation;
    private float m_difference;


    void Awake()
    {
        m_startSyncRotation = m_transformToSyncWith != null ? m_transformToSyncWith.localEulerAngles.y : 0f;
        m_previousSyncRotation = m_startSyncRotation;
    }


	void Update()
    {
        if (m_transformToSyncWith == null)
            return;

        float syncRotation = m_transformToSyncWith.localEulerAngles.y;
        float difference = syncRotation - m_previousSyncRotation;

        if (difference > 180f)
            difference = (difference - 360f) % 360f;
        else if (difference < -180f)
            difference = (difference + 360f) % 360f;

        //print(difference);
        float rotateAmount = m_rotationMultiplier * difference;
        
        transform.Rotate(0f, rotateAmount, 0f);

        m_previousSyncRotation = syncRotation;
    }
}
