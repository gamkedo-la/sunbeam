using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class AddMaterial : MonoBehaviour
{
    [SerializeField] Material m_materialToAdd;

    private MeshRenderer m_renderer;


    void Awake()
    {
        m_renderer = GetComponent<MeshRenderer>();

        m_renderer.material = m_materialToAdd;
    }


    void OnDestroy()
    {
        m_renderer.material = null;
    }
}
