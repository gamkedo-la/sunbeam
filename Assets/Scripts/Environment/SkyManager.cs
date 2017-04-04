using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Light), typeof(LensFlare))]
public class SkyManager : MonoBehaviour
{
    [SerializeField] Gradient m_skyColour;
    [SerializeField] Gradient m_ambientColour;
    [SerializeField] Gradient m_sunColour;
    [SerializeField] Gradient m_sunFlairColour;
    [SerializeField] AnimationCurve m_sunIntensity;
    [SerializeField] AnimationCurve m_playerAltitudeAdjustment;
    [SerializeField] Vector2 m_playerAltitudeMinMax = new Vector2(150f, 300f);
    [SerializeField] float m_transitionToSpaceAltitude = 50f;

    private Transform m_camera;
    private Transform m_sunImage;
    private Light m_sunLight;
    private LensFlare m_flair;
    private Material m_skybox;
    private Material m_sunMaterial;
    private Color m_originalSkyboxColour;
    private Color m_originalSunColour;
    private float m_sunDistanceFromPlayer;
    private float m_playerAltitude;
    private float m_evaluationValue;
    private float m_skyColourEvaluationValue;
    private float m_sunEvaluationValue;
    private float m_spaceFraction;


	void Awake()
    {
        m_camera = Camera.main.transform;
        m_sunLight = GetComponent<Light>();
        m_flair = GetComponent<LensFlare>();

        var sunMesh = GetComponentInChildren<MeshRenderer>();
        m_sunImage = sunMesh.transform;
        m_sunMaterial = sunMesh.material;

        m_skybox = RenderSettings.skybox;

        m_originalSkyboxColour = m_skybox.GetColor("_Tint");
        m_originalSunColour = m_sunMaterial.GetColor("_TintColor");

        m_sunDistanceFromPlayer = Vector3.Distance(m_camera.position, m_sunImage.position);
    }


    void Update()
    {
        m_playerAltitude = m_camera.position.magnitude;
        var playerDirection = m_camera.position.normalized;
        
        float dotToPlayer = Vector3.Dot(-transform.forward, playerDirection);

        m_evaluationValue = 0.5f * (dotToPlayer + 1f);

        bool playerInSpace = m_playerAltitude > m_playerAltitudeMinMax.y;

        float altitudeEvaluationValue = Mathf.InverseLerp(m_playerAltitudeMinMax.x, m_playerAltitudeMinMax.y, m_playerAltitude);
        float altitudeAdjustment = m_playerAltitudeAdjustment.Evaluate(altitudeEvaluationValue);
        m_evaluationValue += altitudeAdjustment;
        m_evaluationValue = Mathf.Clamp01(m_evaluationValue);

        m_skyColourEvaluationValue = m_evaluationValue;
        m_sunEvaluationValue = m_evaluationValue;

        if (playerInSpace)
        {
            float spaceMin = m_playerAltitudeMinMax.y;
            float spaceMax = m_playerAltitudeMinMax.y + m_transitionToSpaceAltitude;
            m_spaceFraction = Mathf.InverseLerp(spaceMin, spaceMax, m_playerAltitude);
            m_skyColourEvaluationValue -= m_spaceFraction;
            m_sunEvaluationValue += m_spaceFraction;
            m_skyColourEvaluationValue = Mathf.Clamp01(m_skyColourEvaluationValue);
            m_sunEvaluationValue = Mathf.Clamp01(m_sunEvaluationValue);
        }

        var skyColour = m_skyColour.Evaluate(m_skyColourEvaluationValue);
        m_skybox.SetColor("_Tint", skyColour);

        RenderSettings.ambientLight = m_ambientColour.Evaluate(m_skyColourEvaluationValue);
        m_sunLight.intensity = m_sunIntensity.Evaluate(m_sunEvaluationValue);

        var sunColour = m_sunColour.Evaluate(m_sunEvaluationValue);
        m_sunLight.color = sunColour;
        m_sunMaterial.SetColor("_TintColor", sunColour);

        var sunFlairColour = m_sunFlairColour.Evaluate(m_sunEvaluationValue);
        m_flair.color = sunFlairColour;
       
        m_sunImage.position = m_camera.position - transform.forward * m_sunDistanceFromPlayer;
	}


    public float SunAngleAboveHorizon(Transform observer)
    {
        var observerDirection = observer.position.normalized;

        float dotToObserver = Vector3.Dot(-transform.forward, observerDirection);
        float sunAngleAboveHorizon = Vector3.Angle(transform.forward, observerDirection) - 90f;

        return sunAngleAboveHorizon;
    }


    public float GetEvaluationValue(Transform observer)
    {
        float observerAltitude = observer.position.magnitude;

        var observerDirection = observer.position.normalized;
        float dotToObserver = Vector3.Dot(-transform.forward, observerDirection);
        float evaluationValue = 0.5f * (dotToObserver + 1f);

        float altitudeEvaluationValue = Mathf.InverseLerp(m_playerAltitudeMinMax.x, m_playerAltitudeMinMax.y, observerAltitude);
        float altitudeAdjustment = m_playerAltitudeAdjustment.Evaluate(altitudeEvaluationValue);
        evaluationValue += altitudeAdjustment;
        evaluationValue = Mathf.Clamp01(evaluationValue);

        return evaluationValue;
    }


    public float GetStarsIntensityEvaluationValue(Transform observer)
    {
        float evaluationValue = GetEvaluationValue(observer);
        float observerAltitude = observer.position.magnitude;
        bool observerInSpace = observerAltitude > m_playerAltitudeMinMax.y;

        if (observerInSpace)
        {
            float spaceMin = m_playerAltitudeMinMax.y;
            float spaceMax = m_playerAltitudeMinMax.y + m_transitionToSpaceAltitude;
            float spaceFraction = Mathf.InverseLerp(spaceMin, spaceMax, observerAltitude);
            evaluationValue -= spaceFraction;
        }

        evaluationValue = Mathf.Clamp01(evaluationValue);

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


    private void SwitchCamera(Transform camera, IActivatable activatable)
    {
        m_camera = camera;
    }


    void OnEnable()
    {
        EventManager.StartListening(TransformEventName.CameraActivated, SwitchCamera);
    }


    void OnDisable()
    {
        EventManager.StopListening(TransformEventName.CameraActivated, SwitchCamera);
    }


    void OnDestroy()
    {
        m_skybox.SetColor("_Tint", m_originalSkyboxColour);
        m_sunMaterial.SetColor("_TintColor", m_originalSunColour);
    }
}
