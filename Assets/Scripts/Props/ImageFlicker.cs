using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageFlicker : MonoBehaviour
{
    [SerializeField] Vector2 m_alphaMinMax = new Vector2(0.6f, 0.7f);
    [SerializeField] float m_flickerSpeed = 10f;

    private Image m_image;
	
    void Awake()
    {
        m_image = GetComponent<Image>();
    }


	void Update()
    {
        float alpha = Mathf.PerlinNoise(1f, Time.time * m_flickerSpeed);
        alpha = Mathf.Lerp(m_alphaMinMax.x, m_alphaMinMax.y, alpha);

        var colour = m_image.color;

        colour.a = alpha;

        m_image.color = colour;
	}
}
