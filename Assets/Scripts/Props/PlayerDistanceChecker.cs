using UnityEngine;
using System.Collections;

public class PlayerDistanceChecker : MonoBehaviour
{
    [SerializeField] float m_playerDistanceThreshold = 50f;
    [SerializeField] GameObject[] m_objectsToManage;
	[SerializeField] float m_distanceCheckInterval = 0.5f;

    private WaitForSeconds m_interval;
    private Transform m_player;
    private float m_previousPlayerDistance;


    void Awake()
    {
        m_interval = new WaitForSeconds(m_distanceCheckInterval);

        var playerObject = GameObject.FindGameObjectWithTag(Tags.Player);

        if (playerObject != null)
            m_player = playerObject.transform;
    }

	
    void Start()
    {
        if (m_player == null)
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
        float playerDistance = Vector3.Distance(transform.position, m_player.position);

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
}
