using UnityEngine;
using System.Collections;

public class SetUpCullingDistances : MonoBehaviour
{
	void Start() 
	{
		var camera = Camera.main;
		float[] distances = new float[32];

		distances[10] = 30f;		// grass

		camera.layerCullDistances = distances;
		camera.layerCullSpherical = true;
	}
}
