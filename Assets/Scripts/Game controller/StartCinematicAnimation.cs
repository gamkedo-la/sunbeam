using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class StartCinematicAnimation : MonoBehaviour
{
    [SerializeField] string m_triggerName = "Start";

    private Animator m_anim;

	
    void Awake()
    {
        m_anim = GetComponent<Animator>();
    }


    public void StartAnimation(float delay)
    {
        StartCoroutine(StartAnimationAfterDelay(delay));
    }


    private IEnumerator StartAnimationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        m_anim.SetTrigger(m_triggerName);
    }
}
