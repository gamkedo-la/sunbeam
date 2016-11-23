using UnityEngine;
using UnityEditor;
using System.Collections;

public class SetItemsOnGround : MonoBehaviour 
{
    [MenuItem("Planet/Set on ground %t")]
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
        if (myTransform.childCount == 0 || myTransform.GetChild(0).GetComponent<MeshRenderer>() != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(myTransform.position, -myTransform.up, out hit, 100f, layerMask))
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
}
