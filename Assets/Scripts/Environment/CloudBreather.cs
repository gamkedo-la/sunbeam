using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudBreather : MonoBehaviour
{
    private Vector2 m_scaleMultiplierMinMax = new Vector2(0.7f, 1.2f);
    private float m_breathingRate = 0.05f;

    private Vector3 m_initialScale;
    private float m_perlinX;


    void Awake()
    {
        m_initialScale = transform.localScale;
        m_perlinX = Random.Range(0f, 100f);
    }
	

	void Update()
    {
        float scale = Mathf.PerlinNoise(m_perlinX, Time.time * m_breathingRate);
        scale = Mathf.Lerp(m_scaleMultiplierMinMax.x, m_scaleMultiplierMinMax.y, scale);

        transform.localScale = scale * m_initialScale;
    }
}
