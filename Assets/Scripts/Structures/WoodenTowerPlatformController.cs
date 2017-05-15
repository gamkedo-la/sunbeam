using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class WoodenTowerPlatformController : MonoBehaviour
{
    [SerializeField] UnityEvent m_platformUpEndActions;
    [SerializeField] UnityEvent m_platformDownEndActions;

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


    public void PlatformUpEndTrigger()
    {
        UnparentPlayer();
        m_platformUpEndActions.Invoke();
    }


    public void PlatformDownEndTrigger()
    {
        UnparentPlayer();
        m_platformDownEndActions.Invoke();
    }
}
