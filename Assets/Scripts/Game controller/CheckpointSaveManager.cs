using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSaveManager : MonoBehaviour
{
    private Transform m_player;


    void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag(Tags.Player).transform;
    }


    void OnTriggerEnter(Collider col)
    {
        PlayerPrefs.SetString("Checkpoint", name);
    }


    private void LoadCheckpointState()
    {
        string checkPoint = PlayerPrefs.GetString("Checkpoint", "None");

        if (checkPoint == name)
        {
            m_player.position = transform.position;
            m_player.rotation = transform.rotation;
        }
    }


    private void DeleteCheckpointState()
    {
        PlayerPrefs.DeleteKey(name);
    }


    void OnEnable()
    {
        EventManager.StartListening(StandardEventName.LoadSaveGame, LoadCheckpointState);
        EventManager.StartListening(StandardEventName.DeleteSaveData, DeleteCheckpointState);
    }


    void OnDisable()
    {
        EventManager.StopListening(StandardEventName.LoadSaveGame, LoadCheckpointState);
        EventManager.StopListening(StandardEventName.DeleteSaveData, DeleteCheckpointState);
    }
}
