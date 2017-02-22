using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MovePlayerToStartPointTool))]
public class MovePlayerToStartPointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var movePlayerTool = (MovePlayerToStartPointTool) target;

        DrawDefaultInspector();

        if (GUILayout.Button("Move player to start point"))
            movePlayerTool.MovePlayerToStartPoint();
    }
}
