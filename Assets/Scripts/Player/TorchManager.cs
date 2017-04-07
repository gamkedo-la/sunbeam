using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(AudioSource))]
public class TorchManager : MonoBehaviour
{
    [SerializeField] float m_sunAngleForTorchToggle = 0f;

    private Light m_light;
    private AudioSource m_audioSource;
    private float m_previousSunAngle;
    private SkyManager m_skyManager;
    private bool m_activeManagement;


    void Start()
    {
        m_light = GetComponent<Light>();
        m_audioSource = GetComponent<AudioSource>();
        m_skyManager = FindObjectOfType<SkyManager>();

        if (m_skyManager != null)
        {
            m_previousSunAngle = m_skyManager.SunAngleAboveHorizon(transform);
            m_activeManagement = true;

            if (m_previousSunAngle <= m_sunAngleForTorchToggle)
                m_light.enabled = true;
            else
                m_light.enabled = false;
        }
        else
        {
            m_activeManagement = false;
            m_light.enabled = false;
        }

        if (GameController.AllowCheatMode)
            StartCoroutine(CheckForAxisInput("Torch", ToggleLight));
    }
	

	void Update()
    {
        if (!m_activeManagement)
            return;

        float sunAngle = m_skyManager.SunAngleAboveHorizon(transform);

        if (sunAngle <= m_sunAngleForTorchToggle
            && m_previousSunAngle > m_sunAngleForTorchToggle
            && !m_light.enabled)
        {
            //print("Turn light on automatically");
            SwitchTorch(true);           
        }
        else if (sunAngle > m_sunAngleForTorchToggle
            && m_previousSunAngle <= m_sunAngleForTorchToggle
            && m_light.enabled)
        {
            //print("Turn light off automatically");
            SwitchTorch(false);
        }

        m_previousSunAngle = sunAngle;
    }


    private IEnumerator CheckForAxisInput(string axisName, Action action)
    {
        bool buttonPressedPreviously = false;

        while (true)
        {
            if (GameController.AllowCheatMode)
            {
                bool buttonPressed = Input.GetAxisRaw(axisName) == 1f;

                if (buttonPressed && !buttonPressedPreviously)
                    action.Invoke();

                buttonPressedPreviously = buttonPressed;
            }

            yield return null;
        }
    }


    private void ToggleLight()
    {
        m_light.enabled = !m_light.enabled;
        m_audioSource.Play();
    }


    private void SwitchTorch(bool on)
    {
        if (m_light.enabled && !on)
            ToggleLight();

        if (!m_light.enabled && on)
            ToggleLight();     
    }


    private void TriggerClosingCinematic()
    {
        StopAllCoroutines();
        m_activeManagement = false;
        m_light.enabled = false;
    }


    private void ContinueExploring()
    {
        m_activeManagement = true;

        if (GameController.AllowCheatMode)
            StartCoroutine(CheckForAxisInput("Torch", ToggleLight));
    }


    void OnEnable()
    {
        EventManager.StartListening(BooleanEventName.SwitchTorch, SwitchTorch);
        EventManager.StartListening(StandardEventName.TriggerClosingCinematic, TriggerClosingCinematic);
        EventManager.StartListening(StandardEventName.ContinueExploring, ContinueExploring);
    }


    void OnDisable()
    {
        EventManager.StopListening(BooleanEventName.SwitchTorch, SwitchTorch);
        EventManager.StopListening(StandardEventName.TriggerClosingCinematic, TriggerClosingCinematic);
        EventManager.StopListening(StandardEventName.ContinueExploring, ContinueExploring);
    }
}
