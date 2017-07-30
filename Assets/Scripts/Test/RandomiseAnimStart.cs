using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RandomiseAnimStart : MonoBehaviour
{
    private Animator m_anim;


	void Awake()
    {
        m_anim = GetComponent<Animator>();
        var state = m_anim.GetCurrentAnimatorStateInfo(0);
        m_anim.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
    }
}
