using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class LightBeamManager : MonoBehaviour, IActivatable
{
    [SerializeField] Transform m_mirror;
    [SerializeField] LayerMask m_triggerMask;

    private Light m_lightSource;
    private float m_distance;
    private Light m_light;
    private bool m_active;
    private float m_range;
    private MeshRenderer m_volumetricLightRenderer;
    private float m_volumetricLightAlpha;


    void Awake()
    {
        m_lightSource = GameObject.FindGameObjectWithTag(Tags.Sun).GetComponent<Light>();
        m_distance = Vector3.Distance(m_mirror.position, transform.position);
        m_light = GetComponent<Light>();
        m_range = m_light.range;
        m_volumetricLightRenderer = GetComponentInChildren<MeshRenderer>();

        if (m_volumetricLightRenderer != null)
            m_volumetricLightAlpha = m_volumetricLightRenderer.material.GetColor("_TintColor").a;

        m_active = true;

        UpdateLightBeam();
    }


    void Update()
    {
        if (!m_active)
            return;

        UpdateLightBeam();
        CastRayAlongBeam();
    }


    private void UpdateLightBeam()
    {
        var directionToSource = m_lightSource.transform.forward;
        var reflection = Vector3.Reflect(directionToSource, m_mirror.up).normalized;

        transform.position = m_mirror.position + m_distance * reflection;
        transform.rotation = Quaternion.LookRotation(reflection);

        float dot = Vector3.Dot(-directionToSource, m_mirror.up);

        dot = Mathf.Clamp01(dot);

        m_light.color = m_lightSource.color;
        m_light.intensity = m_lightSource.intensity * dot;
        
        if (m_volumetricLightRenderer != null)
        {
            var colour = m_lightSource.color;
            colour.a = m_volumetricLightAlpha;
            m_volumetricLightRenderer.material.SetColor("_TintColor", colour);
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
        m_active = true;
        m_light.enabled = true;

        if (m_volumetricLightRenderer != null)
            m_volumetricLightRenderer.enabled = true;
    }


    public void Deactivate()
    {
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
