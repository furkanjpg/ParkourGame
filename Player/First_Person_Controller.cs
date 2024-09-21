using System.Collections;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravity = 9.81f;

    [Header("Camera Settings")]
    [SerializeField] private float lookSensitivity = 2.0f;
    [SerializeField] private float crouchCameraOffset = 0.5f;
    [SerializeField] private float minYRotation = -90f;
    [SerializeField] private float maxYRotation = 90f;

    [Header("Camera Tilt and Elevation")]
    [SerializeField] private float cameraTiltAdjustment = 0.0f; // Manual tilt adjustment
    [SerializeField] private float cameraElevationAdjustment = 0.0f; // Manual elevation adjustment

    [Header("Control Keys")]
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    private Camera mainCamera;
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private bool isCrouching = false;
    private bool crouchRequested = false;
    private bool uncrouchRequested = false;

    private float originalCameraY;
    private float xRotation = 0f;
    private float yRotation = 0f;

    [SerializeField] private float crouchHeight = 1.0f;
    private float originalHeight;
    private Vector3 originalCenter;

    private bool isSprinting = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalCameraY = mainCamera.transform.localPosition.y;

        originalHeight = characterController.height;
        originalCenter = characterController.center;
    }

    void Update()
    {
        HandleMovement();
        HandleCameraRotation();

        if (Input.GetKeyDown(crouchKey) && !isCrouching)
        {
            crouchRequested = true;
        }
        else if (Input.GetKeyUp(crouchKey) && isCrouching)
        {
            uncrouchRequested = true;
        }

        HandleCrouchCamera();
        ApplyGravityAndJumping();
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleMovement()
    {
        float speedMultiplier = isSprinting ? sprintMultiplier : (isCrouching ? crouchSpeedMultiplier : 1.0f);

        float verticalSpeed = Input.GetAxis("Vertical") * walkSpeed * speedMultiplier;
        float horizontalSpeed = Input.GetAxis("Horizontal") * walkSpeed * speedMultiplier;

        Vector3 movement = transform.forward * verticalSpeed + transform.right * horizontalSpeed;

        moveDirection.x = movement.x;
        moveDirection.z = movement.z;

        isSprinting = Input.GetKey(sprintKey) && !isCrouching;
    }

    void ApplyGravityAndJumping()
    {
        if (characterController.isGrounded)
        {
            moveDirection.y = -0.1f;

            if (Input.GetKeyDown(jumpKey) && !isCrouching)
            {
                moveDirection.y = jumpForce;
            }
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
    }

    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        yRotation += mouseX;
        transform.localRotation = Quaternion.Euler(0, yRotation, 0);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minYRotation, maxYRotation);

        // Apply camera tilt and elevation adjustments
        mainCamera.transform.localRotation = Quaternion.Euler(xRotation + cameraTiltAdjustment, 0, 0);
        mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, originalCameraY + cameraElevationAdjustment, mainCamera.transform.localPosition.z);
    }

    void HandleCrouchCamera()
    {
        if (crouchRequested)
        {
            crouchRequested = false;
            isCrouching = true;
            StartCoroutine(CrouchTransition(originalCameraY - crouchCameraOffset));
            Crunch(true);
        }
        else if (uncrouchRequested)
        {
            uncrouchRequested = false;
            isCrouching = false;
            StartCoroutine(CrouchTransition(originalCameraY));
            Crunch(false);
        }
    }

    IEnumerator CrouchTransition(float targetHeight)
    {
        float duration = 0.2f;
        float elapsedTime = 0.0f;
        float startHeight = mainCamera.transform.localPosition.y;

        while (elapsedTime < duration)
        {
            float newHeight = Mathf.Lerp(startHeight, targetHeight, elapsedTime / duration);
            mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, newHeight, mainCamera.transform.localPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, targetHeight, mainCamera.transform.localPosition.z);
    }

    void Crunch(bool crouching)
    {
        // Adjust CharacterController height and center based on crouch state
        characterController.height = crouching ? crouchHeight : originalHeight;
        characterController.center = new Vector3(0, characterController.height / 2, 0);
    }
}
