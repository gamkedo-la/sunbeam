using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshManager : MonoBehaviour
{
    private static MeshManager s_Instance = null;

    public enum GripMesh
    {
        CUBE,
        SPHERE,
    }
    public enum CopyMesh
    {
        ORIGINAL,
        COPY
    }
    public CopyMesh modifyMesh = CopyMesh.ORIGINAL;
    public string postfixName = "_Edit";
    public GripMesh vertexDisplay = GripMesh.SPHERE;
    public Color defaultColor = Color.red;
    public Color selectedColor = Color.blue;
    public float currentSize = .1f;

    public static MeshManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                s_Instance = FindObjectOfType(typeof(MeshManager)) as MeshManager;
            }

            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                GameObject obj = new GameObject("MeshManager");
                s_Instance = obj.AddComponent(typeof(MeshManager)) as MeshManager;
            }

            return s_Instance;
        }
    }

    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit()
    {
        s_Instance = null;
    }
}
