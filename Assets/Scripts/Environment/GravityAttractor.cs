using UnityEngine;
using System.Collections;

public class GravityAttractor : MonoBehaviour
{
    [SerializeField] float m_gravity = -10f;

    private Vector3 m_targetDirection;


    public void Attract(Rigidbody rigidbody)
    {
        Align(rigidbody);

        rigidbody.AddForce(m_targetDirection * m_gravity);
    }


    public void Align(Rigidbody rigidbody)
    {
        var bodyTransform = rigidbody.transform;

        m_targetDirection = (bodyTransform.position - transform.position).normalized;

        if (rigidbody.freezeRotation)
        {
            var bodyUp = bodyTransform.up;

            bodyTransform.rotation = Quaternion.FromToRotation(bodyUp, m_targetDirection) * bodyTransform.rotation;
        }
    }
}
