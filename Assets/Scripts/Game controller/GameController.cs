using UnityEngine;
using System;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static bool AllowCheatMode = true;

    private Camera m_mainCamera;
    private bool m_freeCameraEnabled = false;
    private bool m_paused = false;
    private float m_timeScale;

    private FirstPersonController m_firstPersonController;
    private Vector3 m_lastCameraPosition;
    private Quaternion m_lastCameraRotation;


    void Awake()
    {
        m_mainCamera = Camera.main;
        m_timeScale = Time.timeScale;

        OnUnpause();
    }


    void Start()
    {
        m_firstPersonController = m_mainCamera.GetComponentInParent<FirstPersonController>();

        StartCoroutine(CheckForAxisInput("Free camera", ToggleFreeCamera, false));
        StartCoroutine(CheckForAxisInput("Pause", TogglePause, true));

        // This may need to be done after Start if anything ever gets set up in other Start methods that needs it to be unpaused,
        // but so far it looks to be fine to do this here.
        EventManager.TriggerEvent(StandardEventName.Pause);
    }


    void Update()
    {
        //if (m_paused && Input.GetKeyDown(KeyCode.Escape))
        //    QuitGame();

        // Capture the mouse just in case it gets detached for some reason
        if (!m_paused && Input.GetMouseButton(0))
            OnUnpause();
    }


    private IEnumerator CheckForAxisInput(string axisName, Action action, bool ignoreCheatModeFlag)
    {
        bool buttonPressedPreviously = false;

        while (true)
        {
            if (ignoreCheatModeFlag || AllowCheatMode)
            {
                bool buttonPressed = Input.GetAxisRaw(axisName) == 1f;

                if (buttonPressed && !buttonPressedPreviously)
                {
                    action.Invoke();
                }

                buttonPressedPreviously = buttonPressed;
            }

            yield return null;
        }
    }


    private void TogglePause()
    {
        //if (m_freeCameraEnabled)
        //{
            m_paused = !m_paused;
            SetTimeScale();
        //}
    }


    private void ToggleFreeCamera()
    {
        m_freeCameraEnabled = !m_freeCameraEnabled;
        //m_paused = m_freeCameraEnabled;

        //if (m_freeCameraEnabled)
        //{
        //    m_lastCameraPosition = m_mainCamera.transform.localPosition;
        //    m_lastCameraRotation = m_mainCamera.transform.localRotation;
        //    m_mainCamera.transform.parent = null;
        //    var rotation = m_lastCameraRotation.eulerAngles;
        //    rotation.z = 0;
        //    m_mainCamera.transform.rotation.eulerAngles.Set(rotation.x, rotation.y, rotation.z);
        //}
        //else
        //{
        //    m_mainCamera.transform.parent = m_cameraParent;
        //    m_mainCamera.transform.localPosition = m_lastCameraPosition;
        //    m_mainCamera.transform.localRotation = m_lastCameraRotation;
        //}

        SetTimeScale();

        m_firstPersonController.FreeMode(m_freeCameraEnabled);

        //EventManager.TriggerEvent(EventNames.ToggleFreeCamera, m_freeCameraEnabled);
    }


    private void SetTimeScale()
    {
        if (m_freeCameraEnabled)
        {
            if (m_paused)
                OnPause();
            else
            {
                OnUnpause();
            }
        }
        else
        {
            if (m_paused)
            {
                EventManager.TriggerEvent(StandardEventName.Pause);
            }
            else
            {
                EventManager.TriggerEvent(StandardEventName.Unpause);
            }
        }
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    private void OnPause()
    {
        m_paused = true;
        Time.timeScale = 0;
        //print("Pause");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    private void OnUnpause()
    {
        m_paused = false;
        Time.timeScale = m_timeScale;
        //print("Unpause");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void OnEnable()
    {
        EventManager.StartListening(StandardEventName.Pause, OnPause);
        EventManager.StartListening(StandardEventName.Unpause, OnUnpause);
    }


    void OnDisable()
    {
        EventManager.StopListening(StandardEventName.Pause, OnPause);
        EventManager.StopListening(StandardEventName.Unpause, OnUnpause);

        Time.timeScale = m_timeScale;
    }
}
