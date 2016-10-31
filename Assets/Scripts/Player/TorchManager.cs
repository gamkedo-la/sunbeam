using UnityEngine;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(AudioSource))]
public class TorchManager : MonoBehaviour
{
    [SerializeField] float m_sunAngleForTorchToggle = 0f;

    private Light m_light;
    private AudioSource m_audioSource;
    private float m_previousSunAngle;


    void Start()
    {
        m_light = GetComponent<Light>();
        m_audioSource = GetComponent<AudioSource>();
        m_previousSunAngle = SkyManager.SunAngleAboveHorizon;

        if (m_previousSunAngle <= m_sunAngleForTorchToggle)
            m_light.enabled = true;
        else
            m_light.enabled = false;
    }
	

	void Update()
    {
        if (GameController.AllowCheatMode && Input.GetKeyDown(KeyCode.T))
            ToggleLight();

        float sunAngle = SkyManager.SunAngleAboveHorizon;

        if (sunAngle <= m_sunAngleForTorchToggle
            && !m_light.enabled)
        {
            SwitchTorch(true);           
        }
        else if (sunAngle > m_sunAngleForTorchToggle
            && m_light.enabled)
        {
            SwitchTorch(false);
        }

        m_previousSunAngle = sunAngle;
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


    void OnEnable()
    {
        EventManager.StartListening(BooleanEventName.SwitchTorch, SwitchTorch);
    }


    void OnDisable()
    {
        EventManager.StopListening(BooleanEventName.SwitchTorch, SwitchTorch);
    }
}
