using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class TestAction : MonoBehaviour
{
    private MeshRenderer m_renderer;


    void Awake()
    {
        m_renderer = GetComponent<MeshRenderer>();
        m_renderer.enabled = false;
    }


    public void Appear()
    {
        m_renderer.enabled = true;
    }
}
