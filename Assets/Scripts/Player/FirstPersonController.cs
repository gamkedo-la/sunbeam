using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Look")]
    [SerializeField] float m_mouseSensitivityX = 3f;
    [SerializeField] float m_mouseSensitivityY = 3f;

    [SerializeField] Vector2 m_verticalLookMinMan = new Vector2(-90f, 90f);

    [Header("Walk")]
    [SerializeField] float m_walkSpeed = 3f;
    [SerializeField] float m_runSpeed = 10f;
    [SerializeField] float m_moveSmooth = 0.15f;
    [SerializeField] bool m_toggleRun = true;

    [Header("Jump")]
    [SerializeField] float m_jumpForce = 5f;
    [SerializeField] LayerMask m_jumpMask;
    [SerializeField] float m_groundedRayDistance = 0.1f;


    private Rigidbody m_rigidbody;
    private Transform m_camera;
    private float m_verticalLookRotation;
    private Vector3 m_moveAmount;
    private Vector3 m_smoothMoveVelocity;
    private bool m_grounded;
    private bool m_isRunning;


	void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_camera = Camera.main.transform;

        StartCoroutine(GetSprint());
    }
	

	void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up * mouseX * m_mouseSensitivityX);
        m_verticalLookRotation += mouseY * m_mouseSensitivityY;
        m_verticalLookRotation = Mathf.Clamp(m_verticalLookRotation, m_verticalLookMinMan.x, m_verticalLookMinMan.y);
        m_camera.localEulerAngles = Vector3.left * m_verticalLookRotation;

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
