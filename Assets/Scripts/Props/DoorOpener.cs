using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class DoorOpener : MonoBehaviour
{
    private Animator m_animator;


    void Awake()
    {
        m_animator = GetComponent<Animator>();
    }


    public void OpenDoor()
    {
        m_animator.SetTrigger("Open");
    }
}
