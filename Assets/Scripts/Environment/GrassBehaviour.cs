using UnityEngine;

public class GrassBehaviour : MonoBehaviour
{
	Renderer rend;
	Material mat;
	float wind;

	void Start()
	{
		rend = GetComponent<Renderer>();
		mat = rend.material;
	}

	void OnBecameVisible()
	{
		UpdateShader();
	}

	void Update()
	{
		if(rend.isVisible)
		{
			UpdateShader();
		}
	}

	void UpdateShader()
	{
		wind = WindManager.grassPlane.windIntensity(transform.position);
		//Vector2 windDirection = WindManager.grassPlane.windDirection3D;
		mat.SetColor("_dir", new Vector4(wind - 0.5f, 0, 0, 0));
	}
}
