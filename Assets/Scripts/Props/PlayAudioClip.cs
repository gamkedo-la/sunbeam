using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayAudioClip : MonoBehaviour
{
    private AudioSource m_audioSource;


    void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
    }


    public void PlayAudio()
    {
        m_audioSource.Play();
    }
}
