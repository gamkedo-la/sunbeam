using UnityEngine;
using System.Collections;

public class MirrorManager : MonoBehaviour
{
    [SerializeField] LightBeamManager m_lightBeam;
    [SerializeField] float m_rayDistanceToLightSource = 500f;

    private Transform m_lightSource;


    void Start()
    {
        m_lightSource = m_lightBeam.LightSource.transform;
    }


    void Update()
    {
        CastRayToLightSource();
    }


    private void CastRayToLightSource()
    {
        var direction = -m_lightSource.forward;

        RaycastHit hit;
        var ray = new Ray(transform.position, direction);

        if (Physics.Raycast(ray, out hit, m_rayDistanceToLightSource))
        {
            float distance = Vector3.Distance(transform.position, hit.point);

            Debug.DrawRay(transform.position, direction * distance, Color.red);
            m_lightBeam.Deactivate();
        }
        else
        {
            Debug.DrawRay(transform.position, direction * m_rayDistanceToLightSource, Color.green);
            m_lightBeam.Activate();
        }
    }
}
