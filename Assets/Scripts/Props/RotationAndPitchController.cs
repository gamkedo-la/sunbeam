using UnityEngine;
using System.Collections;

public class RotationAndPitchController : PropControllerBase
{
    [Header("Rotation")]
    [SerializeField] bool m_reverseDirection;
    [SerializeField] Transform m_rotationPoint;
    [SerializeField] float m_rotationSpeed = 20f;
    [SerializeField] bool m_constrainRotation = true;
    [SerializeField] Vector2 m_rotationMinMax = new Vector2(-30, 30);

    [Header("Pitch")]
    [SerializeField] Transform m_pitchPoint;
    [SerializeField] float m_pitchSpeed = 10f;
    [SerializeField] Vector2 m_pitchMinMax = new Vector2(20, 40);   

    [Header("Audio")]
    [SerializeField] AudioSource m_rotationAudioSource;
    [SerializeField] AudioSource m_pitchAudioSource;
    [SerializeField] float m_fadeTime = 0.1f;

    [Header("Gizmos")]
    [SerializeField] float m_gizmoLineLength = 10f;

    private float m_rotation;
    private float m_pitch;
    private float m_previousRotation;
    private float m_previousPitch;
    private float m_rotationAudioVolume;
    private float m_pitchAudioVolume;
    private float m_targetRotationVolume;
    private float m_targetPitchVolume;   
    private Coroutine m_rotationAudioFade;
    private Coroutine m_pitchAudioFade;


    protected override void Awake()
    {
        m_pitch = m_pitchPoint != null ? 360f - m_pitchPoint.localEulerAngles.x : 0f;

        if (m_pitch > 180f)
            m_pitch = m_pitch - 360f;

        m_rotation = m_rotationPoint != null ? m_rotationPoint.localEulerAngles.y : 0;

        if (m_rotation > 180f)
            m_rotation = m_rotation - 360f;

        ClampAngles();

        if (m_rotationAudioSource != null)
        {
            m_rotationAudioVolume = m_rotationAudioSource.volume;
            m_rotationAudioSource.volume = 0f;
        }

        if (m_pitchAudioSource != null)
        {
            m_pitchAudioVolume = m_pitchAudioSource.volume;
            m_pitchAudioSource.volume = 0f;
        }

        m_previousRotation = m_rotation;
        m_previousPitch = m_pitch;
    }


    void Update()
    {
        //base.Update();
        if (!m_active)
            return;

        float h1 = Input.GetAxis("Horizontal");
        float v1 = Input.GetAxis("Vertical");
        float h2 = Input.GetAxis("Horizontal look");
        float v2 = Input.GetAxis("Vertical look");

        float h = Mathf.Clamp(h1 + h2, -1f, 1f);
        float v = Mathf.Clamp(v1 + v2, -1f, 1f);

        float rotationToAdd = h * m_rotationSpeed * Time.deltaTime;
        rotationToAdd = m_reverseDirection ? -rotationToAdd : rotationToAdd;

        m_rotation += rotationToAdd;

        float pitchToAdd = v * m_pitchSpeed * Time.deltaTime;
        m_pitch += pitchToAdd;

        ClampAngles();

        if (m_rotationAudioSource != null) 
        {
            bool rotating = m_previousRotation != m_rotation;
            if (rotating && !m_rotationAudioSource.isPlaying && m_targetRotationVolume != m_rotationAudioVolume)
            {
                if (m_rotationAudioFade != null)
                    StopCoroutine(m_rotationAudioFade);

                m_targetRotationVolume = m_rotationAudioVolume;
                m_rotationAudioFade = StartCoroutine(FadeAudio(m_rotationAudioSource, m_targetRotationVolume));
            }
            else if (!rotating && m_rotationAudioSource.isPlaying && m_targetRotationVolume != 0f)
            {
                if (m_rotationAudioFade != null)
                    StopCoroutine(m_rotationAudioFade);

                m_targetRotationVolume = 0f;
                m_rotationAudioFade = StartCoroutine(FadeAudio(m_rotationAudioSource, m_targetRotationVolume));
            }
        }

        if (m_pitchAudioSource != null)
        {
            bool pitching = m_previousPitch != m_pitch;
            if (pitching && !m_pitchAudioSource.isPlaying && m_targetPitchVolume != m_pitchAudioVolume)
            {
                if (m_pitchAudioFade != null)
                    StopCoroutine(m_pitchAudioFade);

                m_targetPitchVolume = m_pitchAudioVolume;
                m_pitchAudioFade = StartCoroutine(FadeAudio(m_pitchAudioSource, m_targetPitchVolume));
            }
            else if (!pitching && m_pitchAudioSource.isPlaying && m_targetPitchVolume != 0f)
            {
                if (m_pitchAudioFade != null) 
                    StopCoroutine(m_pitchAudioFade);
                
                m_targetPitchVolume = 0f;
                m_pitchAudioFade = StartCoroutine(FadeAudio(m_pitchAudioSource, m_targetPitchVolume));
            }
        }

        m_previousRotation = m_rotation;
        m_previousPitch = m_pitch;
    }


    public override void TriggerDeactivation()
    {
        base.TriggerDeactivation();

        if (m_rotationAudioSource != null && m_rotationAudioSource.isPlaying)
        {
            if (m_rotationAudioFade != null)
                StopCoroutine(m_rotationAudioFade);

            m_targetRotationVolume = 0f;
            StartCoroutine(FadeAudio(m_rotationAudioSource, m_targetRotationVolume));
        }

        if (m_pitchAudioSource != null && m_pitchAudioSource.isPlaying)
        {
            if (m_pitchAudioFade != null)
                StopCoroutine(m_pitchAudioFade);

            m_targetPitchVolume = 0f;
            StartCoroutine(FadeAudio(m_pitchAudioSource, m_targetPitchVolume));
        }
    }


    private IEnumerator FadeAudio(AudioSource audioSource, float targetVolume)
    {
        if (targetVolume > 1e-6)
            audioSource.Play();

        float startVolume = audioSource.volume;
        float time = 0;

        while (time < m_fadeTime)
        {
            time += Time.deltaTime;

            float frac = time / m_fadeTime;

            float volume = Mathf.Lerp(startVolume, targetVolume, frac);
            audioSource.volume = volume;

            yield return null;
        }

        audioSource.volume = targetVolume;

        if (targetVolume < 1e-4)
            audioSource.Stop();
    }


    private void ClampAngles()
    {
        if (m_constrainRotation)
        {
            m_rotation = Mathf.Clamp(m_rotation, m_rotationMinMax.x, m_rotationMinMax.y);
        }

        if (m_rotationPoint != null)
            m_rotationPoint.localEulerAngles = Vector3.up * m_rotation;

        m_pitch = Mathf.Clamp(m_pitch, m_pitchMinMax.x, m_pitchMinMax.y);

        if (m_pitchPoint != null)
            m_pitchPoint.localEulerAngles = Vector3.left * m_pitch;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        if (m_constrainRotation)
        {
            var rotationMin = Quaternion.Euler(transform.up * m_rotationMinMax.x) * transform.forward;
            var rotationMax = Quaternion.Euler(transform.up * m_rotationMinMax.y) * transform.forward;

            if (m_rotationPoint != null)
            {
                Gizmos.DrawRay(m_rotationPoint.position + m_rotationPoint.up * 0.5f, rotationMin * m_gizmoLineLength);
                Gizmos.DrawRay(m_rotationPoint.position + m_rotationPoint.up * 0.5f, rotationMax * m_gizmoLineLength);
            }
        }

        Gizmos.color = Color.cyan;

        var pitchMin = Quaternion.Euler(-transform.right * m_pitchMinMax.x) * transform.forward;
        var pitchMax = Quaternion.Euler(-transform.right * m_pitchMinMax.y) * transform.forward;

        if (m_pitchPoint != null)
        {
            Gizmos.DrawRay(m_pitchPoint.position, pitchMin * m_gizmoLineLength);
            Gizmos.DrawRay(m_pitchPoint.position, pitchMax * m_gizmoLineLength);
        }
    }
}
