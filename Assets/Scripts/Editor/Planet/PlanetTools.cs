using UnityEngine;
using UnityEditor;
using System.Collections;

public class PlanetTools : MonoBehaviour
{
    [MenuItem("Planet/Reset environment rotation %w")]
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


    [MenuItem("Planet/Align with planet surface %#y")]
    static void AlignWithGround()
    {
        Transform[] transforms = Selection.transforms;

        foreach (Transform myTransform in transforms)
        {
            print("Aligning " + myTransform.name + " with planet surface");

            var targetDirection = myTransform.position.normalized;
            var cloudUp = myTransform.up;
            myTransform.rotation = Quaternion.FromToRotation(cloudUp, targetDirection) * myTransform.rotation;
        }
    }
}
