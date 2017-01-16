using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Randomization : MonoBehaviour
{
    [MenuItem("Planet/Apply random rotation %.", false, 51)]
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
        if (myTransform.childCount == 0 
            || myTransform.GetChild(0).GetComponent<MeshRenderer>() != null
            || myTransform.GetComponent<PlanetAlignFlag>() != null)
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


    [MenuItem("Planet/Apply random scale %m", false, 52)]
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
        if (myTransform.childCount == 0
            || myTransform.GetChild(0).GetComponent<MeshRenderer>() != null
            || myTransform.GetComponent<PlanetAlignFlag>() != null)
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


    [MenuItem("Planet/Apply random scatter %,", false, 53)]
    static void ScatterTransforms()
    {
        Transform[] transforms = Selection.transforms;

        foreach (Transform myTransform in transforms)
        {
            ScatterTransform(myTransform);
        }
    }


    private static void ScatterTransform(Transform myTransform)
    {
        if (myTransform.childCount == 0 
            || myTransform.GetChild(0).GetComponent<MeshRenderer>() != null
            || myTransform.GetComponent<PlanetAlignFlag>() != null)
        {
            //print("Scattering " + myTransform.name);

            var scatter2D = Random.insideUnitCircle * 0.2f;
            var scatter = new Vector3(scatter2D.x, 0, scatter2D.y);

            myTransform.Translate(scatter, Space.Self);
        }
        else
        {
            for (int i = 0; i < myTransform.childCount; i++)
            {
                ScatterTransform(myTransform.GetChild(i));
            }
        }
    }
}
