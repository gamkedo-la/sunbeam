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
        OnUnpause();

        m_mainCamera = Camera.main;
        m_timeScale = Time.timeScale;
    }


    void Start()
    {
        m_firstPersonController = m_mainCamera.GetComponentInParent<FirstPersonController>();

        StartCoroutine(CheckForAxisInput("Free camera", ToggleFreeCamera));
        StartCoroutine(CheckForAxisInput("Pause", TogglePause));
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            QuitGame();
    }


    private IEnumerator CheckForAxisInput(string axisName, Action action)
    {
        bool buttonPressedPreviously = false;

        while (true)
        {
            if (AllowCheatMode)
            {
                bool buttonPressed = Input.GetAxisRaw(axisName) == 1f;

                if (buttonPressed && !buttonPressedPreviously)
                    action.Invoke();

                buttonPressedPreviously = buttonPressed;
            }

            yield return null;
        }
    }


    private void TogglePause()
    {
        if (m_freeCameraEnabled)
        {
            m_paused = !m_paused;
            SetTimeScale();
        }
    }


    private void ToggleFreeCamera()
    {
        m_freeCameraEnabled = !m_freeCameraEnabled;
        m_paused = m_freeCameraEnabled;

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
        Time.timeScale = m_paused ? 0 : m_timeScale;
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
        Time.timeScale = 0;
        //print("Pause");
    }


    private void OnUnpause()
    {
        Time.timeScale = 1;
        //print("Unpause");
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
