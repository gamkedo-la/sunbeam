using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class SolarPanelManager : MonoBehaviour
{
    [SerializeField] bool m_discharges;
    [SerializeField] float m_chargeTime = 5f;
    [SerializeField] float m_dischargeTime = 3f;
    [SerializeField] Transform m_chargeBar;
    [SerializeField] GameObject m_chargedButton;
    [SerializeField] UnityEvent m_triggerActions;

    private float m_chargeLevel;
    private bool m_charging;
    private bool m_charged;
    private bool m_actionsTrggered;


    void Awake()
    {
        UpdateChargeLevel();
        m_chargedButton.SetActive(false);
    }


    void Update()
    {
        if (m_discharges)
            Discharge();
    }


    public void ChargeUp()
    {
        if (m_charged)
            return;

        m_chargeLevel += Time.deltaTime / m_chargeTime;

        if (m_discharges)
            m_chargeLevel += Time.deltaTime / m_dischargeTime;

        m_chargeLevel = Mathf.Clamp01(m_chargeLevel);

        UpdateChargeLevel();

        if (m_chargeLevel >= 1f)
            m_charged = true;

        if (m_charged && !m_actionsTrggered)
            TriggerActions();
    }


    public void Discharge()
    {
        if (m_charged)
            return;

        m_chargeLevel -= Time.deltaTime / m_dischargeTime;
        m_chargeLevel = Mathf.Clamp01(m_chargeLevel);

        UpdateChargeLevel();
    }


    private void UpdateChargeLevel()
    {
        var chargeBarScale = m_chargeBar.localScale;
        chargeBarScale.y = m_chargeLevel;
        m_chargeBar.localScale = chargeBarScale;

        if (m_chargeLevel >= 1f && m_chargedButton.activeSelf == false)
            m_chargedButton.SetActive(true);
    }


    private void TriggerActions()
    {
        m_triggerActions.Invoke();

        m_actionsTrggered = true;
    }
}
