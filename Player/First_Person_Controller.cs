using System.Collections;
using UnityEngine;

public class First_Person_Controller : MonoBehaviour
{
    [Header("SUPER SIMPLE MOVEMENT")]
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeedGroundedVertical = 3.0f;
    [SerializeField] private float walkSpeedGroundedHorizontal = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;
    [SerializeField] private float slideMultiplier = 5.0f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravity = 9.81f;

    [Header("Look Sensitivity")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float upDownRange = 80.0f;
    [SerializeField] private float mouseSmoothing = 2.0f;
    [SerializeField] private float slideCameraOffset = 0.3f;

    [Header("Inputs Customization")]
    [SerializeField] private string horizontalMoveInput = "Horizontal";
    [SerializeField] private string verticalMoveInput = "Vertical";
    [SerializeField] private string mouseXInput = "Mouse X";
    [SerializeField] private string mouseYInput = "Mouse Y";
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode slideKey = KeyCode.C;

    [Header("Inputs Crunch")]
    [SerializeField] private float crunchMoveInput = 0.3f;
    [SerializeField] private KeyCode crunchKey = KeyCode.LeftControl;

    // Bobbing effect
    [Header("Head Bobbing")]
    [SerializeField] private float bobbingSpeed = 14.0f; // Speed of bobbing
    [SerializeField] private float bobbingAmount = 0.05f; // Amount of vertical movement during bobbing

    private bool canCrouch = true; // Crouch yapabilmek için bir cooldown
    private bool canSlide = true; // To track if the slide is on cooldown

    private Camera mainCamera;
    private float verticalRotation;
    private Vector3 currentMovement = Vector3.zero;
    private Vector2 currentMouseDelta = Vector2.zero;
    private CharacterController characterController;

    bool isCtrlPressed = false;
    bool isShiftPressed = false;
    bool isSlidePressed = false;

    float originalCameraPositionY;
    float targetCameraPositionY;

    float bobbingTimer = 0.0f; // Timer for the bobbing effect
    [SerializeField] private float crouchHeight = 1.0f;  // Eðilme sýrasýnda karakterin yeni yüksekliði
    private float originalHeight;  // Karakterin orijinal yüksekliði
    private Vector3 originalCenter;  // Karakterin orijinal merkez pozisyonu



    void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalCameraPositionY = mainCamera.transform.localPosition.y;
        targetCameraPositionY = originalCameraPositionY - slideCameraOffset;

        // Karakterin orijinal yüksekliðini ve merkezini kaydet
        originalHeight = characterController.height;
        originalCenter = characterController.center;
    }


    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleBobbing(); // Call the bobbing method

        if (canCrouch)
        {
            if (Input.GetKeyDown(crunchKey))
            {
                isCtrlPressed = true;
                Crunch(true);
                StartCoroutine(CrouchCooldown()); // Cooldown baþlatýlýyor
            }
            else if (Input.GetKeyUp(crunchKey))
            {
                isCtrlPressed = false;
                Crunch(false);
                StartCoroutine(CrouchCooldown()); // Cooldown baþlatýlýyor
            }
        }

        if (Input.GetKeyDown(crunchKey))
        {
            isCtrlPressed = true;
            Crunch(true);
        }
        else if (Input.GetKeyUp(crunchKey))
        {
            isCtrlPressed = false;
            Crunch(false);
        }

        if (Input.GetKeyDown(sprintKey))
        {
            isShiftPressed = true;
        }
        else if (Input.GetKeyUp(sprintKey))
        {
            isShiftPressed = false;
        }

        if (Input.GetKeyDown(slideKey) && characterController.isGrounded && canSlide)
        {
            isSlidePressed = true;
            StartCoroutine(SlideCamera(true));
            StartCoroutine(SlideCooldown()); // Start the cooldown coroutine
        }
        else if (Input.GetKeyUp(slideKey))
        {
            isSlidePressed = false;
            StartCoroutine(SlideCamera(false));
        }

        if (isCtrlPressed && isShiftPressed)
        {
            isShiftPressed = false;
        }
    }

    // Slide cooldown coroutine
    IEnumerator SlideCooldown()
    {
        canSlide = false; // Disable sliding
        yield return new WaitForSeconds(2f); // Cooldown for 2 seconds
        canSlide = true; // Re-enable sliding after cooldown
    }

    IEnumerator CrouchCooldown()
    {
        canCrouch = false; // Crouch yapýlamaz
        yield return new WaitForSeconds(0.3f); // 0.3 saniye bekler
        canCrouch = true;  // Crouch tekrar yapýlabilir
    }

    void HandleMovement()
    {
        float speedMultiplier = isShiftPressed ? sprintMultiplier : 1f;

        float verticalSpeed;
        float horizontalSpeed;

        if (characterController.isGrounded)
        {
            verticalSpeed = Input.GetAxis(verticalMoveInput) * walkSpeedGroundedVertical * speedMultiplier;
            horizontalSpeed = Input.GetAxis(horizontalMoveInput) * walkSpeedGroundedHorizontal * speedMultiplier;
        }
        else
        {
            verticalSpeed = Input.GetAxis(verticalMoveInput) * walkSpeedGroundedVertical * speedMultiplier;
            horizontalSpeed = Input.GetAxis(horizontalMoveInput) * walkSpeedGroundedHorizontal * speedMultiplier;
        }

        float strafe = Input.GetAxis("Horizontal") * walkSpeedGroundedHorizontal * speedMultiplier;
        float backwards = Input.GetAxis("Vertical") * walkSpeedGroundedVertical * speedMultiplier;

        if (isSlidePressed)
        {
            verticalSpeed *= slideMultiplier;
            horizontalSpeed *= slideMultiplier;
        }

        Vector3 horizontalMovement = new Vector3(strafe, 0, verticalSpeed + backwards);
        horizontalMovement = transform.rotation * horizontalMovement;

        HandleGravityAndJumping();

        currentMovement.x = horizontalMovement.x;
        currentMovement.z = horizontalMovement.z;

        characterController.Move(currentMovement * Time.deltaTime);
    }

    void HandleGravityAndJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;

            if (Input.GetKey(jumpKey))
            {
                currentMovement.y = jumpForce;
            }
            else
            {
                currentMovement.y = -gravity * Time.deltaTime;
            }
        }
        else
        {
            currentMovement.y -= gravity * Time.deltaTime;
        }
    }

    void HandleRotation()
    {
        float mouseXRotation = Input.GetAxis(mouseXInput) * mouseSensitivity;
        float mouseYRotation = Input.GetAxis(mouseYInput) * mouseSensitivity;

        currentMouseDelta.x = Mathf.Lerp(currentMouseDelta.x, mouseXRotation, 1f / mouseSmoothing);
        currentMouseDelta.y = Mathf.Lerp(currentMouseDelta.y, mouseYRotation, 1f / mouseSmoothing);

        verticalRotation -= currentMouseDelta.y;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);

        transform.Rotate(Vector3.up * currentMouseDelta.x);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    // Head bobbing effect while walking
    void HandleBobbing()
    {
        if (characterController.isGrounded && characterController.velocity.magnitude > 0)
        {
            bobbingTimer += Time.deltaTime * bobbingSpeed;

            float newCameraYPosition = originalCameraPositionY + Mathf.Sin(bobbingTimer) * bobbingAmount;
            mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, newCameraYPosition, mainCamera.transform.localPosition.z);
        }
        else
        {
            bobbingTimer = 0.0f; // Reset bobbing when not moving
            mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, originalCameraPositionY, mainCamera.transform.localPosition.z);
        }
    }

    IEnumerator SlideCamera(bool isSliding)
    {
        float elapsedTime = 0f;
        float duration = 0.3f;

        while (elapsedTime < duration)
        {
            float yOffset = isSliding ? Mathf.Lerp(0, -slideCameraOffset, elapsedTime / duration) :
                                          Mathf.Lerp(-slideCameraOffset, 0, elapsedTime / duration);

            mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x,
                                                             originalCameraPositionY + yOffset,
                                                             mainCamera.transform.localPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x,
                                                         isSliding ? targetCameraPositionY : originalCameraPositionY,
                                                         mainCamera.transform.localPosition.z);
    }

    void Crunch(bool isCrouching)
    {
        if (isCrouching)
        {
            // Yalnýzca yukarýdan küçültme yapýlacak
            characterController.height = crouchHeight;
            characterController.center = new Vector3(originalCenter.x, originalCenter.y - (originalHeight - crouchHeight) / 2, originalCenter.z);

            Vector3 position = mainCamera.transform.localPosition;
            position.y -= crunchMoveInput;
            mainCamera.transform.localPosition = position;
        }
        else
        {
            // Orijinal yüksekliðe geri dönüyoruz
            characterController.height = originalHeight;
            characterController.center = originalCenter;

            Vector3 position = mainCamera.transform.localPosition;
            position.y += crunchMoveInput;
            mainCamera.transform.localPosition = position;
        }
    }
}
