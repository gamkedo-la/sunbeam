using UnityEngine;
using System.Collections;

public class GravityAttractor : MonoBehaviour
{
    [SerializeField] float m_gravity = -10f;


    public void Attract(Rigidbody rigidbody)
    {
        var bodyTransform = rigidbody.transform;

        var targetDirection = (bodyTransform.position - transform.position).normalized;

        if (rigidbody.freezeRotation)
        {
            var bodyUp = bodyTransform.up;

            bodyTransform.rotation = Quaternion.FromToRotation(bodyUp, targetDirection) * bodyTransform.rotation;
        }

        rigidbody.AddForce(targetDirection * m_gravity);
    }
}
