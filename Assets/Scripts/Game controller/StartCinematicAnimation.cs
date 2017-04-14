using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class StartCinematicAnimation : MonoBehaviour
{
    [SerializeField] string m_triggerName = "Start";
    [SerializeField] UnityEvent m_eventsToTriggerAtEndOfAnimation;

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


    public void TriggerEventsAtEndOfAnimation()
    {
        m_eventsToTriggerAtEndOfAnimation.Invoke();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        EventManager.TriggerEvent(StandardEventName.ClosingCinematicEnd);
    }
}
