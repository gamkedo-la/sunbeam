using UnityEngine;
using System.Collections;

public class LightBeamManager : MonoBehaviour
{
    [SerializeField] Transform m_mirror;

    private Transform m_lightSource;
    private float m_distance;


    void Awake()
    {
        m_lightSource = GameObject.FindGameObjectWithTag(Tags.Sun).transform;
        m_distance = Vector3.Distance(m_mirror.position, transform.position);

        UpdateLightBeam();
    }


    void Update()
    {
        UpdateLightBeam();
    }


    private void UpdateLightBeam()
    {
        var directionToSource = m_lightSource.forward;
        var reflection = Vector3.Reflect(directionToSource, m_mirror.up).normalized;

        transform.position = m_mirror.position + m_distance * reflection;
        transform.rotation = Quaternion.LookRotation(reflection);
    }
}
