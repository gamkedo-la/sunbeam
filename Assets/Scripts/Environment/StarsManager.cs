using UnityEngine;
using System.Collections;

public class StarsManager : MonoBehaviour 
{
	[SerializeField] float m_degreesAboveHorizonForMinAlpha = 5f;
	[SerializeField] float m_degreesAboveHorizonForMaxAlpha = -15f;
	[SerializeField] Transform m_cameraTransform;
    //[SerializeField] Transform m_sunTransform;


    private SkyManager m_skyManager;
    private ParticleSystem m_stars;
	private ParticleSystemRenderer m_renderer;
	private Color m_colour;
	private Vector3 m_position;


	void Awake()
	{
        m_skyManager = FindObjectOfType<SkyManager>();
        m_stars = GetComponent<ParticleSystem>();

		m_renderer = (ParticleSystemRenderer) m_stars.GetComponent<Renderer>();
		m_colour = m_renderer.material.GetColor("_TintColor");

		if (m_cameraTransform == null)
			m_cameraTransform = Camera.main.transform;

		//if (m_sunTransform == null)
		//	m_sunTransform = GameObject.Find("Directional Light").transform;
	}


	void Update() 
	{
		float degreesAboveHorizon = m_skyManager.SunAngleAboveHorizon;

		float alpha = 1f;

        if (degreesAboveHorizon > m_degreesAboveHorizonForMinAlpha)
            alpha = 0f;
        else if (degreesAboveHorizon < m_degreesAboveHorizonForMaxAlpha)
            alpha = 1f;
        else
        {
            float x = (degreesAboveHorizon - m_degreesAboveHorizonForMaxAlpha) /
                (m_degreesAboveHorizonForMinAlpha - m_degreesAboveHorizonForMaxAlpha);

            alpha = 1 - x;
        }

        //print ("Degrees: " + degreesAboveHorizon + " alpha: " + alpha);

        m_colour.a = alpha;

		m_renderer.material.SetColor("_TintColor", m_colour);

		//transform.rotation = m_sunTransform.rotation;

		m_position = m_cameraTransform.position;
		//m_position.y = 0;

		transform.position = m_position;
	}
}
