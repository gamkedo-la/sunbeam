using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class LightBeamManager : MonoBehaviour, IActivatable
{
    [SerializeField] Transform m_mirror;
    [SerializeField] LayerMask m_triggerMask;
    [SerializeField] Light m_lightSource;
    [SerializeField] float m_rayDistanceToSun = 500f;
    [SerializeField] LayerMask m_blockingMask;

    private bool m_lightSourceIsSun;
    private float m_distance;
    private Light m_light;
    private bool m_active;
    private float m_range;
    private MeshRenderer m_volumetricLightRenderer;
    private float m_rayDistanceToLightSource;
    private SkyManager m_skyManager;


    void Awake()
    {
        if (m_lightSource == null)
        {
            m_lightSource = GameObject.FindGameObjectWithTag(Tags.Sky).GetComponentInChildren<Light>();
            m_skyManager = m_lightSource.GetComponent<SkyManager>();
            m_lightSourceIsSun = true;
        }

        m_distance = Vector3.Distance(m_mirror.position, transform.position);
        m_light = GetComponent<Light>();
        m_range = m_light.range;
        m_volumetricLightRenderer = GetComponentInChildren<MeshRenderer>();

        m_rayDistanceToLightSource = m_lightSourceIsSun
            ? m_rayDistanceToSun
            : Vector3.Distance(m_lightSource.transform.position, transform.position) - 0.2f;
            
        m_active = true;

        UpdateLightBeam();
    }


    void Update()
    {
        CastRayToLightSource();

        if (!m_active)
            return;

        UpdateLightBeam();
        CastRayAlongBeam();     
    }


    private void CastRayToLightSource()
    {
        var direction = m_lightSourceIsSun
            ? -m_lightSource.transform.forward
            : (m_lightSource.transform.position - transform.position).normalized;

        RaycastHit hit;
        var ray = new Ray(transform.position, direction);

        if (Physics.Raycast(ray, out hit, m_rayDistanceToLightSource, m_blockingMask))
        {
            //print("Blocked by " + hit.transform.name);
            float distance = Vector3.Distance(transform.position, hit.point);

            Debug.DrawRay(transform.position, direction * distance, Color.red);
            Deactivate();
        }
        else
        {
            if (IsWithinLightCone(direction))
            {
                Debug.DrawRay(transform.position, direction * m_rayDistanceToLightSource, Color.green);
                if (m_lightSource.enabled)
                    Activate();
                else
                    Deactivate();
            }
            else
            {
                Debug.DrawRay(transform.position, direction * m_rayDistanceToLightSource, Color.yellow);
                Deactivate();
            }
        }
    }


    private bool IsWithinLightCone(Vector3 direction)
    {
        if (m_lightSourceIsSun)
            return true;

        direction = -direction;
        float angleOfLightSource = Vector3.Angle(direction, m_lightSource.transform.forward);

        // Make sure the light source is covering at least half of this light beam
        return angleOfLightSource * 4f <= m_lightSource.spotAngle;
    }


    private void UpdateLightBeam()
    {
        var directionToSource = m_lightSourceIsSun 
            ? m_lightSource.transform.forward
            : transform.position - m_lightSource.transform.position;

        var reflection = Vector3.Reflect(directionToSource, m_mirror.up).normalized;

        transform.position = m_mirror.position + m_distance * reflection;
        transform.rotation = Quaternion.LookRotation(reflection);
        var localRotation = transform.localEulerAngles;
        localRotation.z = 0;
        transform.localEulerAngles = localRotation;

        float dot = Vector3.Dot(-directionToSource, m_mirror.up);

        dot = Mathf.Clamp01(dot);

        if (m_skyManager == null)
        {
            m_light.color = m_lightSource.color;
            m_light.intensity = m_lightSource.intensity * dot;
        }
        else
        {
            float evaluationValue = m_skyManager.GetEvaluationValue(transform);
            m_light.color = m_skyManager.GetSunColour(evaluationValue);
            m_light.intensity = m_skyManager.GetSunIntensity(evaluationValue) * dot;
        }
    }


    private void CastRayAlongBeam()
    {
        RaycastHit hit;
        var ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit, m_range, m_triggerMask))
        {
            float distance = Vector3.Distance(transform.position, hit.point);
            var solarPanelManager = hit.transform.GetComponentInParent<SolarPanelManager>();

            if (solarPanelManager != null)
            {
                solarPanelManager.ChargeUp();
                Debug.DrawRay(transform.position, transform.forward * distance, Color.green);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.forward * distance, Color.magenta);
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * m_range, Color.red);
        }
    }


    public void Activate()
    {
        if (m_active)
            return;

        m_active = true;
        m_light.enabled = true;

        if (m_volumetricLightRenderer != null)
            m_volumetricLightRenderer.enabled = true;
    }


    public void Deactivate()
    {
        if (!m_active)
            return;

        m_active = false;
        m_light.enabled = false;

        if (m_volumetricLightRenderer != null)
            m_volumetricLightRenderer.enabled = false;
    }


    public Light LightSource
    {
        get { return m_lightSource; }
    }
}
