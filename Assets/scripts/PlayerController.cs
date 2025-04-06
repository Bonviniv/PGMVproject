using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidade de movimento
    public float floatAmplitude = 0.5f; // Amplitude do movimento senoidal
    public float floatFrequency = 2f; // Frequência do movimento senoidal
    public float rotationSpeed = 10f; // Velocidade de rotação
    public float heightOffset = 3.11f; // Distância fixa acima do terreno

    [SerializeField] private Transform armacaoMarvin;
    [SerializeField] private Animator animator; // Referência ao Animator
    [SerializeField] private Transform vaso_space; // Posição onde o vaso será fixado ao ser carregado

    public bool canPick = false;
    public bool carrying = false;

    private GameObject currentVaso = null; // Referência ao vaso mais próximo
    private Vector3 startPosition;
    private Transform cameraTransform;

    void Start()
    {
        startPosition = transform.position;
        cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

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

        float groundHeight = Terrain.activeTerrain.SampleHeight(transform.position);
        float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, groundHeight + heightOffset + floatOffset, transform.position.z);

        if (armacaoMarvin != null)
        {
            armacaoMarvin.rotation = Quaternion.Euler(-90, armacaoMarvin.rotation.eulerAngles.y, armacaoMarvin.rotation.eulerAngles.z);
        }

        // Verifica o vaso mais próximo
        GameObject[] vasos = GameObject.FindGameObjectsWithTag("vaso");
        float minDist = Mathf.Infinity;
        GameObject closestVaso = null;

        foreach (GameObject vaso in vasos)
        {
            float dist = Vector3.Distance(transform.position, vaso.transform.position);
            Renderer rend = vaso.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = Color.white; // Resetar a cor padrão
            }
            if (dist < 5f && dist < minDist)
            {
                minDist = dist;
                closestVaso = vaso;
            }
        }

        if (closestVaso != null)
        {
           // Debug.Log("canPick = true");
            canPick = true;
            currentVaso = closestVaso;
            Renderer rend = currentVaso.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = Color.yellow; // Destaque visual
            }
        }
        else
        {
            canPick = false;
            currentVaso = null;
        }

        // Interação com vasos
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canPick && !carrying && currentVaso != null)
            {
              
                animator.SetTrigger("pick");
                currentVaso.transform.SetParent(vaso_space);
                currentVaso.transform.localPosition = Vector3.zero;
                carrying = true;
                Debug.Log("Trigger 'pick' ativado. carrying = true");
            }
            else if (carrying && currentVaso != null)
            {
               
                animator.SetTrigger("drop");
                currentVaso.transform.SetParent(null);
                currentVaso.transform.position = new Vector3(transform.position.x, groundHeight + 0.8f , transform.position.z);
                carrying = false;
                Debug.Log("Trigger 'drop' ativado. carrying = false");

            }
        }
    }
}
