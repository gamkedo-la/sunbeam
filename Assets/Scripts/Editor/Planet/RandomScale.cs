using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RandomScale : MonoBehaviour
{

    [MenuItem("Planet/Apply random scale %m")]
    static void ScaleTransforms()
    {
        Transform[] transforms = Selection.transforms;

        foreach (Transform myTransform in transforms)
        {
            ScaleTransform(myTransform);
        }
    }


    private static void ScaleTransform(Transform myTransform)
    {
        if (myTransform.childCount == 0 || myTransform.GetChild(0).GetComponent<MeshRenderer>() != null)
        {
            float scale = Random.Range(0.95f, 1.0526f);
            myTransform.localScale = myTransform.localScale * scale;
        }
        else
        {
            for (int i = 0; i < myTransform.childCount; i++)
            {
                ScaleTransform(myTransform.GetChild(i));
            }
        }
    }
}
