using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LensFlare))]
public class PodLightManager : MonoBehaviour
{
    [SerializeField] float m_onDuration = 1.5f;
    [SerializeField] float m_offDuration = 2f;
    [SerializeField] float m_fadeDuration = 0.5f;
    [SerializeField] bool m_randomStartDelay = true;

    private LensFlare m_flair;
    private Transform m_camera;
    private float m_maxBrightness;
    private float m_brightness;
    private float m_brightnessMultiplier;

	
    void Awake()
    {
        m_flair = GetComponent<LensFlare>();
        m_maxBrightness = m_flair.brightness;
        m_camera = Camera.main.transform;
    }


    void Start()
    {
        StartCoroutine(PulseLight());
    }
	

	void Update()
    {
        float distance = Vector3.Distance(transform.position, m_camera.position);
        float maxBrightness = Mathf.Min(m_maxBrightness, m_maxBrightness / distance);
        m_flair.brightness = maxBrightness * m_brightnessMultiplier;
	}


    private IEnumerator PulseLight()
    {
        m_brightnessMultiplier = 1f;

        if (m_randomStartDelay)
            yield return new WaitForSeconds(Random.Range(0, m_onDuration));

        while(true)
        {
            yield return FadeBrightness(0f);

            yield return new WaitForSeconds(m_offDuration);

            yield return FadeBrightness(1f);

            yield return new WaitForSeconds(m_onDuration);
        }
    }


    private IEnumerator FadeBrightness(float target)
    {
        float startMultiplier = m_brightnessMultiplier;
        float time = 0f;

        while (time < m_fadeDuration)
        {
            float frac = time / m_fadeDuration;

            m_brightnessMultiplier = Mathf.Lerp(startMultiplier, target, frac);

            time += Time.deltaTime;

            yield return null;
        }

        m_brightnessMultiplier = target;
    }
}
