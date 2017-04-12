using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerTriggerable : MonoBehaviour
{
    [SerializeField] bool m_startActive = true;
    [SerializeField] bool m_dontTriggerCameraMovement;
    [SerializeField] bool m_disableTriggerOnActivate;
    [SerializeField] bool m_resetAfterDelay;
    [SerializeField] float m_resetAfterSeconds = 1f;
    [SerializeField] Transform m_tranformToParentPlayerTo;
    [SerializeField] UnityEvent m_actionsOnActivate;
    [SerializeField] UnityEvent m_actionsOnDeactivate;
    public bool m_showControls;
    [SerializeField] bool m_dontShowRotationControls;

    private Collider m_trigger;
    private Transform m_player;
    private bool m_active;
    private bool m_canBeTriggered;
    private bool m_actionsTrggered;
    private bool m_activationTriggered;
    private bool m_submit;
    private bool m_cancel;
    private bool m_paused;
    private bool m_activeWhenCinemticStarted;
    private bool m_canBeTriggeredWhenCinematicStarted;


    void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag(Tags.Player).transform;
        m_trigger = GetComponent<Collider>();

        m_active = m_startActive;
    }


    void Start()
    {
        StartCoroutine(CheckForAxisInput("Activate", SetSubmit));
        StartCoroutine(CheckForAxisInput("Cancel", SetCancel));
    }


    protected virtual void Update()
    {
        if (m_canBeTriggered && !m_paused)
        {
            if (!m_activationTriggered && m_submit)
            {
                m_activationTriggered = true;
                //print("Trigger activate action");
                TriggerActivateActions();

                if (m_disableTriggerOnActivate && m_trigger != null)
                {
                    m_trigger.enabled = false;
                    m_activationTriggered = false;
                }

                if (!m_dontTriggerCameraMovement)
                    EventManager.TriggerEvent(BooleanEventName.Interact, false);

                if (m_showControls && !m_dontShowRotationControls)
                    EventManager.TriggerEvent(BooleanEventName.ShowRotationControls, true);

                if (m_resetAfterDelay)
                    StartCoroutine(Reset());
            }
            else if (m_activationTriggered && (m_submit || m_cancel))
            {
                m_activationTriggered = false;
                //print("Trigger deactivate action");
                TriggerDeactivateActions();

                EventManager.TriggerEvent(BooleanEventName.Interact, true);
                
                if (m_showControls && !m_dontShowRotationControls)
                    EventManager.TriggerEvent(BooleanEventName.ShowRotationControls, false);
            }
        }

        m_submit = false;
        m_cancel = false;
    }


    private IEnumerator CheckForAxisInput(string axisName, Action action)
    {
        bool buttonPressedPreviously = false;

        while (true)
        {
            bool buttonPressed = Input.GetAxisRaw(axisName) == 1f;

            if (buttonPressed && !buttonPressedPreviously)
            {
                //print(axisName + " pressed");
                action.Invoke();
            }

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


    private void TriggerActivateActions()
    {
        m_player.parent = m_tranformToParentPlayerTo;
        m_actionsOnActivate.Invoke();
    }


    private void TriggerDeactivateActions()
    {
        m_actionsOnDeactivate.Invoke();
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


    private void OnPause()
    {
        m_paused = true;
    }


    private void OnUnpause()
    {
        m_paused = false;
    }


    private void TriggerClosingCinematic()
    {
        m_activeWhenCinemticStarted = m_active;
        m_canBeTriggeredWhenCinematicStarted = m_canBeTriggered;
        m_active = false;
        m_canBeTriggered = false;
        m_activationTriggered = false;
    }


    private void ContinueExploring()
    {
        m_active = m_activeWhenCinemticStarted;
        m_canBeTriggered = m_canBeTriggeredWhenCinematicStarted;
    }


    void OnEnable()
    {
        EventManager.StartListening(StandardEventName.Pause, OnPause);
        EventManager.StartListening(StandardEventName.Unpause, OnUnpause);
        EventManager.StartListening(StandardEventName.TriggerClosingCinematic, TriggerClosingCinematic);
        EventManager.StartListening(StandardEventName.ContinueExploring, ContinueExploring);
    }


    void OnDisable()
    {
        EventManager.StopListening(StandardEventName.Pause, OnPause);
        EventManager.StopListening(StandardEventName.Unpause, OnUnpause);
        EventManager.StopListening(StandardEventName.TriggerClosingCinematic, TriggerClosingCinematic);
        EventManager.StopListening(StandardEventName.ContinueExploring, ContinueExploring);
    }
}
