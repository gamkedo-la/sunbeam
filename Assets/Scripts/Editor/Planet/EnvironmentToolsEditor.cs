using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(EnvironmentTools))]
public class EnvironmentToolsEditor : Editor
{
    public void OnSceneGUI()
    {
        var e = Event.current;  

        if (e != null && e.type == EventType.mouseDown && e.button == 1 && e.control)
        {
            Ray ray = Camera.current.ScreenPointToRay(e.mousePosition);

            int layerMask = 1 << LayerMask.NameToLayer(Layers.Environment);
            //Debug.Log("Layer mask: " + layerMask);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, layerMask))
            {
                //Debug.Log("Mouse clicked on " + hit.collider.name + " at " + hit.point + ", " + hit.distance + " away");
                //Debug.Log("Layer: " + hit.collider.gameObject.layer + " (" + LayerMask.LayerToName(hit.collider.gameObject.layer) + ")");

                var transform = (target as EnvironmentTools).transform;
                var targetDirection = hit.point.normalized;
                var bodyUp = Vector3.up;

                transform.rotation = Quaternion.FromToRotation(targetDirection, bodyUp) * transform.rotation;
            }
            else
            {
                Debug.Log("No collider detected");
            }
        }
    }
}
