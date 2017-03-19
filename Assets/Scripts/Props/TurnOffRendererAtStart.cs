using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class TurnOffRendererAtStart : MonoBehaviour
{
    [SerializeField] bool m_active = true;

    void Awake()
    {
        if (!m_active)
            return;

        var renderer = GetComponent<MeshRenderer>(); 
        renderer.enabled = false;
    }

}
