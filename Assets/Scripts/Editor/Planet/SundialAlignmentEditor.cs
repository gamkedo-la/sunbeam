using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SundialAlignmentTool))]
public class SundialAlignmentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var alignmentTool = (SundialAlignmentTool) target;

        DrawDefaultInspector();

        if (GUILayout.Button("Align sundial"))
            alignmentTool.Align();
    }
}
