using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform target; // Refer�ncia ao personagem
    public float distance = 5f; // Dist�ncia fixa da c�mera
    public float sensitivity = 3f; // Sensibilidade do mouse
    public float minYAngle = -20f, maxYAngle = 60f; // Limites verticais da c�mera

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        if (target != null)
        {
            Vector3 angles = transform.eulerAngles;
            rotationX = angles.y;
            rotationY = angles.x;
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            
            rotationX += Input.GetAxis("Mouse X") * sensitivity;
            rotationY -= Input.GetAxis("Mouse Y") * sensitivity;
            rotationY = Mathf.Clamp(rotationY, minYAngle, maxYAngle);

            
            Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);
            Vector3 offset = rotation * new Vector3(0, 0, -distance);
            transform.position = target.position + offset;

            // Olha para o alvo
            transform.LookAt(target);
        }
    }
}
