using UnityEngine;
using System.Collections;

public class PlayerDistanceChecker : MonoBehaviour
{
    [SerializeField] float m_playerDistanceThreshold = 40f;
    [SerializeField] GameObject[] m_objectsToManage;
	[SerializeField] float m_distanceCheckInterval = 0.5f;
    //[SerializeField] bool m_checkLineOfSight;
    //[SerializeField] LayerMask m_lineOfSightMask;

    private WaitForSeconds m_interval;
    private Transform m_camera;
    private float m_previousPlayerDistance;


    void Awake()
    {
        m_interval = new WaitForSeconds(m_distanceCheckInterval);

        m_camera = Camera.main.transform;
    }

	
    void Start()
    {
        if (m_camera == null)
            return;

        CheckPlayerDistance();

        StartCoroutine(CheckPlayerDistanceCoroutine());
    }


	
    private IEnumerator CheckPlayerDistanceCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(0f, m_distanceCheckInterval));

        while (true)
        {
            CheckPlayerDistance();

            yield return m_interval;
        }
    }


    private void CheckPlayerDistance()
    {
        float playerDistance = Vector3.Distance(transform.position, m_camera.position);

        if (playerDistance > m_playerDistanceThreshold
            && m_previousPlayerDistance <= m_playerDistanceThreshold)
        {
            for (int i = 0; i < m_objectsToManage.Length; i++)
                m_objectsToManage[i].SetActive(false);
        }
        else if (playerDistance <= m_playerDistanceThreshold
            && m_previousPlayerDistance > m_playerDistanceThreshold)
        {
            for (int i = 0; i < m_objectsToManage.Length; i++)
                m_objectsToManage[i].SetActive(true);
        }

        m_previousPlayerDistance = playerDistance;
    }


    private void SwitchCamera(Transform camera, IActivatable activatable)
    {
        m_camera = camera;
    }


    void OnEnable()
    {
        EventManager.StartListening(TransformEventName.CameraActivated, SwitchCamera);
    }


    void OnDisable()
    {
        EventManager.StopListening(TransformEventName.CameraActivated, SwitchCamera);
    }
}
