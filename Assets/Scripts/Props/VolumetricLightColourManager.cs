using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class VolumetricLightColourManager : MonoBehaviour
{
    private MeshRenderer m_meshRenderer;
    private Light m_light;
    private float m_alpha;

    void Awake()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_light = GetComponentInParent<Light>();
        m_alpha = m_meshRenderer.material.GetColor("_TintColor").a;
    }


	void Update()
    {
        if (m_light == null)
            return;
        
        var colour = m_light.color;
        colour.a = m_alpha;
        m_meshRenderer.material.SetColor("_TintColor", colour);
        
    }
}
