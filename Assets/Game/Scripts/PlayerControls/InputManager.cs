using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;
    AnimatorManager animatorManager;
    PlayerMovement playerMovement;

    public Vector2 movementInput;
    public Vector2 cameraMovementInput;

    public float verticalInput;
    public float horizontalInput;

    public float cameraInputX;
    public float cameraInputY;

    public float movementAmount;

    [Header("Input Button Flag")]
    public bool bInput;
    public bool jumpInput;

    public bool fireInput;
    public bool reloadInput;
    public bool scopeInput;
    public bool healingInput;


     void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    //This is a Unity callback method that is called when the script is enabled.
    void OnEnable()
    {
        if(playerControls == null)
        {
            playerControls = new PlayerControls();

            //This line subscribes to the 'performed' event of the 'Movement' action in the 'PlayerMovement' control scheme. 
            //When this action is performed (e.g., when the player moves an input device), the lambda expression 'i => movementInput = i.ReadValue<Vector2>()' is executed. 
            //It reads the input value as a 'Vector2 from the context (i) and assigns it to the 'movementInpu't variable.
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.CameraMovement.performed += i => cameraMovementInput = i.ReadValue<Vector2>();
            playerControls.PlayerActions.B.performed += i => bInput = true;
            playerControls.PlayerActions.B.canceled += i => bInput = false; 
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
            playerControls.PlayerActions.Fire.performed += i => fireInput = true;
            playerControls.PlayerActions.Fire.canceled += i => fireInput = false;
            playerControls.PlayerActions.Reload.performed += i => reloadInput = true;
            playerControls.PlayerActions.Scope.performed += i => scopeInput = true;
            playerControls.PlayerActions.Scope.canceled += i => scopeInput = false;
            playerControls.PlayerActions.Healing.performed += i => healingInput = true;
            playerControls.PlayerActions.Healing.canceled += i => healingInput = false;
        }
        playerControls.Enable();
    }
    // Unity callback method that is called when the script is disabled or the GameObject to which the script is attached is disabled.
    // In this case, it's used to clean up and disable the player controls.
    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInput()
    {
        HandleMovementInput();
        HandleSprintingInput();
        HandleJumpingInput();

    }

    private void HandleMovementInput()
    {
        // Extract vertical and horizontal inputs from the movementInput Vector2
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputX = cameraMovementInput.x;
        cameraInputY = cameraMovementInput.y;

        // Calculate total movement amount by summing absolute values of vertical and horizontal inputs
        movementAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

        // Update Animator values to reflect the movement amount
        animatorManager.ChangeAnimatorValues(0, movementAmount, playerMovement.isSprinting);
    }

    private void HandleSprintingInput()
    {
        if(bInput && movementAmount > 0.5)
        {
            playerMovement.isSprinting = true;
        }
        else
        {
            playerMovement.isSprinting = false;
        }
    }

    private void HandleJumpingInput()
    {
        if (jumpInput)
        {
            jumpInput = false;
            playerMovement.HandleJumping();
        }
    }
}
