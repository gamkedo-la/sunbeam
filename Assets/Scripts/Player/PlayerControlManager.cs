using UnityEngine;
using System.Collections;

[RequireComponent(typeof(FirstPersonController))]
public class PlayerControlManager : MonoBehaviour
{
    [SerializeField] float m_transitionTime = 1f;
    [SerializeField] AnimationCurve m_transitionCurve;

    private FirstPersonController m_playerController;
    private Transform m_camera;
    private Transform m_cameraAnchor;
    //private Transform m_newCameraPoint;
    private float m_totalDistance;
    private float m_transitionTimeToTarget;


    void Awake()
    {
        m_playerController = GetComponent<FirstPersonController>();
        m_camera = Camera.main.transform;
        m_cameraAnchor = m_camera.parent;
    }


    private void MirrorActivated(Transform newCameraPoint, IActivatable activatable)
    {
        //print("Mirror activated");
        //m_newCameraPoint = newCameraPoint;
        m_totalDistance = Vector3.Distance(m_cameraAnchor.position, newCameraPoint.position);

        m_playerController.enabled = false;
        m_camera.parent = null;
        
        StopAllCoroutines();
        StartCoroutine(MoveCameraTo(newCameraPoint));

        //var activatable = newCameraPoint.GetComponentInParent<IActivatable>();

        if (activatable != null)
            StartCoroutine(ActivateAfterTransition(activatable));
    }


    private IEnumerator MoveCameraTo(Transform target)
    {
        var startPosition = m_camera.position;
        var startRotation = m_camera.rotation;

        float distanceToTarget = Vector3.Distance(startPosition, target.position);
        m_transitionTimeToTarget = m_transitionTime * distanceToTarget / m_totalDistance;

        //print("Transition time: " + m_transitionTimeToTarget);

        float time = 0;
        float startTime = Time.time;
        
        while (time < m_transitionTimeToTarget)
        {
            time = Time.time - startTime;
            float t = time / m_transitionTimeToTarget;
            t = m_transitionCurve.Evaluate(t);

            m_camera.position = Vector3.Lerp(startPosition, target.position, t);
            m_camera.rotation = Quaternion.Lerp(startRotation, target.rotation, t);

            yield return null;
        }

        m_camera.position = target.position;
        m_camera.rotation = target.rotation;
    }


    private IEnumerator ActivateAfterTransition(IActivatable activatable)
    {
        yield return new WaitForSeconds(m_transitionTimeToTarget);

        activatable.Activate();
    }


    private void MirrorDeactivated()
    {
        //print("Mirror deactivated");
        StopAllCoroutines();
        StartCoroutine(MoveCameraTo(m_cameraAnchor));
        StartCoroutine(EnablePlayerControl());
    }


    private IEnumerator EnablePlayerControl()
    {
        yield return new WaitForSeconds(m_transitionTimeToTarget);

        m_camera.parent = m_cameraAnchor;
        m_playerController.enabled = true;
    }


    private void TriggerClosingCinematic()
    {
        StopAllCoroutines();
        m_playerController.enabled = false;
    }


    void OnEnable()
    {
        EventManager.StartListening(TransformEventName.PropActivated, MirrorActivated);
        EventManager.StartListening(StandardEventName.PropDeactivated, MirrorDeactivated);
        EventManager.StartListening(StandardEventName.TriggerClosingCinematic, TriggerClosingCinematic);
    }


    void OnDisable()
    {
        EventManager.StopListening(TransformEventName.PropActivated, MirrorActivated);
        EventManager.StopListening(StandardEventName.PropDeactivated, MirrorDeactivated);
        EventManager.StopListening(StandardEventName.TriggerClosingCinematic, TriggerClosingCinematic);
    }
}
