using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private float yaw;
    private float pitch;

    [SerializeField]
    private float mouseSensitivity = 3f;

    [SerializeField]
    private float distanceFromTargetX;

    [SerializeField]
    private float distanceFromTargetY;

    [SerializeField]
    private float pitchMin = -40;

    [SerializeField]
    private float pitchMax = 80;

    [SerializeField]
    private bool lockCursor;

    private Vector3 currentRotation;

    [SerializeField]
    private float rotationSmoothTime = 0.12f;

    [SerializeField]
    private Vector3 rotationSmoothVelocity;

    [SerializeField]
    private Transform target; // rotacionar em volta de um alvo, no caso o player

    [SerializeField]
    private LayerMask cameraLayerMask;

    // Use this for initialization
    private void Start()
    {
        if (lockCursor)
        {
            //trava o cursor no centro da tela
            Cursor.lockState = CursorLockMode.Locked;
            // deixa o cursor invisivel
            Cursor.visible = false;
        }

        // transform.eulerAngles = target.eulerAngles;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        // input do mouse
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        //limita a rotacao no y
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        Vector3 targetRotation = new Vector3(pitch, yaw);

        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);
        //fazemos a rotação
        transform.eulerAngles = currentRotation;

        // colocamos a camera numa posicao logo atras do jogador
        transform.position = target.position - transform.forward * distanceFromTargetX + transform.up * distanceFromTargetY;

        CheckWall();
    }

    private void CheckWall()
    {
        RaycastHit hit;
        // inicio do raio
        Vector3 raystart = target.position;
        //direção da posição
        Vector3 dir = (transform.position - target.position).normalized;

        //distancia do raio ( que é a distancia do player até a camera
        float dist = Vector3.Distance(transform.position, target.position);

        if (Physics.Raycast(raystart, dir, out hit, dist, cameraLayerMask))
        {
            float hitDistance = hit.distance;

            Vector3 castCenterHit = target.position + (dir.normalized * hitDistance);

            transform.position = castCenterHit;
        }
    }
}
