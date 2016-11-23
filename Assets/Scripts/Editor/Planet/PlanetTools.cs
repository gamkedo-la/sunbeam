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
    static void AlignTransformsWithPlanetSurface()
    {
        Transform[] transforms = Selection.transforms;

        foreach (Transform myTransform in transforms)
        {
            AlignTransformWithPlanetSurface(myTransform);
        }
    }


    private static void AlignTransformWithPlanetSurface(Transform myTransform)
    {
        if (myTransform.childCount == 0 || myTransform.GetChild(0).GetComponent<MeshRenderer>() != null)
        {
            var targetDirection = myTransform.position.normalized;
            var transformUp = myTransform.up;
            myTransform.rotation = Quaternion.FromToRotation(transformUp, targetDirection) * myTransform.rotation;
        }
        else
        {
            for (int i = 0; i < myTransform.childCount; i++)
            {
                AlignTransformWithPlanetSurface(myTransform.GetChild(i));
            }
        }
    }
}
