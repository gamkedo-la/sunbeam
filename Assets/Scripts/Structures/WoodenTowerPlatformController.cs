using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WoodenTowerPlatformController : MonoBehaviour
{
    private Animator m_anim;
    private int m_up;
    private int m_down;

	
    void Awake()
    {
        m_anim = GetComponent<Animator>();
        //m_up = 
    }


    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Tags.PlayerBody))
            return;

        // TODO: determine state of animator before triggering correct animation

        print("Triggered by " + other.name);
        m_anim.SetTrigger("Up");
    }
}
