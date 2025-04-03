using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform target; // Referência ao personagem
    public float distance = 5f; // Distância fixa da câmera
    public float sensitivity = 3f; // Sensibilidade do mouse
    public float minYAngle = -20f, maxYAngle = 60f; // Limites verticais da câmera

    private float rotationX = 0f;
    private float rotationY = 0f;
    private bool followFloating = true; // Controle para seguir ou não a subida e descida
    private float baseHeightOffset; // Armazena o offset base da altura do personagem

    void Start()
    {
        if (target != null)
        {
            Vector3 angles = transform.eulerAngles;
            rotationX = angles.y;
            rotationY = angles.x;
            baseHeightOffset = target.position.y - Mathf.Sin(Time.time * 2f) * 0.5f; // Define a altura base do personagem sem o efeito senoidal
        }
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
            Vector3 desiredCameraPos = target.position + rotation * new Vector3(0, 0, -distance);

            // Ajusta a posição da câmera com base na configuração de followFloating
            float adjustedY = followFloating ? target.position.y : baseHeightOffset;
            Vector3 correctedTargetPos = new Vector3(target.position.x, adjustedY, target.position.z);
            Vector3 direction = (desiredCameraPos - correctedTargetPos).normalized;
            float maxDistance = distance;

            // Raycast para detectar colisões
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
