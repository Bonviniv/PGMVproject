using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidade de movimento
    public float floatAmplitude = 0.5f; // Amplitude do movimento senoidal
    public float floatFrequency = 2f; // Frequência do movimento senoidal
    public float rotationSpeed = 10f; // Velocidade de rotação
    public float heightOffset = 3.11f; // Distância fixa acima do terreno

    private Vector3 startPosition;
    private Transform cameraTransform;

    void Start()
    {
        startPosition = transform.position;
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal"); // Mapeia A/D e ←/→
        float moveZ = Input.GetAxis("Vertical");   // Mapeia W/S e ↑/↓

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * moveZ + right * moveX).normalized;
        if (moveDirection.magnitude > 0)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection) * Quaternion.Euler(0, 90, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Obtém a altura do terreno abaixo do personagem
        float groundHeight = Terrain.activeTerrain.SampleHeight(transform.position);

        // Aplica o movimento de subida e descida com a função seno, mantendo a altura relativa ao terreno
        float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, groundHeight + heightOffset + floatOffset, transform.position.z);
    }
}
