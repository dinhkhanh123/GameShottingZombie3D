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
    InputManager inputManager; // Qu?n lý ??u vào c?a ng??i ch?i
    PlayerManager playerManager;
    AnimatorManager animatorManager;
    ShootingController playerShooting;

    Vector3 moveDirection; // H??ng di chuy?n c?a ng??i ch?i

    Transform cameragameObject; // ??i t??ng camera trong trò ch?i

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
        cameragameObject = Camera.main.transform; // L?y tham chi?u ??n transform c?a camera chính

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
        UIManager.instance.healthCount--; // Gi?m healthCount ?i 1 sau khi h?i máu
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
        HandleMovement(); // X? lý di chuy?n
        HandleRotate(); // X? lý quay
    }

    void HandleMovement()
    {
        if (isJumping)
            return;
        // Tính toán h??ng di chuy?n d?a trên ??u vào t? InputManager
        moveDirection = new Vector3(cameragameObject.forward.x,0f,cameragameObject.forward.z) * inputManager.verticalInput;
        moveDirection = moveDirection + cameragameObject.right * inputManager.horizontalInput;
        moveDirection.Normalize(); // Chu?n hóa vector h??ng di chuy?n

        moveDirection.y = 0; // Không thay ??i h??ng di chuy?n theo tr?c y

        if (isSprinting)
        {
            // N?u ng??i ch?i ?ang ch?y, t?ng t?c ?? di chuy?n lên t?c ?? ch?y
            moveDirection *= sprintingSpeed;
        }
        else
        {
            // N?u không ph?i ?ang ch?y
            if (inputManager.movementAmount >= 0.5f)
            {
                // N?u ng??i ch?i ?ang di chuy?n ít nh?t 50% (t?c là ?ang di chuy?n nhanh), thì áp d?ng t?c ?? di chuy?n th??ng
                moveDirection *= movementSpeed;
                isMoving = true; // ??t c? hi?u ?ang di chuy?n thành true
            }

            // N?u inputManager.movementAmount l?n h?n ho?c b?ng 0 (ng??i ch?i ?ang di chuy?n)
            if (inputManager.movementAmount <= 0)
            {
                isMoving = false; // ??t c? hi?u ?ang di chuy?n thành false (t?c là không di chuy?n)
            }
        }


        // Gán v?n t?c di chuy?n vào Rigidbody c?a ng??i ch?i
        Vector3 movementVelocity = moveDirection;
        playerRb.velocity = movementVelocity;

      
    }

    void HandleRotate()
    {
        if (isJumping)
            return;

            Vector3 targetDirection = Vector3.zero;
    
            // Tính toán h??ng quay d?a trên ??u vào t? InputManager
            targetDirection = cameragameObject.forward * inputManager.verticalInput;
            targetDirection = targetDirection + cameragameObject.right * inputManager.horizontalInput;
            targetDirection.Normalize(); // Chu?n hóa vector h??ng quay

            targetDirection.y = 0; // Không thay ??i h??ng quay theo tr?c y
     
        // N?u không có h??ng quay, gi? nguyên h??ng quay hi?n t?i c?a ng??i ch?i
        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        // Lerp gi?a h??ng quay hi?n t?i và h??ng quay m?i
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        // Gán h??ng quay m?i cho ng??i ch?i
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


        // Ki?m tra n?u ng??i ch?i không ??ng trên m?t ??t (?ang r?i)
        if (!isGrounded && !isJumping)
        {
            // N?u ng??i ch?i không ?ang t??ng tác (ví d?: không ?ang th?c hi?n hành ??ng nào khác)
            if (!playerManager.isInteracting)
            {
                // Ch?y animation khi ng??i ch?i r?i
                animatorManager.PlayTargetAnim("Falling", true);
            }

            // T?ng th?i gian trong không trung
            inAirTimer += Time.deltaTime;

            // Áp d?ng l?c ?? di chuy?n ng??i ch?i v? phía tr??c khi nh?y
            playerRb.AddForce(transform.forward * leapingVelocity);

            // Áp d?ng l?c ?? ng??i ch?i r?i xu?ng (v?i m?t l?c t?ng d?n theo th?i gian trong không trung)
            playerRb.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }

        // Ki?m tra n?u có va ch?m v?i m?t ??t
        if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, groundLayer))
        {
            // N?u tr??c ?ó ng??i ch?i không ??ng trên m?t ??t và không ?ang t??ng tác
            if (!isGrounded && !playerManager.isInteracting)
            {
                // Ch?y animation khi ng??i ch?i ??t ??t
                animatorManager.PlayTargetAnim("Landing", false);
            }

            // L?y ?i?m va ch?m t? raycast
            Vector3 rayCastHitPoint = hit.point;

            // C?p nh?t v? trí m?c tiêu c?a ng??i ch?i ?? ??t ng??i ch?i lên m?t ??t
            targetPosition.y = rayCastHitPoint.y;

            // ??t l?i th?i gian trong không trung
            inAirTimer = 0;

            // Ng??i ch?i ?ang ??ng trên m?t ??t
            isGrounded = true;
        }
        else
        {
            // N?u không có va ch?m, ng??i ch?i không ??ng trên m?t ??t
            isGrounded = false;
        }

        if (isGrounded && !isJumping) // Ki?m tra n?u ng??i ch?i ?ang ??ng trên m?t ??t và không nh?y
        {
            if (playerManager.isInteracting || inputManager.movementAmount > 0)
            {
                // N?u ng??i ch?i ?ang t??ng tác ho?c có ??u vào v?n ??ng
                // Di chuy?n ng??i ch?i m?t cách m??t mà ??n v? trí ?ích s? d?ng linear interpolation
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else
            {
                // N?u ng??i ch?i không t??ng tác và không có ??u vào v?n ??ng
                // ??t v? trí c?a ng??i ch?i tr?c ti?p ??n v? trí ?ích
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
