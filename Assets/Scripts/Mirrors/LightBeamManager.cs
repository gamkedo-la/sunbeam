using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class LightBeamManager : MonoBehaviour
{
    [SerializeField] Transform m_mirror;

    private Light m_lightSource;
    private float m_distance;
    private Light m_light;    


    void Awake()
    {
        m_lightSource = GameObject.FindGameObjectWithTag(Tags.Sun).GetComponent<Light>();
        m_distance = Vector3.Distance(m_mirror.position, transform.position);
        m_light = GetComponent<Light>();

        UpdateLightBeam();
    }


    void Update()
    {
        UpdateLightBeam();
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
    }
}
