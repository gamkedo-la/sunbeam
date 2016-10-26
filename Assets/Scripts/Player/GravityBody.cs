using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
    private GravityAttractor m_gravityAttractor;
    private Rigidbody m_rigidBody;


    void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_rigidBody.useGravity = false;

        m_gravityAttractor = GameObject.FindGameObjectWithTag(Tags.Planet).GetComponent<GravityAttractor>();
    }

    
	void FixedUpdate()
    {
        m_gravityAttractor.Attract(m_rigidBody);
	}
}
