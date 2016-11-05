using UnityEngine;
using System.Collections;

[RequireComponent(typeof(FirstPersonController))]
public class PlayerControlManager : MonoBehaviour
{
    [SerializeField] float m_transitionTime = 1f;

    private FirstPersonController m_playerController;
    private Transform m_camera;
    private Transform m_cameraAnchor;


    void Awake()
    {
        m_playerController = GetComponent<FirstPersonController>();
        m_camera = Camera.main.transform;
        m_cameraAnchor = m_camera.parent;
    }


    private void MirrorActivated(Transform newCameraPoint)
    {
        m_playerController.enabled = false;

        m_camera.parent = null;
        m_camera.position = newCameraPoint.position;
        m_camera.rotation = newCameraPoint.rotation;
    }


    private void MirrorDeactivated()
    {
        m_camera.position = m_cameraAnchor.position;
        m_camera.rotation = m_cameraAnchor.rotation;
        m_camera.parent = m_cameraAnchor;

        m_playerController.enabled = true;
    }


    void OnEnable()
    {
        EventManager.StartListening(TransformEventName.MirrorActivated, MirrorActivated);
        EventManager.StartListening(StandardEventName.MirrorDeactivated, MirrorDeactivated);
    }


    void OnDisable()
    {
        EventManager.StopListening(TransformEventName.MirrorActivated, MirrorActivated);
        EventManager.StopListening(StandardEventName.MirrorDeactivated, MirrorDeactivated);
    }
}
