using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSynchroniser : MonoBehaviour
{
    public enum Axis
    {
        X,
        Y,
        Z
    }

    [SerializeField] Transform m_transformToSyncWith;
    [SerializeField] float m_rotationMultiplier = 1f;
    [SerializeField] Axis m_axisToSynchWith = Axis.Y;

    private float m_startSyncRotation;
    private float m_previousSyncRotation;
    private float m_difference;


    void Awake()
    {
        m_startSyncRotation = m_transformToSyncWith != null ? GetAngle() : 0f;
        m_previousSyncRotation = m_startSyncRotation;
    }


    private float GetAngle()
    {
        float angle = 0;

        switch (m_axisToSynchWith)
        {
            case (Axis.X):
                angle = m_transformToSyncWith.localEulerAngles.x;
                break;

            case (Axis.Y):
                angle = m_transformToSyncWith.localEulerAngles.y;
                break;

            case (Axis.Z):
                angle = m_transformToSyncWith.localEulerAngles.z;
                break;
        }

        return angle;
    }


	void Update()
    {
        if (m_transformToSyncWith == null)
            return;

        float syncRotation = GetAngle();
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
