using UnityEngine;
using UnityEditor;
using System.Collections;

public class RotateEnvironment : MonoBehaviour
{
    [MenuItem("Custom/Reset environment rotation %w")]
    static void ResetRotation()
    {
        var environment = GameObject.FindGameObjectWithTag(Tags.Environment);

        if (environment != null)
        {
            print("Reset environment rotation");
            environment.transform.rotation = Quaternion.identity;
        }
        else
            print("Couldn't find environment object");
    }
}
