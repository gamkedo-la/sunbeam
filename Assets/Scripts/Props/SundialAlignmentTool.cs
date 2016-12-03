using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SundialAlignmentTool : MonoBehaviour
{
    [SerializeField] Transform m_sundialTop;


    public void Align()
    {
        print("Align() called");

        var planetObject = GameObject.FindGameObjectWithTag(Tags.Planet);

        if (planetObject != null)
        {
            var planet = planetObject.transform;

            var north = planet.up;
            var sundialDirection = transform.position;

            var a = Vector3.Cross(north, sundialDirection);
            var forwardDirection = Vector3.Cross(a, sundialDirection);

            var upDirection = transform.position.normalized;

            transform.rotation = Quaternion.FromToRotation(transform.up, upDirection) * transform.rotation;
            transform.rotation = Quaternion.FromToRotation(transform.forward, forwardDirection) * transform.rotation;

            float latitude = Vector3.Angle(north, sundialDirection) - 90f;

            m_sundialTop.localEulerAngles = Vector3.left * latitude;
        }
        else
        {
            print("Planet not found, alignment couldn't be performed");
        }
    }
}
