using UnityEngine;

public class WindPlane : MonoBehaviour {
	public float randomRange, windScale;

	public Vector2 windDirection2D;

	public Vector3 normal;

	public Texture2D windTex;

	private Vector2 playerPlanePosition;
	private Vector2 windPosition;

	private Transform playerTransform;

	void Start()
	{
		//If no wind texture is assigned crash the game (this saves a lot of headache if the texture is not present)
		if(windTex == null)
		{
			Debug.Log("No wind texture set to wind plane");
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}

		playerPlanePosition = new Vector2();
		windPosition = new Vector2(Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange));

		playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		WindManager.grassPlane = this;

		transform.rotation = Quaternion.LookRotation(playerTransform.position);
	}

	public float windIntensity(Vector3 pos)
	{
		float x = Vector3.Project(pos, transform.right).magnitude;
		float y = Vector3.Project(pos, transform.up).magnitude;

		x += playerPlanePosition.x + windPosition.x;
		y += playerPlanePosition.y + windPosition.y;

		return windTex.GetPixelBilinear(x * windScale, y * windScale).r;
	}

	void Update()
	{
		playerPlanePosition.x += Vector3.Project(playerTransform.position, transform.right).magnitude;
		playerPlanePosition.y += Vector3.Project(playerTransform.position, transform.up).magnitude;

		transform.rotation = Quaternion.LookRotation(playerTransform.position);

		windPosition += windDirection2D * Time.deltaTime;

		normal = playerTransform.position.normalized;
	}
}
