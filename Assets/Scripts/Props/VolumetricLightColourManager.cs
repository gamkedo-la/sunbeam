using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class VolumetricLightColourManager : MonoBehaviour
{
    private MeshRenderer m_meshRenderer;
    private LightBeamManager m_lightBeamManager;
    private float m_alpha;

    void Awake()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_lightBeamManager = GetComponentInParent<LightBeamManager>();
        m_alpha = m_meshRenderer.material.GetColor("_TintColor").a;
    }


	void Update()
    {
        if (m_lightBeamManager == null)
            return;
        
        var colour = m_lightBeamManager.LightColour;
        colour.a = m_alpha * m_lightBeamManager.Intensity;
        m_meshRenderer.material.SetColor("_TintColor", colour);     
    }
}
