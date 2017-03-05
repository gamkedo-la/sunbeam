using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActivationCounter : MonoBehaviour
{
    [SerializeField] int m_activationCountTrigger = 2;
    [SerializeField] bool m_deactivationAllowed = true;
    [SerializeField] UnityEvent m_activationActions;
    [SerializeField] UnityEvent m_deactivationActions;

    private int m_activationCounter;
    private bool m_activationActionsTrggered;
    private bool m_deactivationsActionsTriggered;


    public void IncrementActivationCounter()
    {
        m_activationCounter++;

        if (m_activationCounter == m_activationCountTrigger && !m_activationActionsTrggered)
            TriggerActivationActions();
    }


    public void DecrementActivtionCounter()
    {
        m_activationCounter--;

        if (m_deactivationAllowed && m_activationCounter < m_activationCountTrigger && !m_deactivationsActionsTriggered)
            TriggerDeactivationActions();
    }


    private void TriggerActivationActions()
    {
        m_activationActions.Invoke();

        m_activationActionsTrggered = true;
        m_deactivationsActionsTriggered = false;
    }


    private void TriggerDeactivationActions()
    {
        m_deactivationActions.Invoke();

        m_activationActionsTrggered = false;
        m_deactivationsActionsTriggered = true;
    }
}
