﻿using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class SolarPanelManager : MonoBehaviour
{
    [SerializeField] bool m_discharges;
    [SerializeField] bool m_startCharged;
    [SerializeField] bool m_dischargesOnceCharged;
    [SerializeField] float m_chargeTime = 5f;
    [SerializeField] float m_dischargeTime = 3f;
    [SerializeField] Transform m_chargeBar;
    [SerializeField] GameObject m_chargedButton;
    [SerializeField] UnityEvent m_triggerActions;
    [SerializeField] UnityEvent m_dischargedTriggerActions;

    [Header("Audio")]
    [SerializeField] AudioSource m_chargingUpAudio;
    [SerializeField] AudioSource m_poweringDownAudio;
    [SerializeField] AudioSource m_actionsTriggeredAudio;
    [SerializeField] AudioSource m_chargedAudio;

    private float m_chargeLevel;
    private float m_previousChargeLevel;
    private bool m_charging;
    private bool m_charged;
    private bool m_discharged;
    private bool m_chargedActionsTrggered;
    private bool m_dischargedActionsTriggered;


    void Awake()
    {
        if (m_startCharged)
        {
            m_chargeLevel = 1f;
            m_chargedActionsTrggered = true;
            m_dischargedActionsTriggered = false;
            m_discharged = false;
        }
        else
        {
            m_chargeLevel = 0f;
            m_chargedActionsTrggered = false;
            m_dischargedActionsTriggered = true;
            m_discharged = true;
        }

        m_previousChargeLevel = m_chargeLevel;

        UpdateChargeLevel();    
    }


    void Update()
    {
        if (m_charging)
        {
            Charge();
            m_charging = false;
        }

        if (m_discharges)
            Discharge();

        m_previousChargeLevel = m_chargeLevel;
    }


    public void ChargeUp()
    {
        if (m_charged && (!m_discharges || !m_dischargesOnceCharged))
            return;

        m_charging = true;
    }


    private void Charge()
    {
        m_chargeLevel += Time.deltaTime / m_chargeTime;

        if (m_discharges)
            m_chargeLevel += Time.deltaTime / m_dischargeTime;

        m_chargeLevel = Mathf.Clamp01(m_chargeLevel);

        UpdateChargeLevel();

        if (m_chargeLevel >= 1f)
        {
            m_charged = true;
            m_discharged = false;
        }

        if (m_charged && !m_chargedActionsTrggered)
            TriggerActions();
    }


    private void Discharge()
    {
        if (m_charged && (!m_discharges || !m_dischargesOnceCharged))
            return;

        m_chargeLevel -= Time.deltaTime / m_dischargeTime;
        m_chargeLevel = Mathf.Clamp01(m_chargeLevel);

        UpdateChargeLevel();

        if (m_chargeLevel <= 0f)
        {
            m_discharged = true;
            m_charged = false;
        }

        if (m_discharged && !m_dischargedActionsTriggered)
            TriggerDischargedActions();
    }


    private void UpdateChargeLevel()
    {
        var chargeBarScale = m_chargeBar.localScale;
        chargeBarScale.y = m_chargeLevel;
        m_chargeBar.localScale = chargeBarScale;

        if (m_chargeLevel >= 1f && m_chargedButton.activeSelf == false)
            m_chargedButton.SetActive(true);
        else if (m_chargeLevel <= 0f && m_chargedButton.activeSelf == true)
            m_chargedButton.SetActive(false);

        if (m_chargeLevel > m_previousChargeLevel)
        {
            if (m_poweringDownAudio != null && m_poweringDownAudio.isPlaying)
                m_poweringDownAudio.Stop();

            if (m_chargingUpAudio != null && !m_chargingUpAudio.isPlaying)   
                m_chargingUpAudio.Play();
        }
        else if (m_chargeLevel < m_previousChargeLevel)
        {
            if (m_chargingUpAudio != null && m_chargingUpAudio.isPlaying)
                m_chargingUpAudio.Stop();

            if (m_poweringDownAudio != null && !m_poweringDownAudio.isPlaying)
                m_poweringDownAudio.Play();   
        }

        if (m_chargeLevel == 0)
        {
            if (m_poweringDownAudio != null && m_poweringDownAudio.isPlaying)
                m_poweringDownAudio.Stop();
        }
    }


    private void TriggerActions()
    {
        m_triggerActions.Invoke();

        if (m_chargingUpAudio != null)
            m_chargingUpAudio.Stop();

        if (m_poweringDownAudio != null)
            m_poweringDownAudio.Stop();

        if (m_actionsTriggeredAudio != null)
            m_actionsTriggeredAudio.Play();

        if (m_chargedAudio != null)
            m_chargedAudio.Play();
        
        m_chargedActionsTrggered = true;
        m_dischargedActionsTriggered = false;
    }


    private void TriggerDischargedActions()
    {
        m_dischargedTriggerActions.Invoke();

        m_chargedActionsTrggered = false;
        m_dischargedActionsTriggered = true;
    }
}
