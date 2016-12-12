using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WoodenTowerPlatformController : MonoBehaviour
{
    private Animator m_anim;
    private int m_up;
    private int m_down;
    private Transform m_player;

	
    void Awake()
    {
        m_anim = GetComponent<Animator>();
        m_up = Animator.StringToHash("Up");
        m_down = Animator.StringToHash("Down");
        m_player = GameObject.FindGameObjectWithTag(Tags.Player).transform;
    }


    public void PlatformUp()
    {
        m_anim.SetTrigger(m_up);
    }


    public void PlatformDown()
    {
        m_anim.SetTrigger(m_down);
    }


    public void UnparentPlayer()
    {
        m_player.parent = null;
    }
}
