using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MouseLook))]
[RequireComponent(typeof(JoystickLook))]
public class FirstPersonController : MonoBehaviour
{
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
    [SerializeField] AudioClip[] m_footsteps;

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
    [SerializeField] float m_freeModeStartSpeed = 10f;
    [Range(1, 2)]
    [SerializeField] float m_speedMultiplier = 1.0f;

    private MouseLook m_mouseLook;
    private JoystickLook m_joystickLook;
    private GravityBody m_gravityBody;

    private Rigidbody m_rigidbody;
    private Collider m_collider;

    private Vector3 m_moveAmount;
    private Vector3 m_moveDirection;
    private Vector3 m_smoothMoveVelocity;
    private bool m_grounded;
    private bool m_isRunning;
    private bool m_useJoystickLook;
    private bool m_freeMode;
    private float m_speed;

    private AudioSource m_audioSource;
    private float m_stepCycle;
    private float m_nextStep;

    private Vector3 m_contact;
    private float m_slope;
    private Vector3 m_slopeNormal;
    private float m_slopeMovementDot;
    private float m_step;
    private bool m_canClimb;


    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();
        m_audioSource = GetComponent<AudioSource>();

        if (m_camera == null)
            m_camera = Camera.main.transform;

        m_mouseLook = GetComponent<MouseLook>();
        m_joystickLook = GetComponent<JoystickLook>();
        m_gravityBody = GetComponent<GravityBody>();

        StartCoroutine(GetSprint());
        StartCoroutine(CheckForJoysticks());
    }
	

	void Update()
    {
        RotateView();

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

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
      
            var freeMoveDirection = new Vector3(h, u, v).normalized;

            transform.Translate(freeMoveDirection * m_speed * Time.unscaledDeltaTime);

            return;
        }
        else
        {
            m_speed = m_isRunning ? m_runSpeed : m_walkSpeed;
        }

        var moveDirection = new Vector3(h, 0, v).normalized;
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
        var contact = col.contacts[0];
        m_slopeNormal = contact.normal;
        m_contact = contact.point;

        Debug.DrawRay(contact.point, m_slopeNormal, Color.yellow);

        m_slope = Vector3.Angle(transform.up, m_slopeNormal);
        m_step = Vector3.Dot(m_contact - transform.position, transform.up);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(m_contact, 0.05f);
    }


    private void RotateView()
    {
        if (m_useJoystickLook)
        {
            float deltaTime = m_freeMode ? Time.unscaledDeltaTime : Time.deltaTime;
            m_joystickLook.LookRotation(transform, m_camera.transform, m_verticalLookMinMax, deltaTime);
        }
        else
            m_mouseLook.LookRotation(transform, m_camera.transform, m_verticalLookMinMax);
    }


    private IEnumerator CheckForJoysticks()
    {
        while (true)
        {
            var joystickNames = Input.GetJoystickNames();

            bool validJoystick = false;

            for (int i = 0; i < joystickNames.Length; i++)
            {
                validJoystick = validJoystick || !string.IsNullOrEmpty(joystickNames[i]);
                //print(string.Format("{0} Joystick {1}: {2}", Time.time, i + 1, joystickNames[i]));
            }

            if (validJoystick)
            {
                if (!m_useJoystickLook)
                    EventManager.TriggerEvent(StandardEventName.ActivateJoypadControls);

                m_useJoystickLook = true;
            }
            else
            {
                if (m_useJoystickLook)
                    EventManager.TriggerEvent(StandardEventName.ActivateMouseControls);

                m_useJoystickLook = false;
            }

            yield return new WaitForSeconds(1f);
        }
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


    public void FreeMode(bool active)
    {
        m_freeMode = active;

        if (m_gravityBody != null)
            m_gravityBody.useGravityAttractorGravity = !m_freeMode;

        if (m_collider != null)
            m_collider.enabled = !m_freeMode;

        if (m_speed > 0 && m_freeMode)
            m_speed = m_freeModeStartSpeed;

        if (m_freeMode)
            m_rigidbody.velocity = Vector3.zero;

        var bodyRenderer = GetComponentInChildren<MeshRenderer>();

        if (bodyRenderer != null)
            bodyRenderer.enabled = !active;
    }
}
