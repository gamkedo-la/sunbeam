using UnityEngine;
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
    [SerializeField] AudioSource m_audioSource;
    [SerializeField] AudioClip m_chargingUpClip;
    [SerializeField] AudioClip m_poweringDownClip;
    [SerializeField] AudioClip m_chargedClip;

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

        if (m_audioSource != null)
        {
            if (m_chargeLevel > m_previousChargeLevel)
            {
                if (m_audioSource.clip != m_chargingUpClip)
                {
                    m_audioSource.Stop();
                    //print("Stop to switch clip for charge up");
                    m_audioSource.clip = m_chargingUpClip;
                }

                if (!m_audioSource.isPlaying)
                {
                    //print("Play charge up");
                    m_audioSource.Play();
                }
            }
            else if (m_chargeLevel < m_previousChargeLevel)
            {
                if (m_audioSource.clip != m_poweringDownClip)
                {
                    m_audioSource.Stop();
                    //print("Stop to switch clip for power down");
                    m_audioSource.clip = m_poweringDownClip;
                }

                if (!m_audioSource.isPlaying)
                {
                    //print("Play power down");
                    m_audioSource.Play();
                }
            }

            if (m_audioSource.isPlaying && m_chargeLevel == 0)
            {
                //print("Stop because fully discharged");
                m_audioSource.Stop();
            }
        }
    }


    private void TriggerActions()
    {
        m_triggerActions.Invoke();

        if (m_audioSource != null)
        {
            m_audioSource.loop = true;
            m_audioSource.clip = m_chargedClip;
            m_audioSource.pitch = 1.0f;
            m_audioSource.Play();
        }

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
