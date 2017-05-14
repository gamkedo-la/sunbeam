using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Collider))]
public class TriggerEnterActions : MonoBehaviour
{
    [SerializeField] UnityEvent m_actionsOnTriggerEnter;
    [SerializeField] UnityEvent m_actionsOnTriggerExit;
 
	
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Tags.PlayerCollider))
            return;

        m_actionsOnTriggerEnter.Invoke();
    }


    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(Tags.PlayerCollider))
            return;

        m_actionsOnTriggerExit.Invoke();
    }
}
