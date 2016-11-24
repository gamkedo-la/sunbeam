using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class WaterUvAnimator : MonoBehaviour
{
    [SerializeField] Vector2 m_primaryUvSpeed =new Vector2(0, -0.1f);
    [SerializeField] Vector2 m_secondaryUvSpeed = new Vector2(0.1f, 0);

    private MeshRenderer m_meshRenderer;


    void Awake()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
    }


	void Update()
    {
        var mainOffset = Time.time * m_primaryUvSpeed;
        var secondaryOffset = Time.time * m_secondaryUvSpeed;

        m_meshRenderer.material.mainTextureOffset =  mainOffset;
        m_meshRenderer.material.SetTextureOffset("_DetailAlbedoMap", secondaryOffset);
    }
}
