using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForInteractables : MonoBehaviour
{
    [SerializeField] float m_checkDistance = 2f;
    [SerializeField] LayerMask m_mask;

    private bool m_canBeTriggered;
    private bool m_canBeTriggeredLastFrame;
    private PlayerTriggerable m_playerTriggereable;
	

    void Update()
    {
        var ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, m_checkDistance, m_mask))
        {
            if (m_playerTriggereable == null)
            {
                m_playerTriggereable = hit.transform.GetComponent<PlayerTriggerable>();

                if (m_playerTriggereable != null && m_playerTriggereable.Active)
                {
                    m_playerTriggereable.SetCanBeTriggered(true);
                    m_canBeTriggered = true;
                }
            }

            Debug.DrawRay(transform.position, transform.forward * m_checkDistance, Color.black);          
        }
        else
        {
            m_canBeTriggered = false;

            if (m_playerTriggereable != null && m_playerTriggereable.Active)
            {
                m_playerTriggereable.SetCanBeTriggered(false);
                m_playerTriggereable = null;
            }

            Debug.DrawRay(transform.position, transform.forward * m_checkDistance, Color.white);
        }

        if (m_canBeTriggered != m_canBeTriggeredLastFrame)
        {
            //print("Can be triggered: " + m_canBeTriggered);
            EventManager.TriggerEvent(BooleanEventName.Interact, m_canBeTriggered);
        }

        m_canBeTriggeredLastFrame = m_canBeTriggered;
    }
}
