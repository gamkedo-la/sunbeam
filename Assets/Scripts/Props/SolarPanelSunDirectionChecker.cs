using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarPanelSunDirectionChecker : MonoBehaviour
{
    [SerializeField] float m_sunDotThreshold = 0.7f;
    [SerializeField] float m_sunRayDistance = 500f;

    private SolarPanelManager m_solarPanelManager;
    private Transform m_sun;


	
    void Awake()
    {
        m_solarPanelManager = GetComponentInParent<SolarPanelManager>();
        var sky = GameObject.FindGameObjectWithTag(Tags.Sky);

        if (sky != null)
            m_sun = sky.GetComponent<Light>().transform;
    }


    void Update()
    {
        var direction = -m_sun.forward;
        float dot = Vector3.Dot(direction, transform.forward);

        if (dot < m_sunDotThreshold)
            return;

        RaycastHit hit;
        var ray = new Ray(transform.position, direction);

        if (Physics.Raycast(ray, out hit, m_sunRayDistance))
        {
            float distance = Vector3.Distance(transform.position, hit.point);

            Debug.DrawRay(transform.position, direction * distance, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, direction * m_sunRayDistance, Color.green);
            m_solarPanelManager.ChargeUp();
        }  
    }
}
