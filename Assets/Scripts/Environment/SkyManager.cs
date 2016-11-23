﻿using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Light))]
public class SkyManager : MonoBehaviour
{
    public float SunAngleAboveHorizon;

    [SerializeField] Gradient m_skyColour;
    [SerializeField] Gradient m_ambientColour;
    [SerializeField] Gradient m_sunColour;
    [SerializeField] AnimationCurve m_sunIntensity;

    private Transform m_player;
    private Transform m_sunImage;
    private Light m_sunLight;
    private Material m_skybox;
    private Material m_sunMaterial;
    private Color m_originalSkyboxColour;
    private Color m_originalSunColour;
    private float m_sunDistanceFromPlayer;


	void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag(Tags.Player).transform;
        m_sunLight = GetComponent<Light>();

        var sunMesh = GetComponentInChildren<MeshRenderer>();
        m_sunImage = sunMesh.transform;
        m_sunMaterial = sunMesh.material;

        m_skybox = RenderSettings.skybox;

        m_originalSkyboxColour = m_skybox.GetColor("_Tint");
        m_originalSunColour = m_sunMaterial.GetColor("_TintColor");

        m_sunDistanceFromPlayer = Vector3.Distance(m_player.position, m_sunImage.position);

        var playerDirection = m_player.position.normalized;
        SunAngleAboveHorizon = Vector3.Angle(transform.forward, playerDirection) - 90f;
    }
	

	void Update()
    {
        var playerDirection = m_player.position.normalized;
        float dotToPlayer = Vector3.Dot(-transform.forward, playerDirection);
        SunAngleAboveHorizon = Vector3.Angle(transform.forward, playerDirection) - 90f;

        float evaluationValue = 0.5f * (dotToPlayer + 1f);

        var skyColour = m_skyColour.Evaluate(evaluationValue);
        m_skybox.SetColor("_Tint", skyColour);

        RenderSettings.ambientLight = m_ambientColour.Evaluate(evaluationValue);
        m_sunLight.intensity = m_sunIntensity.Evaluate(evaluationValue);
        var sunColour = m_sunColour.Evaluate(evaluationValue);
        m_sunLight.color = sunColour;
        m_sunMaterial.SetColor("_TintColor", sunColour);

        m_sunImage.position = m_player.position - transform.forward * m_sunDistanceFromPlayer;
	}


    public float GetEvaluationValue(Transform observer)
    {
        var observerDirection = observer.position.normalized;
        float dotToObserver = Vector3.Dot(-transform.forward, observerDirection);
        float evaluationValue = 0.5f * (dotToObserver + 1f);

        return evaluationValue;
    }


    public Color GetSunColour(float evaluationValue)
    {
        var colour = m_sunColour.Evaluate(evaluationValue);

        return colour;
    }


    public float GetSunIntensity(float evaluationValue)
    {
        float intensity = m_sunIntensity.Evaluate(evaluationValue);

        return intensity;
    }


    void OnDestroy()
    {
        m_skybox.SetColor("_Tint", m_originalSkyboxColour);
        m_sunMaterial.SetColor("_TintColor", m_originalSunColour);
    }
}
