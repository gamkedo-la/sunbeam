using UnityEngine;
using UnityEditor;
using System.Collections;

public class RandomRotation : MonoBehaviour
{
    [MenuItem("Planet/Apply random rotation %.")]
    static void RotateTransforms()
    {
        Transform[] transforms = Selection.transforms;

        foreach (Transform myTransform in transforms)
        {
            RotateTransform(myTransform);
        }
    }


    private static void RotateTransform(Transform myTransform)
    {
        if (myTransform.childCount == 0 || myTransform.GetChild(0).GetComponent<MeshRenderer>() != null)
        {
            myTransform.Rotate(Vector3.up, Random.Range(0f, 360f), Space.Self);
        }
        else
        {
            for (int i = 0; i < myTransform.childCount; i++)
            {
                RotateTransform(myTransform.GetChild(i));
            }
        }
    }
}
