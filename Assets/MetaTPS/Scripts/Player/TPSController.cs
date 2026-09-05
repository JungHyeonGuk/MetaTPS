using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] float cameraCollisionRadius = 0.2f;
    [SerializeField] float minCameraDistance = 0.15f;
    [SerializeField] float cameraReturnSpeed = 12f;
    [SerializeField] float jumpSpeed = 6f;

    InputAction lookAction, aimAction, moveAction, jumpAction;
    Vector2 lockedMousePosition;
    Vector3 moveDir;
    float desiredCameraDistance;
    bool jumpPressed;



    void Start()
    {
        var map = inputActionAsset.FindActionMap("Player");
        lookAction = map.FindAction("Look");
        aimAction = map.FindAction("Aim");
        moveAction = map.FindAction("Move");
        jumpAction = map.FindAction("Jump");

        Transform cam = Camera.main.transform;
        cam.SetParent(cameraPivot);
        cam.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        if (cam.TryGetComponent(out Camera camera))
            camera.nearClipPlane = Mathf.Min(camera.nearClipPlane, 0.15f);

        desiredCameraDistance = Mathf.Abs(cameraPivot.localPosition.z);
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 forward = Vector3.ProjectOnPlane(cameraArm.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cameraArm.right, Vector3.up).normalized;

        moveDir = Vector3.ClampMagnitude(forward * input.y + right * input.x, 1f);
        bool isMove = moveDir.sqrMagnitude > 0.01f;
        animator.SetBool("IsMove", isMove);

        if (isMove)
        {
            characterBody.rotation = Quaternion.Slerp(
                characterBody.rotation, Quaternion.LookRotation(moveDir), rotationSpeed * Time.deltaTime);
        }

        if (jumpAction.WasPressedThisFrame())
            jumpPressed = true;

        if (aimAction.WasPressedThisFrame())
        {
            lockedMousePosition = Mouse.current.position.ReadValue();
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (aimAction.WasReleasedThisFrame())
        {
            Cursor.lockState = CursorLockMode.None;
            Mouse.current.WarpCursorPosition(lockedMousePosition);
        }

        if (!aimAction.IsPressed())
            return;

        Vector2 delta = lookAction.ReadValue<Vector2>() * lookSensitivity;
        cameraArm.Rotate(Vector3.up, delta.x, Space.World);
        cameraArm.Rotate(Vector3.right, -delta.y, Space.Self);

        Vector3 angles = cameraArm.eulerAngles;
        float pitch = Mathf.Clamp(Mathf.DeltaAngle(0f, angles.x), minPitch, maxPitch);
        cameraArm.rotation = Quaternion.Euler(pitch, angles.y, 0f);
    }

    void LateUpdate()
    {
        float targetDistance = desiredCameraDistance;
        if (Physics.SphereCast(
                cameraArm.position, cameraCollisionRadius, -cameraArm.forward, out RaycastHit hit,
                desiredCameraDistance)
            && hit.rigidbody != characterRigidbody)
        {
            targetDistance = Mathf.Max(hit.distance, minCameraDistance);
        }

        float currentDistance = -cameraPivot.localPosition.z;
        float nextDistance = targetDistance < currentDistance
            ? targetDistance
            : Mathf.MoveTowards(currentDistance, targetDistance, cameraReturnSpeed * Time.deltaTime);

        if (nextDistance != currentDistance)
            cameraPivot.localPosition = new Vector3(0f, 0f, -nextDistance);
    }

    void FixedUpdate()
    {
        Vector3 velocity = moveDir * moveSpeed;
        velocity.y = characterRigidbody.linearVelocity.y;

        if (jumpPressed)
        {
            jumpPressed = false;
            if (IsGrounded())
                velocity.y = jumpSpeed;
        }

        characterRigidbody.linearVelocity = velocity;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(
                   characterRigidbody.position + Vector3.up * 0.2f, Vector3.down, out RaycastHit hit, 0.3f)
               && hit.rigidbody != characterRigidbody;
    }
}
