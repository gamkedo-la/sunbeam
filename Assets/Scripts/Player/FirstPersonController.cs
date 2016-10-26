using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MouseLook))]
[RequireComponent(typeof(JoystickLook))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Look")]
    [SerializeField] Vector2 m_verticalLookMinMax = new Vector2(-90f, 90f);

    [Header("Walk")]
    [SerializeField] float m_walkSpeed = 3f;
    [SerializeField] float m_runSpeed = 10f;
    [SerializeField] float m_moveSmooth = 0.15f;
    [SerializeField] bool m_toggleRun = true;

    [Header("Jump")]
    [SerializeField] float m_jumpForce = 5f;
    [SerializeField] LayerMask m_jumpMask;
    [SerializeField] float m_groundedRayDistance = 0.1f;

    private MouseLook m_mouseLook;
    private JoystickLook m_joystickLook;

    private Rigidbody m_rigidbody;
    private Transform m_camera;
    private Vector3 m_moveAmount;
    private Vector3 m_smoothMoveVelocity;
    private bool m_grounded;
    private bool m_isRunning;
    private bool m_useJoystickLook;


	void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_camera = Camera.main.transform;

        m_mouseLook = GetComponent<MouseLook>();
        m_joystickLook = GetComponent<JoystickLook>();

        StartCoroutine(GetSprint());
        StartCoroutine(CheckForJoysticks());
    }
	

	void Update()
    {
        RotateView();

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        var moveDirection = new Vector3(h, 0, v).normalized;
        float speed = m_isRunning ? m_runSpeed : m_walkSpeed;
        var targetMoveAmount = moveDirection * speed;
        m_moveAmount = Vector3.SmoothDamp(m_moveAmount, targetMoveAmount, ref m_smoothMoveVelocity, m_moveSmooth);

        if (m_grounded && Input.GetButtonDown("Jump"))
        {
            m_rigidbody.AddForce(transform.up * m_jumpForce, ForceMode.Impulse);
        }

        m_grounded = false;
        var ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, m_groundedRayDistance, m_jumpMask))
        {
            m_grounded = true;
        }
	}


    void FixedUpdate()
    {
        m_rigidbody.MovePosition(m_rigidbody.position + transform.TransformDirection(m_moveAmount) * Time.deltaTime);
    }


    private void RotateView()
    {
        if (m_useJoystickLook)
            m_joystickLook.LookRotation(transform, m_camera.transform, m_verticalLookMinMax);
        else
            m_mouseLook.LookRotation(transform, m_camera.transform, m_verticalLookMinMax);
    }


    private IEnumerator CheckForJoysticks()
    {
        while (true)
        {
            var joystickNames = Input.GetJoystickNames();

            //for (int i = 0; i < joystickNames.Length; i++)
            //    print(string.Format("{0} Joystick {1}: {2}", Time.time, i + 1, joystickNames[i]));

            if (joystickNames.Length >= 1 && !string.IsNullOrEmpty(joystickNames[0]))
                m_useJoystickLook = true;
            else
                m_useJoystickLook = false;

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
}
