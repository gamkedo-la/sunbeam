using UnityEngine;
using System.Collections;

public class MouseLook : MonoBehaviour
{
    [SerializeField] float m_sensitivityX = 3f;
    [SerializeField] float m_sensitivityY = 3f;

    private float m_verticalLookRotation;


    public void LookRotation(Transform character, Transform camera, Vector2 verticalLookMinMax)
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        character.Rotate(Vector3.up * mouseX * m_sensitivityX);
        m_verticalLookRotation += mouseY * m_sensitivityY;
        camera.localEulerAngles = Vector3.left * m_verticalLookRotation;

        m_verticalLookRotation = Mathf.Clamp(m_verticalLookRotation, verticalLookMinMax.x, verticalLookMinMax.y);
        camera.localEulerAngles = Vector3.left * m_verticalLookRotation;
    }
}
