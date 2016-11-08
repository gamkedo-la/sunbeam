using UnityEngine;
using System.Collections;

public class MirrorController : MonoBehaviour, IActivatable
{
    [SerializeField] float m_rotationSpeed = 20f;
    [SerializeField] float m_pitchSpeed = 10f;
    [SerializeField] Vector2 m_pitchMinMax = new Vector2(20, 40);
    [SerializeField] Transform m_cameraPoint;
    [SerializeField] Transform m_rotationPoint;
    [SerializeField] Transform m_pitchPoint;

    private bool m_active;
    private bool m_canBeActivated;
    private float m_pitch;
    private bool m_activationTiggered;


    void Awake()
    {
        var camera = GetComponentInChildren<Camera>();

        if (camera != null)
            camera.enabled = false;

        if (m_cameraPoint == null)
        {
            if (camera != null)
                m_cameraPoint = camera.transform;
        }

        m_pitch = m_pitchPoint.localEulerAngles.x;

        if (m_pitch > 180f)
            m_pitch = 360f - m_pitch;
    }


    void Update()
    {
        if (m_canBeActivated)
        {
            if (!m_activationTiggered && Input.GetAxisRaw("Action") == 1)
            {
                m_activationTiggered = true;
                EventManager.TriggerEvent(TransformEventName.MirrorActivated, m_cameraPoint);
            }
            else if (m_activationTiggered && Input.GetAxisRaw("Cancel") == 1)
            {
                m_active = false;
                m_activationTiggered = false;
                EventManager.TriggerEvent(StandardEventName.MirrorDeactivated);
            }
        }
  
        if (!m_active)
            return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        m_rotationPoint.Rotate(Vector3.up, h * m_rotationSpeed * Time.deltaTime, Space.Self);

        m_pitch -= v * m_pitchSpeed * Time.deltaTime;
        m_pitch = Mathf.Clamp(m_pitch, m_pitchMinMax.x, m_pitchMinMax.y);

        m_pitchPoint.localEulerAngles = Vector3.left * m_pitch;
    }


    public void Activate()
    {
        m_active = true;
    }


    public void Deactivate()
    {
        m_active = false;
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
