using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] GameObject[] m_cameras;
    

    void Awake()
    {
        if (m_cameras.Length >= 1)
        {
            TurnAllOff();
            m_cameras[0].SetActive(true);
        }
    }


	void Update()
    {
		if (Input.GetKeyDown(KeyCode.Alpha1) && m_cameras.Length >= 1)
        {
            TurnAllOff();
            m_cameras[0].SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && m_cameras.Length >= 2)
        {
            TurnAllOff();
            m_cameras[1].SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && m_cameras.Length >= 3)
        {
            TurnAllOff();
            m_cameras[2].SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && m_cameras.Length >= 4)
        {
            TurnAllOff();
            m_cameras[3].SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) && m_cameras.Length >= 5)
        {
            TurnAllOff();
            m_cameras[4].SetActive(true);
        }
    }


    private void TurnAllOff()
    {
        for (int i = 0; i < m_cameras.Length; i++)
        {
            m_cameras[i].SetActive(false);
        }
    }
}
