using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform target; // Referencia ao personagem
    public float distance = 13f; // Distancia fixa da c�mera
    public float sensitivity = 3f; // Sensibilidade do mouse
    public float minYAngle = -10f, maxYAngle = 60f; // Limites verticais da camera

    private float rotationX = 0f;
    private float rotationY = 0f;
    private bool followFloating = true; // Controle para seguir ou nao a subida e descida

    void Start()
    {
        if (target != null)
        {
            Vector3 angles = transform.eulerAngles;
            rotationX = angles.y;
            rotationY = angles.x;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Alterna o estado de seguir a subida e descida ao pressionar Enter
            if (Input.GetKeyDown(KeyCode.Return))
            {
                followFloating = !followFloating;
            }

            rotationX += Input.GetAxis("Mouse X") * sensitivity;
            rotationY -= Input.GetAxis("Mouse Y") * sensitivity;
            rotationY = Mathf.Clamp(rotationY, minYAngle, maxYAngle);

            Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);

            // Corrige o valor de Y com base na escolha do usu�rio
            float floatOffset = Mathf.Sin(Time.time * 2f) * 0.5f;
            float adjustedY = followFloating ? target.position.y : target.position.y - floatOffset;

            Vector3 correctedTargetPos = new Vector3(target.position.x, adjustedY, target.position.z);
            Vector3 desiredCameraPos = correctedTargetPos + rotation * new Vector3(0, 0, -distance);
            Vector3 direction = (desiredCameraPos - correctedTargetPos).normalized;
            float maxDistance = distance;

            // Raycast para detectar colis�es
            RaycastHit hit;
            if (Physics.Raycast(correctedTargetPos, direction, out hit, maxDistance))
            {
                transform.position = hit.point - direction * 0.5f; // Pequeno recuo para evitar atravessar
            }
            else
            {
                transform.position = desiredCameraPos;
            }

            transform.LookAt(correctedTargetPos);
        }
    }
}