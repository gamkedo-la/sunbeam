using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerTriggerable : MonoBehaviour
{
    [SerializeField] bool m_startActive = true;
    //[SerializeField] float m_playerLookMaxAngle = 50f;
    [SerializeField] bool m_dontHideInteractIconOnActivate;
    [SerializeField] bool m_resetAfterDelay;
    [SerializeField] float m_resetAfterSeconds = 1f;
    [SerializeField] Transform m_tranformToParentPlayerTo;
    [SerializeField] UnityEvent m_actionsOnActivate;
    [SerializeField] UnityEvent m_actionsOnDeactivate;
    public bool m_showControls;

    private Transform m_player;
    //private Transform m_cameraAnchor;
    private bool m_active;
    private bool m_canBeTriggered;
    //private bool m_canBeTriggeredPreviousFrame;
    private bool m_actionsTrggered;
    private static bool m_activationTriggered;
    //private static int m_howManyCanBeTriggered;
    private bool m_submit;
    private bool m_cancel;
    

    void Awake()
    {
        //m_howManyCanBeTriggered = 0;
        m_player = GameObject.FindGameObjectWithTag(Tags.Player).transform;
        //m_cameraAnchor = Camera.main.transform.parent;

        m_active = m_startActive;
    }


    void Start()
    {
        StartCoroutine(CheckForAxisInput("Submit", SetSubmit));
        StartCoroutine(CheckForAxisInput("Cancel", SetCancel));
    }


    protected virtual void Update()
    {
        if (m_canBeTriggered)
        {
            if (!m_activationTriggered && m_submit)
            {
                m_activationTriggered = true;
                TriggerActivateActions();

                if (!m_dontHideInteractIconOnActivate)
                    EventManager.TriggerEvent(BooleanEventName.Interact, false);

                if (m_showControls)
                    EventManager.TriggerEvent(BooleanEventName.ShowRotationControls, true);

                if (m_resetAfterDelay)
                    StartCoroutine(Reset());
            }
            else if (m_activationTriggered && (m_submit || m_cancel))
            {
                m_activationTriggered = false;
                TriggerDeactivateActions();

                EventManager.TriggerEvent(BooleanEventName.Interact, true);
                
                if (m_showControls)
                    EventManager.TriggerEvent(BooleanEventName.ShowRotationControls, false);
            }
        }

        m_submit = false;
        m_cancel = false;

        //m_canBeTriggeredPreviousFrame = m_canBeTriggered;
    }


    private IEnumerator CheckForAxisInput(string axisName, Action action)
    {
        bool buttonPressedPreviously = false;

        while (true)
        {
            bool buttonPressed = Input.GetAxisRaw(axisName) == 1f;

            if (buttonPressed && !buttonPressedPreviously)
                action.Invoke();

            buttonPressedPreviously = buttonPressed;

            yield return null;
        }
    }


    private void SetSubmit()
    {
        m_submit = true;
    }


    private void SetCancel()
    {
        m_cancel = true;
    }


    public void SetCanBeTriggered(bool canBeTriggered)
    {
        m_canBeTriggered = canBeTriggered;
    }


    public bool Active
    {
        get { return m_active; }
    }


    //void OnTriggerStay(Collider other)
    //{
    //    if (!m_active)
    //        return;

    //    var cameraLookDirection = m_cameraAnchor.forward;
    //    var cameraToTrigger = transform.position - m_cameraAnchor.position;

    //    float angle = Vector3.Angle(cameraLookDirection, cameraToTrigger);

    //    m_canBeTriggered = angle <= m_playerLookMaxAngle;

    //    if (m_canBeTriggered && !m_canBeTriggeredPreviousFrame)
    //    {
    //        m_howManyCanBeTriggered++;
    //        print(m_howManyCanBeTriggered);
    //    }
    //    else if (m_canBeTriggeredPreviousFrame && !m_canBeTriggered)
    //    {
    //        m_howManyCanBeTriggered--;
    //        print(m_howManyCanBeTriggered);
    //    }

    //    if (!m_activationTriggered)
    //        EventManager.TriggerEvent(BooleanEventName.Interact, m_howManyCanBeTriggered > 0);
    //}


    //void OnTriggerExit(Collider other)
    //{
    //    m_canBeTriggered = false;

    //    if (m_canBeTriggeredPreviousFrame)
    //    {
    //        m_howManyCanBeTriggered--;
    //        print(m_howManyCanBeTriggered);
    //    }

    //    if (m_howManyCanBeTriggered == 0)
    //    {
    //        EventManager.TriggerEvent(BooleanEventName.Interact, false);
    //    }
    //}


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

        m_activationTriggered = false;
    }


    public void Activate()
    {
        m_active = true;
    }
}
