using UnityEngine;
using System.Collections;

public class StarsManager : MonoBehaviour 
{
	[SerializeField] Transform m_cameraTransform;
    [SerializeField] AnimationCurve m_sunIntensityToAlpha;

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
	}


	void Update() 
	{
        float evaluationValue = m_skyManager.GetEvaluationValue(m_cameraTransform);
        float sunIntensity = m_skyManager.GetSunIntensity(evaluationValue);

        float alpha = m_sunIntensityToAlpha.Evaluate(sunIntensity);

        m_colour.a = alpha;

		m_renderer.material.SetColor("_TintColor", m_colour);

		m_position = m_cameraTransform.position;

		transform.position = m_position;
	}
}
