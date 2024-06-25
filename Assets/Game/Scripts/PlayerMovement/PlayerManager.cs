using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager; 

    PlayerMovement playerMovement; 

    CameraManager cameraManager;

    public bool isInteracting;

    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>(); 
        playerMovement = GetComponent<PlayerMovement>(); 
        cameraManager = FindObjectOfType<CameraManager>();
    }

    void Update()
    {
        inputManager.HandleAllInput(); 
        
    }

    //FixedUpdate(): ???c g?i theo chu k? c? ??nh, m?c ??nh l� sau m?i 0.02 gi�y (50 l?n m?i gi�y).
    //Th??ng ???c s? d?ng ?? x? l� logic v?t l�, nh? chuy?n ??ng v� t??ng t�c v?i Rigidbody.
    void FixedUpdate()
    {
        playerMovement.HandleAllMovement();
        // X? l� t?t c? chuy?n ??ng c?a ng??i ch?i trong FixedUpdate
    }

     void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement();
        isInteracting = animator.GetBool("isInteracting");
        playerMovement.isJumping = animator.GetBool("isJumping");
        animator.SetBool("isGrounded",playerMovement.isGrounded);   

    }
}
