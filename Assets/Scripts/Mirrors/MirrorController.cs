using UnityEngine;
using System.Collections;

public class MirrorController : MonoBehaviour
{
    [SerializeField] float m_rotationSpeed = 10f;
    [SerializeField] Transform m_cameraPoint;
    [SerializeField] Transform m_rotationPoint;

    private bool m_active;
    private bool m_canBeActivated;


    void Awake()
    {
        var camera = GetComponentInChildren<Camera>();

        if (camera != null)
            camera.enabled = false;

        if (m_cameraPoint == null)
        {
            if (camera != null)
                m_cameraPoint = camera.transform;
            else
                m_cameraPoint = transform.GetChild(0);
        }

        if (m_rotationPoint == null)
            m_rotationPoint = transform.GetChild(1);
    }


	void Update()
    {
        if (m_canBeActivated)
        {
            if (!m_active && Input.GetAxisRaw("Action") == 1)
            {
                m_active = true;
                EventManager.TriggerEvent(TransformEventName.MirrorActivated, m_cameraPoint);
            }
            else if (m_active && Input.GetAxisRaw("Cancel") == 1)
            {
                m_active = false;
                EventManager.TriggerEvent(StandardEventName.MirrorDeactivated);
            }
        }
  
        if (!m_active)
            return;

        float h = Input.GetAxis("Horizontal");

        m_rotationPoint.Rotate(Vector3.up, h * m_rotationSpeed * Time.deltaTime, Space.Self);
    }


    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Tags.Player))
            return;

        m_canBeActivated = true;
    }


    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(Tags.Player))
            return;

        m_canBeActivated = false;
    }
}
