using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatControls : MonoBehaviour
{
    [SerializeField] string m_skyRotationAxisName = "Sky rotation";
    [SerializeField] float m_skyRotationSpeed = 30f;
    [SerializeField] Transform[] m_spawnPoints;

    private Transform m_player;
    private Transform m_sky;


    void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag(Tags.Player).transform;
        m_sky = GameObject.FindGameObjectWithTag(Tags.Sky).transform;
    }


	void Update()
    {
        if (!GameController.AllowCheatMode)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1) && m_spawnPoints.Length > 0)
        {
            m_player.position = m_spawnPoints[0].position;
            m_player.rotation = m_spawnPoints[0].rotation;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && m_spawnPoints.Length > 1)
        {
            m_player.position = m_spawnPoints[1].position;
            m_player.rotation = m_spawnPoints[1].rotation;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && m_spawnPoints.Length > 2)
        {
            m_player.position = m_spawnPoints[2].position;
            m_player.rotation = m_spawnPoints[2].rotation;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && m_spawnPoints.Length > 3)
        {
            m_player.position = m_spawnPoints[3].position;
            m_player.rotation = m_spawnPoints[3].rotation;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) && m_spawnPoints.Length > 4)
        {
            m_player.position = m_spawnPoints[4].position;
            m_player.rotation = m_spawnPoints[4].rotation;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6) && m_spawnPoints.Length > 5)
        {
            m_player.position = m_spawnPoints[5].position;
            m_player.rotation = m_spawnPoints[5].rotation;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7) && m_spawnPoints.Length > 6)
        {
            m_player.position = m_spawnPoints[6].position;
            m_player.rotation = m_spawnPoints[6].rotation;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8) && m_spawnPoints.Length > 7)
        {
            m_player.position = m_spawnPoints[7].position;
            m_player.rotation = m_spawnPoints[7].rotation;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9) && m_spawnPoints.Length > 8)
        {
            m_player.position = m_spawnPoints[8].position;
            m_player.rotation = m_spawnPoints[8].rotation;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0) && m_spawnPoints.Length > 9)
        {
            m_player.position = m_spawnPoints[9].position;
            m_player.rotation = m_spawnPoints[9].rotation;
        }
        else if (Input.GetKeyDown(KeyCode.LeftBracket) && m_spawnPoints.Length > 10)
        {
            m_player.position = m_spawnPoints[10].position;
            m_player.rotation = m_spawnPoints[10].rotation;
        }
        else if (Input.GetKeyDown(KeyCode.RightBracket) && m_spawnPoints.Length > 11)
        {
            m_player.position = m_spawnPoints[11].position;
            m_player.rotation = m_spawnPoints[11].rotation;
        }

        float skyRotation = Input.GetAxis(m_skyRotationAxisName);

        m_sky.Rotate(Vector3.up * skyRotation * m_skyRotationSpeed * Time.deltaTime, Space.Self);
    }
}
