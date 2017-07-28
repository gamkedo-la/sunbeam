using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] float m_speed = 2.5f;
    [SerializeField] float m_animationSpeed = 0.25f;

    private Animator m_anim;


    void Awake()
    {
        m_anim = GetComponent<Animator>();
        m_anim.speed = m_animationSpeed;
    }


	void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * m_speed * m_animationSpeed, Space.Self);
	}
}
