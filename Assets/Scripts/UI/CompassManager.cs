using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class CompassManager : MonoBehaviour
{
    private Transform m_planet;
    private Transform m_player;
    private Image m_compassImage;


    void Awake()
    {
        m_compassImage = GetComponent<Image>();
        m_player = GameObject.FindGameObjectWithTag(Tags.Player).transform;
        var planetObject = GameObject.FindGameObjectWithTag(Tags.Planet);

        if (planetObject != null)
            m_planet = planetObject.transform;   
    }
    

	void Update()
    {
        // http://www.movable-type.co.uk/scripts/latlong-vectors.html

        var n = m_planet.up;
        var a = m_player.position;
        var b = m_player.position + m_player.forward;

        var c1 = Vector3.Cross(a, b);
        var c2 = Vector3.Cross(a, n);
        var c1CrossC2 = Vector3.Cross(c1, c2);

        float sinTheta = c1CrossC2.magnitude * Mathf.Sign(Vector3.Dot(c1CrossC2, a));
        float cosTheta = Vector3.Dot(c1, c2);

        float theta = -Mathf.Atan2(sinTheta, cosTheta) * Mathf.Rad2Deg;

        m_compassImage.rectTransform.rotation = Quaternion.Euler(0, 0, theta);
	}
}
