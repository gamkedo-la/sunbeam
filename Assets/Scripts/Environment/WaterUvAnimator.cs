using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class WaterUvAnimator : MonoBehaviour
{
    [SerializeField] float m_primaryUvSpeed = 0.1f;
    [SerializeField] float m_secondaryUvSpeed = 0.1f;

    private MeshRenderer m_meshRenderer;


    void Awake()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
    }


	void Update()
    {
        float mainOffset = Time.time * m_primaryUvSpeed;
        float secondaryOffset = Time.time * m_secondaryUvSpeed;

        m_meshRenderer.material.mainTextureOffset = new Vector2(mainOffset, 0);
        m_meshRenderer.material.SetTextureOffset("_DetailAlbedoMap", new Vector2(0, secondaryOffset));
    }
}
