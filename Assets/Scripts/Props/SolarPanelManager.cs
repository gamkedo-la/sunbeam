using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class SolarPanelManager : MonoBehaviour
{
    [SerializeField] float m_chargeTime = 5f;
    [SerializeField] Transform m_chargeBar;
    [SerializeField] GameObject m_chargedButton;
    [SerializeField] UnityEvent m_triggerActions;

    private float m_chargeLevel;
    private bool m_charged;
    private bool m_actionsTrggered;


    void Awake()
    {
        UpdateChargeLevel();
        m_chargedButton.SetActive(false);
    }


    public void ChargeUp()
    {
        if (m_charged)
            return;

        m_chargeLevel += Time.deltaTime / m_chargeTime;
        m_chargeLevel = Mathf.Clamp01(m_chargeLevel);

        UpdateChargeLevel();

        if (m_chargeLevel >= 1f)
            m_charged = true;

        if (m_charged && !m_actionsTrggered)
            TriggerActions();
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
