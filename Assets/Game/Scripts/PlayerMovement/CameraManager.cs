using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Reference to InputManager for handling player input
    InputManager inputManager;

    // Reference to the player's transform
    public Transform playerTransform;
    public Transform cameraTransform;

    [Header("Camera Movement")]
    private Vector3 cameraFollowVelocity = Vector3.zero; 
    public Transform cameraPivot;
    public float cameraFollowSpeed = 0.3f;    
    public float camLookSpeed = 1f;
    public float camPivotSpeed = 1f;
    public float lookAngle;
    public float pivotAngle;
    public float minPivotAngle = -30f;
    public float maxPivotAngle = 30f;

    [Header("Camera Collision")]
    public LayerMask collisionLayer;
    private float defaultPosition;
    public float cameraCollisionOffset = 0.2f;
    public float minCollisionOffset = 0.2f;
    public float cameraCollisionRadius = 2f;

    private Vector3 cameraVectorPosition;

    private PlayerMovement playerMovement;
    [Header("Scope")]
    public GameObject scopeCanvas;
    public GameObject playerUI;
    public GameObject bodyPlayer;

    public Camera mainCamera;
    public bool isScoped = false;
    private float originalFOV = 60f;
    
    void Awake()
    {
        // Hide and lock the cursor at the center of the screen
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Find and assign references to InputManager and PlayerManager
        inputManager = FindObjectOfType<InputManager>();
        playerTransform = FindObjectOfType<PlayerManager>().transform;
        playerMovement = FindObjectOfType<PlayerMovement>();
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    // Function to handle all camera movements
    public void HandleAllCameraMovement()
    {
        FollowTarget(); // Call function to make the camera follow the player
        RotateCamera(); // Call function to rotate the camera based on player input
        CameraCollision();
        isPlayerScope();
    }

    // Function to make the camera smoothly follow the player
    void FollowTarget()
    {
        // Calculate the target position for the camera using SmoothDamp
        Vector3 targetPosition = Vector3.SmoothDamp(playerTransform.position, playerTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);
        transform.position = targetPosition; // Move the camera to the calculated position
    }

    // Function to rotate the camera based on player input
    void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        // Update the look and pivot angles based on player input
        lookAngle = lookAngle + (inputManager.cameraInputX * camLookSpeed);
        pivotAngle = pivotAngle + (inputManager.cameraInputY * - camPivotSpeed);

        pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle, maxPivotAngle);

        // Rotate the camera horizontally
         rotation = Vector3.zero;
         rotation.y = lookAngle;
         targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        // Rotate the camera pivot vertically within specified limits
        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;

        if(playerMovement.isMoving == false && playerMovement.isSprinting == false)
        {
            playerTransform.rotation = Quaternion.Euler(0,lookAngle,0);
        }
    }

    void CameraCollision()
    {
        // Set the initial target position to the default position
        float targetPosition = defaultPosition;

        // Declare a RaycastHit variable to store information about the collision
        RaycastHit hit;

        // Calculate the normalized direction from the camera pivot to the camera position
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        // Perform a sphere cast to check for collisions along the camera's view direction
        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayer))
        {
            // Calculate the distance from the camera pivot to the collision point
            float distance = Vector3.Distance(cameraPivot.position, hit.point);

            // Adjust the target position to avoid collision, taking into account the collision offset
            targetPosition = -(distance - cameraCollisionOffset);
        }

        // Ensure that the adjusted target position is not below the minimum collision offset
        if (Mathf.Abs(targetPosition) < minCollisionOffset)
        {
            targetPosition = targetPosition - minCollisionOffset;
        }

        // Use Lerp to smoothly update the camera's local position along the z-axis
        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }

    public void isPlayerScope()
    {
        if (inputManager.scopeInput)
        {
            Vector3 newPosition = cameraTransform.localPosition;
            newPosition.z = 1f;
            cameraTransform.localPosition = newPosition;

            scopeCanvas.SetActive(true);
            playerUI.SetActive(false);
            bodyPlayer.SetActive(false);
            mainCamera.fieldOfView = 30f;

            isScoped = true;


        }
        else
        {
            Vector3 newPosition = cameraTransform.localPosition;
            newPosition.z = defaultPosition;
            cameraTransform.localPosition = newPosition;

            scopeCanvas.SetActive(false);
            playerUI.SetActive(true);
            mainCamera.fieldOfView = originalFOV;
        }
    }
}
