using UnityEngine;
using System.Collections;

public class SetUpCullingDistances : MonoBehaviour
{
    [SerializeField] float[] m_cullingDistances = new float[32];
    [SerializeField] bool m_sphericalCull = true;


	void Start() 
	{
		var camera = Camera.main;

		camera.layerCullDistances = m_cullingDistances;
		camera.layerCullSpherical = m_sphericalCull;
	}
}
