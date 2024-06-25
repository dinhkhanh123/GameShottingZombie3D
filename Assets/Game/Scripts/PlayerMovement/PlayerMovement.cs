using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Health")]
    const float maxHealth = 100f;
    public float currentHealth;

    public Slider healthSlider;
    public GameObject playerUI;

     TextMeshProUGUI killNumber;
     TextMeshProUGUI healthNumber;

    [Header("Ref & Physics")]
    InputManager inputManager; // Qu?n l� ??u v�o c?a ng??i ch?i
    PlayerManager playerManager;
    AnimatorManager animatorManager;
    ShootingController playerShooting;

    Vector3 moveDirection; // H??ng di chuy?n c?a ng??i ch?i

    Transform cameragameObject; // ??i t??ng camera trong tr� ch?i

    Rigidbody playerRb; // Rigidbody c?a ng??i ch?i

    [Header("Falling and Landing")]
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffset;
    public LayerMask groundLayer;

    [Header("Movement flags")]
    public bool isMoving;
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;
    public bool isHealing = false;


    [Header("Movement values")]
    public float movementSpeed = 2f; // T?c ?? di chuy?n c?a ng??i ch?i
    public float rotateSpeed = 13f; // T?c ?? quay c?a ng??i ch?i
    public float sprintingSpeed = 5f;

    [Header("Jump Var")]
    public float jumpHeight = 4f;
    public float gravityIntensity = -15f;

    [Header("Effects")]
    public ParticleSystem healing;
    void Awake()
    {
        currentHealth = maxHealth; 
        animatorManager = GetComponent<AnimatorManager>();
        playerManager = GetComponent<PlayerManager>();
        playerShooting = GetComponent<ShootingController>();
        inputManager = GetComponent<InputManager>(); // L?y tham chi?u ??n InputManager
        playerRb = GetComponent<Rigidbody>(); // L?y tham chi?u ??n Rigidbody
        cameragameObject = Camera.main.transform; // L?y tham chi?u ??n transform c?a camera ch�nh

        healthSlider.minValue = 0f;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;



    }

     void Update()
    {

        if (currentHealth < maxHealth && inputManager.healingInput &&  UIManager.instance.healthCount > 0)
        {
            Healing();
        }

    }

    private void Healing()
    {
        if (!isHealing )
        {
            isHealing = true;
            healing.Play();
            Invoke("FinishHealth", 1.5f);
        }
    }

    private void FinishHealth()
    {

        currentHealth = Mathf.Min(currentHealth + 50, maxHealth);
        healthSlider.value = currentHealth;
        UIManager.instance.healthCount--; // Gi?m healthCount ?i 1 sau khi h?i m�u
        UIManager.instance.UpdateHealthCounter(); // C?p nh?t UI
        isHealing = false;
        
       
    }


    public void HandleAllMovement()
    {
        HandleFallingAndLanding();
        if (playerManager.isInteracting)
        {
            return;
        }
        HandleMovement(); // X? l� di chuy?n
        HandleRotate(); // X? l� quay
    }

    void HandleMovement()
    {
        if (isJumping)
            return;
        // T�nh to�n h??ng di chuy?n d?a tr�n ??u v�o t? InputManager
        moveDirection = new Vector3(cameragameObject.forward.x,0f,cameragameObject.forward.z) * inputManager.verticalInput;
        moveDirection = moveDirection + cameragameObject.right * inputManager.horizontalInput;
        moveDirection.Normalize(); // Chu?n h�a vector h??ng di chuy?n

        moveDirection.y = 0; // Kh�ng thay ??i h??ng di chuy?n theo tr?c y

        if (isSprinting)
        {
            // N?u ng??i ch?i ?ang ch?y, t?ng t?c ?? di chuy?n l�n t?c ?? ch?y
            moveDirection *= sprintingSpeed;
        }
        else
        {
            // N?u kh�ng ph?i ?ang ch?y
            if (inputManager.movementAmount >= 0.5f)
            {
                // N?u ng??i ch?i ?ang di chuy?n �t nh?t 50% (t?c l� ?ang di chuy?n nhanh), th� �p d?ng t?c ?? di chuy?n th??ng
                moveDirection *= movementSpeed;
                isMoving = true; // ??t c? hi?u ?ang di chuy?n th�nh true
            }

            // N?u inputManager.movementAmount l?n h?n ho?c b?ng 0 (ng??i ch?i ?ang di chuy?n)
            if (inputManager.movementAmount <= 0)
            {
                isMoving = false; // ??t c? hi?u ?ang di chuy?n th�nh false (t?c l� kh�ng di chuy?n)
            }
        }


        // G�n v?n t?c di chuy?n v�o Rigidbody c?a ng??i ch?i
        Vector3 movementVelocity = moveDirection;
        playerRb.velocity = movementVelocity;

      
    }

    void HandleRotate()
    {
        if (isJumping)
            return;

            Vector3 targetDirection = Vector3.zero;
    
            // T�nh to�n h??ng quay d?a tr�n ??u v�o t? InputManager
            targetDirection = cameragameObject.forward * inputManager.verticalInput;
            targetDirection = targetDirection + cameragameObject.right * inputManager.horizontalInput;
            targetDirection.Normalize(); // Chu?n h�a vector h??ng quay

            targetDirection.y = 0; // Kh�ng thay ??i h??ng quay theo tr?c y
     
        // N?u kh�ng c� h??ng quay, gi? nguy�n h??ng quay hi?n t?i c?a ng??i ch?i
        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        // Lerp gi?a h??ng quay hi?n t?i v� h??ng quay m?i
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        // G�n h??ng quay m?i cho ng??i ch?i
        transform.rotation = playerRotation;

        if(playerShooting.isShootingInput && playerShooting.isWalking)
        {
            Vector3 forwardDirection = Vector3.ProjectOnPlane(cameragameObject.forward, Vector3.up).normalized;
            Quaternion forwardRotation = Quaternion.LookRotation(forwardDirection);
            transform.rotation = forwardRotation;
        }

    }

    void HandleFallingAndLanding()
    {
        RaycastHit hit;
         Vector3 rayCastOrigin = transform.position;
        Vector3 targetPosition;
        rayCastOrigin.y += rayCastHeightOffset;
        targetPosition = transform.position;


        // Ki?m tra n?u ng??i ch?i kh�ng ??ng tr�n m?t ??t (?ang r?i)
        if (!isGrounded && !isJumping)
        {
            // N?u ng??i ch?i kh�ng ?ang t??ng t�c (v� d?: kh�ng ?ang th?c hi?n h�nh ??ng n�o kh�c)
            if (!playerManager.isInteracting)
            {
                // Ch?y animation khi ng??i ch?i r?i
                animatorManager.PlayTargetAnim("Falling", true);
            }

            // T?ng th?i gian trong kh�ng trung
            inAirTimer += Time.deltaTime;

            // �p d?ng l?c ?? di chuy?n ng??i ch?i v? ph�a tr??c khi nh?y
            playerRb.AddForce(transform.forward * leapingVelocity);

            // �p d?ng l?c ?? ng??i ch?i r?i xu?ng (v?i m?t l?c t?ng d?n theo th?i gian trong kh�ng trung)
            playerRb.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }

        // Ki?m tra n?u c� va ch?m v?i m?t ??t
        if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, groundLayer))
        {
            // N?u tr??c ?� ng??i ch?i kh�ng ??ng tr�n m?t ??t v� kh�ng ?ang t??ng t�c
            if (!isGrounded && !playerManager.isInteracting)
            {
                // Ch?y animation khi ng??i ch?i ??t ??t
                animatorManager.PlayTargetAnim("Landing", false);
            }

            // L?y ?i?m va ch?m t? raycast
            Vector3 rayCastHitPoint = hit.point;

            // C?p nh?t v? tr� m?c ti�u c?a ng??i ch?i ?? ??t ng??i ch?i l�n m?t ??t
            targetPosition.y = rayCastHitPoint.y;

            // ??t l?i th?i gian trong kh�ng trung
            inAirTimer = 0;

            // Ng??i ch?i ?ang ??ng tr�n m?t ??t
            isGrounded = true;
        }
        else
        {
            // N?u kh�ng c� va ch?m, ng??i ch?i kh�ng ??ng tr�n m?t ??t
            isGrounded = false;
        }

        if (isGrounded && !isJumping) // Ki?m tra n?u ng??i ch?i ?ang ??ng tr�n m?t ??t v� kh�ng nh?y
        {
            if (playerManager.isInteracting || inputManager.movementAmount > 0)
            {
                // N?u ng??i ch?i ?ang t??ng t�c ho?c c� ??u v�o v?n ??ng
                // Di chuy?n ng??i ch?i m?t c�ch m??t m� ??n v? tr� ?�ch s? d?ng linear interpolation
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else
            {
                // N?u ng??i ch?i kh�ng t??ng t�c v� kh�ng c� ??u v�o v?n ??ng
                // ??t v? tr� c?a ng??i ch?i tr?c ti?p ??n v? tr� ?�ch
                transform.position = targetPosition;
            }
        }
    }

   public void HandleJumping()
    {
        if (isGrounded)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnim("Jump",false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            playerRb.velocity = playerVelocity;

            isJumping = false;
        }
    }

    public void SetIsJumping(bool isJumping)
    {
        this.isJumping = isJumping;
    }

    public void playerHitDamage(float takeDamage)
    {
        currentHealth-=takeDamage;
        healthSlider.value = currentHealth;
        
        if (currentHealth <= 0)
        {
            PlayerDie();

        }
    }

    private void PlayerDie()
    {
        Cursor.lockState = CursorLockMode.None;
        UIManager.instance.GameOver();
        Destroy(gameObject,0.1f);
    }


    
}
