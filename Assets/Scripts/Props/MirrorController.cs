using UnityEngine;
using System.Collections;

public class MirrorController : MonoBehaviour, IActivatable
{
    [Header("Rotation")]
    [SerializeField] Transform m_rotationPoint;
    [SerializeField] float m_rotationSpeed = 20f;
    [SerializeField] Vector2 m_rotationMinMax = new Vector2(-30, 30);

    [Header("Pitch")]
    [SerializeField] Transform m_pitchPoint;
    [SerializeField] float m_pitchSpeed = 10f;
    [SerializeField] Vector2 m_pitchMinMax = new Vector2(20, 40);   

    [Header("Camera view point")]
    [SerializeField] Transform m_cameraPoint;

    [Header("Gizmos")]
    [SerializeField] float m_gizmoLineLength = 10f;

    private bool m_active;
    private bool m_canBeActivated;
    private float m_rotation;
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

        m_rotation = m_rotationPoint.localEulerAngles.y;

        if (m_rotation > 180f)
            m_rotation = 360f - m_rotation;

        ClampAngles();
    }


    void Update()
    {
        if (m_canBeActivated)
        {
            if (!m_activationTiggered && Input.GetAxisRaw("Submit") == 1)
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

        float h1 = Input.GetAxis("Horizontal");
        float v1 = Input.GetAxis("Vertical");
        float h2 = Input.GetAxis("Horizontal look");
        float v2 = Input.GetAxis("Vertical look");

        float h = Mathf.Clamp(h1 + h2, -1f, 1f);
        float v = Mathf.Clamp(v1 + v2, -1f, 1f);

        m_rotation += h * m_rotationSpeed * Time.deltaTime;
        m_pitch += v * m_pitchSpeed * Time.deltaTime;

        ClampAngles();
    }


    private void ClampAngles()
    {
        m_rotation = Mathf.Clamp(m_rotation, m_rotationMinMax.x, m_rotationMinMax.y);
        m_rotationPoint.localEulerAngles = Vector3.up * m_rotation;

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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        var rotationMin = Quaternion.Euler(Vector3.up * m_rotationMinMax.x) * transform.forward;
        var rotationMax = Quaternion.Euler(Vector3.up * m_rotationMinMax.y) * transform.forward;

        Gizmos.DrawRay(m_rotationPoint.position + m_rotationPoint.up * 0.5f, rotationMin * m_gizmoLineLength);
        Gizmos.DrawRay(m_rotationPoint.position + m_rotationPoint.up * 0.5f, rotationMax * m_gizmoLineLength);

        Gizmos.color = Color.cyan;

        var pitchMin = Quaternion.Euler(Vector3.right * m_pitchMinMax.x) * transform.forward;
        var pitchMax = Quaternion.Euler(Vector3.right * m_pitchMinMax.y) * transform.forward;

        Gizmos.DrawRay(m_pitchPoint.position, pitchMin * m_gizmoLineLength);
        Gizmos.DrawRay(m_pitchPoint.position, pitchMax * m_gizmoLineLength);
    }
}
