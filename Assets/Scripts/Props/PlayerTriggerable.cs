using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerTriggerable : MonoBehaviour
{
    [SerializeField] bool m_startActive = true;
    [SerializeField] float m_playerLookMaxAngle = 50f;
    [SerializeField] bool m_resetAfterDelay;
    [SerializeField] float m_resetAfterSeconds = 1f;
    [SerializeField] Transform m_tranformToParentPlayerTo;
    [SerializeField] UnityEvent m_actionsOnActivate;
    [SerializeField] UnityEvent m_actionsOnDeactivate;

    private Transform m_player;
    private Transform m_cameraAnchor;
    private bool m_active;
    private bool m_canBeTriggered;
    private bool m_canBeTriggeredPreviousFrame;
    private bool m_actionsTrggered;
    private bool m_activationTiggered;
    

    void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag(Tags.Player).transform;
        m_cameraAnchor = Camera.main.transform.parent;

        m_active = m_startActive;
    }


    protected virtual void Update()
    {
        if (m_canBeTriggered)
        {
            if (!m_activationTiggered && Input.GetAxisRaw("Submit") == 1)
            {
                m_activationTiggered = true;
                TriggerActivateActions();
                EventManager.TriggerEvent(BooleanEventName.Interact, false);

                if (m_resetAfterDelay)
                    StartCoroutine(Reset());
            }
            else if (m_activationTiggered && Input.GetAxisRaw("Cancel") == 1)
            {
                m_activationTiggered = false;
                TriggerDeactivateActions();
                EventManager.TriggerEvent(BooleanEventName.Interact, true);
            }
        }

        m_canBeTriggeredPreviousFrame = m_canBeTriggered;
    }


    void OnTriggerStay(Collider other)
    {
        if (!m_active)
            return;

        var cameraLookDirection = m_cameraAnchor.forward;
        var cameraToTrigger = transform.position - m_cameraAnchor.position;

        float angle = Vector3.Angle(cameraLookDirection, cameraToTrigger);

        m_canBeTriggered = angle <= m_playerLookMaxAngle;

        if (m_activationTiggered)
            return;

        if (m_canBeTriggered && !m_canBeTriggeredPreviousFrame)
            EventManager.TriggerEvent(BooleanEventName.Interact, true);
        else if (!m_canBeTriggered && m_canBeTriggeredPreviousFrame)
            EventManager.TriggerEvent(BooleanEventName.Interact, false);
    }


    void OnTriggerExit(Collider other)
    {
        m_canBeTriggered = false;

        EventManager.TriggerEvent(BooleanEventName.Interact, false);
    }


    private void TriggerActivateActions()
    {
        m_player.parent = m_tranformToParentPlayerTo;
        //print("Trigger activate actions");
        m_actionsOnActivate.Invoke();

        //m_actionsTrggered = true;
    }


    private void TriggerDeactivateActions()
    {
        m_actionsOnDeactivate.Invoke();

        //m_actionsTrggered = true;
    }


    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(m_resetAfterSeconds);

        m_activationTiggered = false;
    }


    public void Activate()
    {
        m_active = true;
    }
}
