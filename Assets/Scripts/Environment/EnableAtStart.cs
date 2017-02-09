using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableAtStart : MonoBehaviour
{
    [SerializeField] GameObject[] m_objectsToEnable = new GameObject[0];
    

	void Awake()
    {
		for (int i = 0; i < m_objectsToEnable.Length; i++)
        {
            m_objectsToEnable[i].SetActive(true);
        }
	}
}
