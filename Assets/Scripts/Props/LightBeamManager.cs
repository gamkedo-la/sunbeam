using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Projector))]
public class LightBeamManager : MonoBehaviour, IActivatable
{
    [SerializeField] Transform m_mirror;
    [SerializeField] LayerMask m_triggerMask;
    [SerializeField] Projector m_lightSource;
    [SerializeField] float m_projectorFarClipPlaneBuffer = 1.0f;
    [SerializeField] float m_rayDistanceToSun = 500f;
    [SerializeField] LayerMask m_blockingMask;
    [SerializeField] LayerMask m_projectorBlockingMask;
    [SerializeField] bool m_printBlocking;
    [SerializeField] bool m_printLightSourceBlocking;

    private Light m_sun;
    private bool m_lightSourceIsSun;
    private float m_distance;
    private Projector m_light;
    
    private Color m_lightColour;
    private float m_intensity;
    private bool m_active;
    private float m_range;
    private MeshRenderer m_volumetricLightRenderer;
    private float m_rayDistanceToLightSource;
    private SkyManager m_skyManager;
    private LightBeamManager m_lightSourceManager;


    void Awake()
    {
        if (m_lightSource == null)
        {
            m_sun = GameObject.FindGameObjectWithTag(Tags.Sky).GetComponentInChildren<Light>();
            m_skyManager = m_sun.GetComponent<SkyManager>();
            m_lightSourceIsSun = true;
        }

        m_lightSourceManager = m_lightSource != null ? m_lightSource.GetComponent<LightBeamManager>() : null;
        m_distance = Vector3.Distance(m_mirror.position, transform.position);
        m_light = GetComponent<Projector>();
        m_range = m_light.farClipPlane;
        m_volumetricLightRenderer = GetComponentInChildren<MeshRenderer>();

        var newMaterial = new Material(m_light.material);
        m_light.material = newMaterial;

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
            ? -m_sun.transform.forward
            : (m_lightSource.transform.position - transform.position).normalized;

        RaycastHit hit;
        var ray = new Ray(transform.position, direction);

        if (Physics.Raycast(ray, out hit, m_rayDistanceToLightSource, m_blockingMask))
        {
            if (m_printLightSourceBlocking)
                print("Light source blocked by " + hit.transform.name);

            float distance = Vector3.Distance(transform.position, hit.point);

            Debug.DrawRay(transform.position, direction * distance, Color.red);
            Deactivate();
        }
        else
        {
            if (IsWithinLightBeam(direction))
            {
                bool lightSourceEnabled = (m_lightSourceIsSun && m_sun.enabled) || m_lightSource.enabled;

                Debug.DrawRay(transform.position, direction * m_rayDistanceToLightSource, Color.green);
                if (lightSourceEnabled || (m_lightSourceManager != null && m_lightSourceManager.m_active))
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


    private bool IsWithinLightBeam(Vector3 direction)
    {
        if (m_lightSourceIsSun)
            return true;

        direction = -direction;

        float angleOfLightSource = Vector3.Angle(direction, m_lightSource.transform.forward);
        float distanceToLightSource = Vector3.Distance(transform.position, m_lightSource.transform.position);
        float distanceSideways = distanceToLightSource * angleOfLightSource * Mathf.Deg2Rad;

        //Debug.DrawRay(m_lightSource.transform.position, m_lightSource.transform.forward * distanceToLightSource, Color.white);
        //Debug.DrawRay(m_lightSource.transform.position, direction * distanceToLightSource, Color.black);

        // Make sure the light source is covering at least half of this light beam, accounting for cookie
        return distanceSideways <= 0.8f * m_lightSource.orthographicSize;
    }


    private void UpdateLightBeam()
    {
        var directionToSource = m_lightSourceIsSun 
            ? m_sun.transform.forward
            : transform.position - m_lightSource.transform.position;

        var reflection = Vector3.Reflect(directionToSource, m_mirror.up).normalized;

        transform.position = m_mirror.position + m_distance * reflection;
        transform.rotation = Quaternion.LookRotation(reflection);

        var projectOntoMirror = Vector3.ProjectOnPlane(transform.up, m_mirror.up);
 
        float angleForward = Vector3.Angle(projectOntoMirror, -m_mirror.forward);
        float angleRight = Vector3.Angle(projectOntoMirror, m_mirror.right);

        float angle = angleRight > 90f ? -angleForward : angleForward;

        transform.Rotate(0f, 0f, angle);

        float dot = Vector3.Dot(-directionToSource, m_mirror.up);

        dot = Mathf.Clamp01(dot);

        //if (!m_active)
        //    return;

        if (!m_lightSourceIsSun)
        {
            m_lightColour = m_lightSourceManager.LightColour;
            m_light.material.color = m_lightColour * dot;
            m_intensity = m_lightSourceManager.Intensity * dot;
        }
        else if (m_skyManager != null)
        {
            float evaluationValue = m_skyManager.GetEvaluationValue(transform);

            m_intensity = m_skyManager.GetSunIntensity(evaluationValue) * dot;

            m_lightColour = m_skyManager.GetSunColour(evaluationValue);
            m_light.material.color = m_lightColour * m_intensity;
        }
        else    // Only used in test scenes without a sky manager
        {
            m_intensity = m_sun.intensity * dot;
            m_lightColour = m_sun.color;
            m_light.material.color = m_sun.color * m_intensity;
        }
    }


    private void CastRayAlongBeam()
    {
        RaycastHit hitTrigger;
        RaycastHit hitBlock;
        var ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hitTrigger, m_range, m_triggerMask))
        {
            float distance = Vector3.Distance(transform.position, hitTrigger.point);
  
            if (Physics.Raycast(ray, out hitBlock, distance, m_blockingMask))
            {
                if (m_printBlocking)
                    print("Blocked by " + hitBlock.collider.name);

                //distance = Vector3.Distance(transform.position, hitBlock.point);
                //Debug.DrawRay(transform.position, transform.forward * distance, Color.cyan);
            }
            else
            {
                var solarPanelManager = hitTrigger.transform.GetComponentInParent<SolarPanelManager>();

                if (solarPanelManager != null)
                {
                    solarPanelManager.ChargeUp();
                    //Debug.DrawRay(transform.position, transform.forward * distance, Color.green);
                }
                else
                {
                    //Debug.DrawRay(transform.position, transform.forward * distance, Color.magenta);
                }
            }

            m_light.farClipPlane = distance + m_projectorFarClipPlaneBuffer;
        }
        else if (Physics.Raycast(ray, out hitBlock, m_range, m_blockingMask))
        {
            if (m_printBlocking)
                print("Blocked by " + hitBlock.collider.name);

            //float distance = Vector3.Distance(transform.position, hitBlock.point);
            //Debug.DrawRay(transform.position, transform.forward * distance, Color.cyan);
        }

        RaycastHit hitBlockProjector;
        if (Physics.Raycast(ray, out hitBlockProjector, m_range, m_projectorBlockingMask))
        {
            float distance = Vector3.Distance(transform.position, hitBlockProjector.point);
            m_light.farClipPlane = distance + m_projectorFarClipPlaneBuffer;
        }
        else
        {
            m_light.farClipPlane = m_range;
            //Debug.DrawRay(transform.position, transform.forward * m_range, Color.red);
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


    public Color LightColour
    {
        get { return m_lightColour; }
    }


    public float Intensity
    {
        get { return m_intensity; }
    }
}
