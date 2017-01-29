using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class PlayerAngleChecker : MonoBehaviour
{
    [SerializeField] float m_checkInterval = 0.1f;

    private WaitForSeconds m_interval;
    private Transform m_player;
    private MeshRenderer m_meshRenderer;


    void Awake()
    {
        m_interval = new WaitForSeconds(m_checkInterval);

        m_player = Camera.main.transform;

        m_meshRenderer = GetComponent<MeshRenderer>();
    }


    private IEnumerator CheckPlayerAngleCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(0f, m_checkInterval));

        while (true)
        {
            CheckPlayerAngle();

            yield return m_interval;
        }
    }


    private void CheckPlayerAngle()
    {
        var playerDirection = (m_player.position - transform.position).normalized;

        float dot = Vector3.Dot(transform.up, playerDirection);

        m_meshRenderer.enabled = dot > 0;
    }


    void OnEnable()
    {
        StartCoroutine(CheckPlayerAngleCoroutine());
    }


    void OnDisable()
    {
        StopAllCoroutines();
    }
}
