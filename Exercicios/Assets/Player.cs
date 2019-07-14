using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 2f;
    public float jumpHeight = 5f;
    public int coinCount = 0;
    public int hp = 3;

    public float walkSpeed = 3f;
    public float runSpeed = 8f;
    public float gravity = -12f;

    [SerializeField]
    private float velocityY;

    [SerializeField]
    private bool running = false;

    [SerializeField]
    private Game game_ref;

    public TextMeshProUGUI textoMoedas;
    public TextMeshProUGUI textoHP;

    private float smoothRotationVelocity;

    [SerializeField]
    private float smoothRotationTime = 0.2f;

    private float smoothSpeedVelocity;

    [SerializeField]
    private float smoothSpeedTime = 0.2f;

    [SerializeField]
    private Transform cameraT;

    [SerializeField]
    private CharacterController charController;

    [SerializeField]
    private Animator animator;

    // Use this for initialization
    private void Start()
    {
        //pegando referências
        cameraT = Camera.main.transform;
        charController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // speed = 2f;
        textoMoedas.text = "Coins: " + coinCount;
        textoHP.text = "HP: " + hp;

        //Instantiate(coinPrefab, new Vector3(2, 2, 2), Quaternion.identity);
    }

    // Update is called once per frame
    private void Update()
    {
        walkingRotating();
        //walkSideways();
    }

    //logica para andar rotacionando
    private void walkingRotating()
    {
        // pegar input do jogador
        // Input.getaxis retorna um valor de -1 a 1
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //normalizamos para pegar apenas a direção
        Vector2 inputDir = input.normalized;

        float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
        //rotação
        if (inputDir != Vector2.zero)
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref smoothRotationVelocity, smoothRotationTime);

        running = (Input.GetKey(KeyCode.LeftShift));

        float targetSpeed = (running) ? runSpeed : walkSpeed * inputDir.magnitude;

        speed = Mathf.SmoothDamp(speed, targetSpeed, ref smoothSpeedVelocity, smoothSpeedTime);

        //aumentando a aceleração da gravidade
        velocityY += gravity * Time.deltaTime;

        Vector3 velocity = transform.forward * speed * inputDir.magnitude + Vector3.up * velocityY;

        charController.Move(velocity * Time.deltaTime);

        speed = new Vector2(charController.velocity.x, charController.velocity.z).magnitude;

        if (charController.isGrounded)
        {
            velocityY = 0;
        }

        {
            float animationSpeedPercent = ((running) ? speed / runSpeed : speed / walkSpeed * 0.5f) * inputDir.magnitude;

            animator.SetFloat("speedPercent", animationSpeedPercent, smoothSpeedTime, Time.deltaTime);
            animator.SetBool("isGrounded", charController.isGrounded);
            animator.SetFloat("velocityY", velocityY);
            animator.SetBool("spaceDown", Input.GetKeyDown(KeyCode.Space));
        }
        Jump();
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (charController.isGrounded)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Animation"))
                {
                    animator.Play("JumpUp");
                }
                float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
                velocityY = jumpVelocity;
            }
        }
    }

    //logica para andar sem rotacionar
    private void walkSideways()
    {
        // pegar input do jogador
        // Input.getaxis retorna um valor de -1 a 1
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        //normalizamos para pegar apenas a direção

        Vector3 direction = input.normalized;
        //nossa velocidade será a direção multiplicada pela nossa velocidade
        Vector3 velocity = direction * speed;

        //por fim a distância para percorrer será essa velocidade multiplicada pelo tempo
        Vector3 moveAmount = velocity * Time.deltaTime;

        //aqui iremos mover nosso jogador pela distância que iremos percorrer
        transform.Translate(moveAmount);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Objeto"))
        {
            GameObject.Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Objeto"))
        {
            GameObject.Destroy(collision.gameObject);
        }
    }
}
