using UnityEngine;
using System.Collections;

public class JoystickLook : MonoBehaviour
{
    [SerializeField] bool m_invertedVertical;
    [SerializeField] string m_horizontalAxisName = "Horizontal look";
    [SerializeField] string m_verticalAxisName = "Vertical look";
    [SerializeField] float m_sensitivityX = 3f;
    [SerializeField] float m_sensitivityY = 3f;

    private float m_verticalLookRotation;


    public void LookRotation(Transform character, Transform camera, Vector2 verticalLookMinMax)
    {
        float h = Input.GetAxis(m_horizontalAxisName);
        float v = Input.GetAxis(m_verticalAxisName);

        v = m_invertedVertical ? -v : v;

        character.Rotate(Vector3.up, h * m_sensitivityX * 60f * Time.deltaTime);
        m_verticalLookRotation += v * m_sensitivityY * 60f * Time.deltaTime;
        camera.localEulerAngles = Vector3.left * m_verticalLookRotation;

        m_verticalLookRotation = Mathf.Clamp(m_verticalLookRotation, verticalLookMinMax.x, verticalLookMinMax.y);
        camera.localEulerAngles = Vector3.left * m_verticalLookRotation;
    }
}
