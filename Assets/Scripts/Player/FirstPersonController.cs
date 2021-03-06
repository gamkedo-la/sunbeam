﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MouseLook))]
[RequireComponent(typeof(JoystickLook))]
public class FirstPersonController : MonoBehaviour
{
    public enum WalkSfxType
    {
        Footsteps = 0,
        Motor = 1
    }

    [Header("Look")]
    [SerializeField] Transform m_camera;
    [SerializeField] Vector2 m_verticalLookMinMax = new Vector2(-90f, 90f);

    [Header("Walk")]
    [SerializeField] float m_walkSpeed = 3f;
    [SerializeField] float m_runSpeed = 10f;
    [SerializeField] float m_moveSmooth = 0.15f;
    [SerializeField] bool m_toggleRun = true;
    [SerializeField] float m_walkStepDistance = 1f;
    [SerializeField] float m_runStepDistance = 2f;
    [SerializeField] WalkSfxType m_walkSfxType = WalkSfxType.Footsteps;
    [SerializeField] AudioClip[] m_footsteps;
    [SerializeField] AudioClip m_motorClip;
    [SerializeField] Vector2 m_motorPitchMinMax = new Vector2(0.5f, 1.5f);
    [SerializeField] float m_motorVolumeSmooth = 1f;

    [Header("Jump")]
    [SerializeField] bool m_allowJump;
    [SerializeField] float m_jumpForce = 5f;
    [SerializeField] LayerMask m_jumpMask;
    [SerializeField] float m_groundedRayDistance = 0.1f;

    [Header("Climb")]
    [SerializeField] float m_maxSlope = 40f;
    [SerializeField] float m_maxStepHeight = 0.3f;

    [Header("Sea blocking")]
    [SerializeField] LayerMask m_seaRayMask;
    [SerializeField] LayerMask m_groundRayMask;
    [SerializeField] float m_seaRayForwardDistance = 0.5f; 
    [SerializeField] float m_seaRayStartHeight = 1f;
    [SerializeField] float m_seaRayLength = 1.2f;
    [SerializeField] float m_groundRayLength = 1f;

    [Header("Free mode options")]
    [SerializeField] Collider m_collider;
    [SerializeField] float m_freeModeStartSpeed = 10f;
    [Range(1, 2)]
    [SerializeField] float m_speedMultiplier = 1.0f;

    private MouseLook m_mouseLook;
    private JoystickLook m_joystickLook;
    private GravityBody m_gravityBody;

    private Rigidbody m_rigidbody;

    private Vector3 m_moveAmount;
    private Vector3 m_moveDirection;
    private Vector3 m_smoothMoveVelocity;
    private bool m_grounded;
    private bool m_isRunning;
    private bool m_freeMode;
    private bool m_paused;
    private float m_speed;

    private AudioSource m_audioSource;
    private float m_stepCycle;
    private float m_nextStep;
    private float m_maxVolume;

    private Vector3 m_contact;
    private float m_slope;
    private Vector3 m_slopeNormal;
    private float m_slopeMovementDot;
    private float m_step;
    private bool m_canClimb;
    private bool m_allowFreeModeMovement;

    private Vector3 m_lastGrountContactPosition;
    private Quaternion m_lastGrountContactRotation;


    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();     
        m_audioSource = GetComponent<AudioSource>();
        m_mouseLook = GetComponent<MouseLook>();
        m_joystickLook = GetComponent<JoystickLook>();
        m_gravityBody = GetComponent<GravityBody>();

        if (m_collider == null)
            m_collider = GetComponentInChildren<Collider>();

        if (m_camera == null)
            m_camera = Camera.main.transform;

        if (m_walkSfxType == WalkSfxType.Motor && m_motorClip != null)
        {
            m_maxVolume = m_audioSource.volume;
            m_audioSource.clip = m_motorClip;
            m_audioSource.volume = 0f;
            m_audioSource.loop = true;
            m_audioSource.Play();
        }

        StartCoroutine(GetSprint());
        
        if (m_walkSfxType == WalkSfxType.Motor)
            StartCoroutine(PlayMotorAudio());
    }
	

	void Update()
    {
        bool useJoystick = GameController.UseJoystick;
        m_allowFreeModeMovement = GameController.FreeModeHidesPauseMenu || !m_paused;

        RotateView();

        float h = useJoystick
            ? Input.GetAxisRaw("Horizontal joystick")
            : Input.GetAxisRaw("Horizontal");

        float v = useJoystick
            ? Input.GetAxisRaw("Vertical joystick")
            : Input.GetAxisRaw("Vertical");

        if (m_freeMode)
        {
            float u = Input.GetAxisRaw("Elevation");

            if (Input.GetKeyDown(KeyCode.Z))
                m_speed /= 1.1f;

            if (Input.GetKeyDown(KeyCode.X))
                m_speed *= 1.1f;

            if (h != 0 || v != 0 || u != 0)
            {
                float multiplier = 1f + Time.unscaledDeltaTime * (m_speedMultiplier - 1f);
                m_speed *= multiplier;
            }
            else
                m_speed = m_freeModeStartSpeed;

            var freeMoveDirection = new Vector3(h, 0, v);

            if (!useJoystick || freeMoveDirection.magnitude > 1f)
                freeMoveDirection.Normalize();

            freeMoveDirection.y = u;

            //print(freeMoveDirection);

            if (m_allowFreeModeMovement)
                transform.Translate(freeMoveDirection * m_speed * Time.unscaledDeltaTime);

            return;
        }
        else
        {
            m_speed = m_isRunning ? m_runSpeed : m_walkSpeed;
            m_speed = m_grounded ? m_speed : 0.5f * m_walkSpeed;
        }

        var moveDirection = new Vector3(h, 0, v);

        if (!useJoystick || moveDirection.magnitude > 1f)
            moveDirection.Normalize();

        //print(moveDirection);

        var targetMoveAmount = moveDirection * m_speed;    

        m_moveAmount = Vector3.SmoothDamp(m_moveAmount, targetMoveAmount, ref m_smoothMoveVelocity, m_moveSmooth);  

        if (m_allowJump && m_grounded && Input.GetButtonDown("Jump"))
        {
            m_rigidbody.AddForce(transform.up * m_jumpForce, ForceMode.Impulse);
        }

        m_grounded = false;
        var ray = new Ray(transform.position + transform.up * 0.1f, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, m_groundedRayDistance, m_jumpMask))
        {
            m_grounded = true;
        }
	}


    void FixedUpdate()
    {
        if (m_freeMode)
            return;

        m_moveDirection = transform.TransformDirection(m_moveAmount);

        CheckForSlopesAndSteps();
        CheckForSea();
        
        m_rigidbody.MovePosition(m_rigidbody.position + m_moveDirection * Time.deltaTime);

        if (m_walkSfxType == WalkSfxType.Footsteps)
            ProgressStepCycle();
    }


    private void CheckForSlopesAndSteps()
    {
        Debug.DrawRay(transform.position + transform.up * 0.5f, m_moveDirection);
        m_slopeMovementDot = Vector3.Dot(m_slopeNormal, m_moveDirection);

        m_canClimb = false;

        if (m_slope > m_maxSlope && m_step < m_maxStepHeight)
        {
            float height = m_maxStepHeight - m_step;
            var rayStart = m_contact + height * transform.up;
            var ray = new Ray(rayStart, m_moveDirection.normalized);
            float distance = 2f * Mathf.Abs(height / Mathf.Tan(m_slope * Mathf.Deg2Rad));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distance))
            {
                m_canClimb = false;
                Debug.DrawRay(rayStart, m_moveDirection.normalized * distance, Color.red);
            }
            else
            {
                m_canClimb = true;
                Debug.DrawRay(rayStart, m_moveDirection.normalized * distance, Color.green);
            }
        }

        if (m_slope > m_maxSlope && m_slopeMovementDot < 0 && !m_canClimb)
        {
            m_slopeMovementDot = -m_slopeMovementDot;

            var tangent = Vector3.Cross(m_slopeNormal, transform.up).normalized;
            var dotMoveTangent = Vector3.Dot(m_moveDirection, tangent);

            m_moveDirection = tangent * dotMoveTangent;

            Debug.DrawRay(transform.position + transform.up * 0.5f, m_moveDirection, Color.cyan);
        }
    }


    private void CheckForSea()
    {
        var seaRayStart = transform.position + m_seaRayForwardDistance * m_moveDirection.normalized + transform.up * m_seaRayStartHeight;
        var seaRay = new Ray(seaRayStart, -transform.up);

        RaycastHit seaHit;
        if (Physics.Raycast(seaRay, out seaHit, m_seaRayLength, m_seaRayMask))
        {
            if (!seaHit.collider.CompareTag(Tags.WaterPart))
            {
                Debug.DrawRay(seaRayStart, -transform.up * m_seaRayLength, Color.green);
            }
            else if (seaHit.collider.CompareTag(Tags.WaterPart))
            { 
                Debug.DrawRay(seaRayStart, -transform.up * m_seaRayLength, Color.blue);

                var groundRayStart = seaRayStart - transform.up * m_seaRayStartHeight;
                var leftGroundRayDirection = Quaternion.AngleAxis(45f, transform.up) * (-m_moveDirection.normalized);
                var rightGroundRayDirection = Quaternion.AngleAxis(-45f, transform.up) * (-m_moveDirection.normalized);

                var leftGroundRay = new Ray(groundRayStart, leftGroundRayDirection);
                var rightGroundRay = new Ray(groundRayStart, rightGroundRayDirection);

                RaycastHit leftGroundHit;
                RaycastHit rightGroundHit;

                bool leftGroundRayImpact = Physics.Raycast(leftGroundRay, out leftGroundHit, m_groundRayLength, m_groundRayMask);
                bool rightGroundRayImpact = Physics.Raycast(rightGroundRay, out rightGroundHit, m_groundRayLength, m_groundRayMask);

                Debug.DrawRay(groundRayStart, leftGroundRayDirection * m_groundRayLength, leftGroundRayImpact ? Color.green : Color.red);
                Debug.DrawRay(groundRayStart, rightGroundRayDirection * m_groundRayLength, rightGroundRayImpact ? Color.green : Color.red);

                if (leftGroundRayImpact && rightGroundRayImpact)
                {
                    float leftDist = Vector3.Distance(transform.position, leftGroundHit.point);
                    float rightDist = Vector3.Distance(transform.position, rightGroundHit.point);

                    var validTarget = leftDist < rightDist ? leftGroundHit.point : rightGroundHit.point;

                    AdjustMoveDirection(validTarget);
                }
                else if (leftGroundRayImpact)
                {
                    AdjustMoveDirection(leftGroundHit.point);
                }
                else if (rightGroundRayImpact)
                {
                    AdjustMoveDirection(rightGroundHit.point);
                }
                else
                {
                    m_moveDirection = Vector3.zero;
                }
            }
        }
    }


    private void AdjustMoveDirection(Vector3 validTarget)
    {
        var validDirection = (validTarget - transform.position).normalized;
        m_moveDirection = validDirection * Vector3.Dot(m_moveDirection, validDirection);
    }


    void OnCollisionStay(Collision col)
    {
        if (!col.gameObject.CompareTag(Tags.WaterPart))
        {
            var contact = col.contacts[0];
            m_slopeNormal = contact.normal;
            m_contact = contact.point;

            Debug.DrawRay(contact.point, m_slopeNormal, Color.yellow);

            m_slope = Vector3.Angle(transform.up, m_slopeNormal);
            m_step = Vector3.Dot(m_contact - transform.position, transform.up);

            if (col.gameObject.CompareTag(Tags.Ground))
            {
                m_lastGrountContactPosition = contact.point;
                //m_lastGrountContactRotation = transform.rotation;
            }
        }
        else
        {
            transform.position = m_lastGrountContactPosition;
            //transform.rotation = m_lastGrountContactRotation;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(m_contact, 0.05f);
    }


    private void RotateView()
    {
        if ((m_paused && !m_freeMode) || !m_allowFreeModeMovement)
            return;

        if (GameController.UseJoystick)
        {
            float deltaTime = m_freeMode ? Time.unscaledDeltaTime : Time.deltaTime;
            m_joystickLook.LookRotation(transform, m_camera.transform, m_verticalLookMinMax, deltaTime);
        }
        else
            m_mouseLook.LookRotation(transform, m_camera.transform, m_verticalLookMinMax);
    }


    private IEnumerator GetSprint()
    {
        bool buttonPressedPreviously = false;

        while (true)
        {
            bool buttonPressed = Input.GetAxisRaw("Sprint") == 1f;

            if (!m_toggleRun)
                m_isRunning = buttonPressed;
            else
            {
                if (buttonPressed && !buttonPressedPreviously)
                    m_isRunning = !m_isRunning;

                buttonPressedPreviously = buttonPressed;
            }

            yield return null;
        }
    }


    private void ProgressStepCycle()
    {
        if (m_moveAmount.sqrMagnitude > 0)
        {
            float distanceTravelled = m_moveAmount.magnitude * Time.fixedDeltaTime;
            m_stepCycle += distanceTravelled;
        }

        if (!(m_stepCycle > m_nextStep))
        {
            return;
        }

        float stepDistance = m_isRunning ? m_runStepDistance : m_walkStepDistance;
        m_nextStep = m_stepCycle + stepDistance;

        PlayFootStepAudio();
    }


    private void PlayFootStepAudio()
    {
        if (!m_grounded)
            return;

        int n = Random.Range(0, m_footsteps.Length);
        m_audioSource.PlayOneShot(m_footsteps[n]);

        //if (transform.position.y > m_footstepUpperAltitudeThreshold)
        //{
        //    // pick & play a random footstep sound from the array,
        //    // excluding sound at index 0
        //    int n = Random.Range(1, m_FootstepSoundsHigh.Length);
        //    m_AudioSource.clip = m_FootstepSoundsHigh[n];
        //    m_AudioSource.PlayOneShot(m_AudioSource.clip);
        //    // move picked sound to index 0 so it's not picked next time
        //    m_FootstepSoundsHigh[n] = m_FootstepSoundsHigh[0];
        //    m_FootstepSoundsHigh[0] = m_AudioSource.clip;
        //}
        //else if (transform.position.y < m_footstepLowerAltitudeThreshold)
        //{
        //    int n = Random.Range(1, m_FootstepSoundsLow.Length);
        //    m_AudioSource.clip = m_FootstepSoundsLow[n];
        //    m_AudioSource.PlayOneShot(m_AudioSource.clip);
        //    // move picked sound to index 0 so it's not picked next time
        //    m_FootstepSoundsLow[n] = m_FootstepSoundsLow[0];
        //    m_FootstepSoundsLow[0] = m_AudioSource.clip;
        //}
        //else
        //{
        //    int n = Random.Range(1, m_FootstepSoundsMid.Length);
        //    m_AudioSource.clip = m_FootstepSoundsMid[n];
        //    m_AudioSource.PlayOneShot(m_AudioSource.clip);
        //    // move picked sound to index 0 so it's not picked next time
        //    m_FootstepSoundsMid[n] = m_FootstepSoundsMid[0];
        //    m_FootstepSoundsMid[0] = m_AudioSource.clip;
        //}
    }


    private IEnumerator PlayMotorAudio()
    {
        while (true)
        {
            float speed = m_moveDirection.magnitude;

            float volumeFrac = m_audioSource.volume / m_maxVolume;
            float frac = Mathf.Lerp(volumeFrac, speed / m_runSpeed, Time.unscaledDeltaTime * m_motorVolumeSmooth);
            m_audioSource.volume = m_maxVolume * frac;
            m_audioSource.pitch = Mathf.Lerp(m_motorPitchMinMax.x, m_motorPitchMinMax.y, frac);

            yield return null;
        }
    }


    public void FreeMode(bool active)
    {
        m_freeMode = active;

        if (m_gravityBody != null)
            m_gravityBody.useGravityAttractorGravity = !m_freeMode;

        if (m_gravityBody.useGravity)
            m_rigidbody.useGravity = !active;

        if (m_collider != null)
            m_collider.enabled = !m_freeMode;

        if (m_speed > 0 && m_freeMode)
            m_speed = m_freeModeStartSpeed;

        if (m_freeMode)
            m_rigidbody.velocity = Vector3.zero;

        var bodyRenderers = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < bodyRenderers.Length; i++)
        {
            bodyRenderers[i].enabled = !active;
        }      
    }


    private void OnPause()
    {
        m_paused = true;
    }


    private void OnUnpause()
    {
        m_paused = false;
    }


    void OnEnable()
    {
        EventManager.StartListening(StandardEventName.Pause, OnPause);
        EventManager.StartListening(StandardEventName.Unpause, OnUnpause);
    }


    void OnDisable()
    {
        m_moveAmount = Vector2.zero;
        m_moveDirection = Vector2.zero;
        EventManager.StopListening(StandardEventName.Pause, OnPause);
        EventManager.StopListening(StandardEventName.Unpause, OnUnpause);
    }
}
