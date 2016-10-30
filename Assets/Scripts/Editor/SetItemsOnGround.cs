using UnityEngine;
using UnityEditor;
using System.Collections;

public class SetItemsOnGround : MonoBehaviour 
{
	[MenuItem ("Custom/Align with ground %t")]
	static void AlignWithGround()
	{
		Transform[] transforms = Selection.transforms;

        int layerMask = 0;// LayerMask.GetMask(Layers.Ground);

		foreach (Transform myTransform in transforms) 
		{
			print ("Aligning " + myTransform.name + " to ground");

			RaycastHit hit;
			if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, 100f, layerMask)) 
			{
				//print ("Ray cast hit " + hit.transform.name);

				Vector3 targetPosition = hit.point;
				if (myTransform.gameObject.GetComponent<MeshRenderer>() != null) 
				{
					Bounds bounds = myTransform.gameObject.GetComponent<MeshRenderer>().bounds;

					targetPosition.y += myTransform.position.y - bounds.min.y;
				}

				myTransform.position = targetPosition;
				//Vector3 targetRotation = new Vector3(hit.normal.x, myTransform.eulerAngles.y, hit.normal.z);
				//myTransform.eulerAngles = targetRotation;
			}
			else
				print ("No ground found");
		}
	}
}
