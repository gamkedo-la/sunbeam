using UnityEngine;
using UnityEditor;
using System.Collections;

public class RandomScatter : MonoBehaviour
{
    [MenuItem("Planet/Apply random scatter %,")]
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
        if (myTransform.childCount == 0 || myTransform.GetChild(0).GetComponent<MeshRenderer>() != null)
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
