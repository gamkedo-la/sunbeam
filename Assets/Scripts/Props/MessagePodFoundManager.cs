using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagePodFoundManager : MonoBehaviour
{
    private bool m_found;


    public void SetFoundToTrue()
    {
        m_found = true;
    }


    public bool Found
    {
        get { return m_found; }
    }
}
