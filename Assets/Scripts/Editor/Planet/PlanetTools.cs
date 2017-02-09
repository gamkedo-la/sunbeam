using UnityEngine;
using UnityEditor;
using System.Collections;

public class PlanetTools : MonoBehaviour
{
    [MenuItem("Planet/Reset environment rotation %w", false, 1)]
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


    [MenuItem("Planet/Align with planet surface %#y", false, 2)]
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
        if (myTransform.childCount == 0 
            || myTransform.GetChild(0).GetComponent<MeshRenderer>() != null
            || myTransform.GetComponent<PlanetAlignFlag>() != null)
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


    [MenuItem("Planet/Set on ground %t", false, 3)]
    static void SetTransformsOnGround()
    {
        Transform[] transforms = Selection.transforms;

        int layerMask = LayerMask.GetMask(Layers.Ground);

        foreach (Transform myTransform in transforms)
        {
            SetTransformOnGround(myTransform, layerMask);
        }
    }


    private static void SetTransformOnGround(Transform myTransform, int layerMask)
    {
        if (myTransform.childCount == 0 
            || myTransform.GetChild(0).GetComponent<MeshRenderer>() != null
            || myTransform.GetComponent<PlanetAlignFlag>() != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(myTransform.position + myTransform.up * 2f, -myTransform.up, out hit, 100f, layerMask))
            {
                var targetPosition = hit.point;

                myTransform.position = targetPosition;
            }
            else
                print("No ground found");
        }
        else
        {
            for (int i = 0; i < myTransform.childCount; i++)
            {
                SetTransformOnGround(myTransform.GetChild(i), layerMask);
            }
        }
    }


    [MenuItem("Planet/Align with and place on surface", false, 4)]
    static void AlignTransformsWithAndPlaceOnSurface()
    {
        Transform[] transforms = Selection.transforms;

        int layerMask = LayerMask.GetMask(Layers.Ground);

        foreach (Transform myTransform in transforms)
        {
            AlignTransformWithAndPlaceOnSurface(myTransform, layerMask);
        }
    }


    private static void AlignTransformWithAndPlaceOnSurface(Transform myTransform, int layerMask)
    {
        if (myTransform.childCount == 0
            || myTransform.GetChild(0).GetComponent<MeshRenderer>() != null
            || myTransform.GetComponent<PlanetAlignFlag>() != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(myTransform.position + myTransform.position.normalized * 2f, -myTransform.position.normalized, out hit, 100f, layerMask))
            {
                var targetPosition = hit.point;
                var normal = hit.normal;

                myTransform.position = targetPosition;

                var targetDirection = normal;
                var transformUp = myTransform.up;
                myTransform.rotation = Quaternion.FromToRotation(transformUp, targetDirection) * myTransform.rotation;       
            }
            else
                print("No ground found");
        }
        else
        {
            for (int i = 0; i < myTransform.childCount; i++)
            {
                AlignTransformWithAndPlaceOnSurface(myTransform.GetChild(i), layerMask);
            }
        }
    }
}
