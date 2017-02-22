using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerToStartPointTool : MonoBehaviour
{
    private Transform m_player;


    public void MovePlayerToStartPoint()
    {
        var playerObject = GameObject.FindGameObjectWithTag(Tags.Player);

        if (playerObject != null)
            m_player = playerObject.transform;

        if (m_player == null)
        {
            print("No player found");
            return;
        }

        m_player.position = transform.position;
        m_player.rotation = transform.rotation;
    }
}
