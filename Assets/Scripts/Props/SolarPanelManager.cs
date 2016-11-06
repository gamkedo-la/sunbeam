using UnityEngine;
using System.Collections;

public class SolarPanelManager : MonoBehaviour
{
    [SerializeField] float m_chargeTime = 5f;
    [SerializeField] Transform m_chargeBar;
    [SerializeField] GameObject m_chargedButton;

    private float m_chargeLevel;


    void Awake()
    {
        UpdateChargeLevel();
        m_chargedButton.SetActive(false);
    }


    void Update()
    {
        //ChargeUp();
        UpdateChargeLevel();
    }


    public void ChargeUp()
    {
        m_chargeLevel += Time.deltaTime / m_chargeTime;
        m_chargeLevel = Mathf.Clamp01(m_chargeLevel);
    }


    private void UpdateChargeLevel()
    {
        var chargeBarScale = m_chargeBar.localScale;
        chargeBarScale.y = m_chargeLevel;
        m_chargeBar.localScale = chargeBarScale;

        if (m_chargeLevel >= 1f && m_chargedButton.activeSelf == false)
            m_chargedButton.SetActive(true);
    }
}
