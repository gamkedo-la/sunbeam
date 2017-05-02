using UnityEngine;

public class GrassBehaviour : MonoBehaviour
{
	Renderer rend;
	Material mat;
	float wind;
	Vector4 windDirection = new Vector4(0, 0, 0, 0);

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
		windDirection.x = wind - 0.5f;
		mat.SetColor("_dir", windDirection);
	}
}
