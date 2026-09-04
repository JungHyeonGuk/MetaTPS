using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TPSController : MonoBehaviour
{
    [SerializeField] Transform characterBody;
    [SerializeField] Rigidbody characterRigidbody;
    [SerializeField] Transform cameraArm;
    [SerializeField] Transform cameraPivot;
    [SerializeField] Animator animator;
    [SerializeField] InputActionAsset inputActionAsset;
    [SerializeField] float lookSensitivity = 0.12f;
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float rotationSpeed = 14f;
    [SerializeField] float minPitch = -80f;
    [SerializeField] float maxPitch = 70f;

    InputAction lookAction;
    InputAction aimAction;
    InputAction moveAction;
    Vector2 lockedMousePosition;
    Vector2 moveInput;
    float cameraYaw;
    float cameraPitch;



    void Start()
    {
        var playerMap = inputActionAsset.FindActionMap("Player");
        lookAction = playerMap.FindAction("Look");
        aimAction = playerMap.FindAction("Aim");
        moveAction = playerMap.FindAction("Move");

        Vector3 euler = cameraArm.localEulerAngles;
        cameraPitch = NormalizePitch(euler.x);
        cameraYaw = euler.y;

        AttachSceneCamera();
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        ReadMoveInput();
        HandleLook();
        RotateCharacter();
    }

    void FixedUpdate()
    {
        Vector3 planarVelocity = GetCameraPlanarMoveDirection() * moveSpeed;
        Vector3 velocity = planarVelocity;
        velocity.y = characterRigidbody.linearVelocity.y;
        characterRigidbody.linearVelocity = velocity;
    }

    void ReadMoveInput()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        if (moveInput.sqrMagnitude > 1f) 
        {
            moveInput.Normalize();
        }
        if (animator != null)
        {
            animator.SetBool("IsMove", moveInput.sqrMagnitude > 0.01f);
        }
    }

    void HandleLook()
    {
        if (aimAction.WasPressedThisFrame())
        {
            lockedMousePosition = Mouse.current.position.ReadValue();
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (aimAction.WasReleasedThisFrame())
        {
            Cursor.lockState = CursorLockMode.None;
            Mouse.current.WarpCursorPosition(lockedMousePosition);
        }

        if (aimAction.IsPressed()) 
        {
            Vector2 mouseDelta = lookAction.ReadValue<Vector2>() * lookSensitivity;
            cameraYaw += mouseDelta.x;
            cameraPitch = Mathf.Clamp(cameraPitch - mouseDelta.y, minPitch, maxPitch);
            cameraArm.localRotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
        }
    }

    void RotateCharacter()
    {
        Vector3 moveDir = GetCameraPlanarMoveDirection();

        if (moveDir.sqrMagnitude >= 0.001f) 
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            float t = 1f - Mathf.Exp(-rotationSpeed * Time.deltaTime);
            characterBody.rotation = Quaternion.Slerp(characterBody.rotation, targetRotation, t);
        }
    }

    Vector3 GetCameraPlanarMoveDirection()
    {
        if (moveInput.sqrMagnitude >= 0.0001f) 
        {
            Vector3 forward = Flatten(cameraArm.forward);
            Vector3 right = Flatten(cameraArm.right);
            Vector3 moveDir = forward * moveInput.y + right * moveInput.x;
            if (moveDir.sqrMagnitude > 1f) 
            {
                moveDir.Normalize();
            } 
            return moveDir;
        }
        else 
        {
            return Vector3.zero;
        }
    }

    static Vector3 Flatten(Vector3 direction)
    {
        direction.y = 0f;
        
        if (direction.sqrMagnitude < 0.001f) 
        {
            return Vector3.zero;
        }
        else 
        {
            return direction.normalized;
        }
    }

    static float NormalizePitch(float pitch)
    {
        if (pitch > 180f)
            pitch -= 360f;
        return pitch;
    }

    void AttachSceneCamera()
    {
        Transform camTransform = Camera.main.transform;
        camTransform.SetParent(cameraPivot);
        camTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}
