using UnityEngine;
using System.Collections;

public class LightBeamManager : MonoBehaviour
{
    [SerializeField] Transform m_mirror;

    private Transform m_sun;
    private float m_distance;


    void Awake()
    {
        m_sun = GameObject.FindGameObjectWithTag(Tags.Sun).transform;
        m_distance = Vector3.Distance(m_mirror.position, transform.position);
    }

	
	void Update()
    {
        var directionToSun = m_sun.forward;
        var reflection = Vector3.Reflect(-directionToSun, m_mirror.forward).normalized;

        transform.position = m_mirror.position + m_distance * reflection;
        transform.rotation = Quaternion.LookRotation(reflection);
	}
}
