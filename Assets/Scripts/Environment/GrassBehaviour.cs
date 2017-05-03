using UnityEngine;

public class GrassBehaviour : MonoBehaviour
{
	Renderer rend;
	Material mat;
	float wind;
	Vector4 windDirection = new Vector4(0, 0, 0, 0);

    private bool updateShader = false;
         

	void Start()
	{
		rend = GetComponent<Renderer>();
		mat = rend.material;
	}


	void OnBecameVisible()
	{
        updateShader = true;
		UpdateShader();
	}


    void OnBecomeInvisible()
    {
        updateShader = false;
    }


	public void UpdateMe()
	{
		if(updateShader)
		{
			UpdateShader();
		}
	}


	void UpdateShader()
	{
		wind = WindManager.grassPlane.windIntensity(transform.position);
		//Vector2 windDirection = WindManager.grassPlane.windDirection3D;
		windDirection.x = wind;
		mat.SetColor("_dir", windDirection);
	}
}
