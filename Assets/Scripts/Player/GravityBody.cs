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

        var gravityAttractorObject = GameObject.FindGameObjectWithTag(Tags.Planet);

        if (gravityAttractorObject != null)
        {
            m_gravityAttractor = gravityAttractorObject.GetComponent<GravityAttractor>();
            m_rigidBody.useGravity = false;
        }
        else
            m_rigidBody.useGravity = true;
    }

    
	void FixedUpdate()
    {
        if (m_gravityAttractor != null)
            m_gravityAttractor.Attract(m_rigidBody);
	}
}
