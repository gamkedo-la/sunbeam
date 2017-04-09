using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class DoorOpener : MonoBehaviour
{
    [SerializeField] bool m_openOnStart;

    private Animator m_animator;


    void Awake()
    {
        m_animator = GetComponent<Animator>();

        if (m_openOnStart)
            OpenDoor();
    }


    public void OpenDoor()
    {
        m_animator.SetTrigger("Open");
    }
}
