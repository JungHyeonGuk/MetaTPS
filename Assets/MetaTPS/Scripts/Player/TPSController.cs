using UnityEngine;

public class TPSController : MonoBehaviour
{
    [SerializeField] Transform characterBody, cameraArm, cameraPivot;
    [SerializeField] Rigidbody characterRigidbody;
    [SerializeField] Animator animator;
    [SerializeField] float lookSensitivity = 0.7f;
    [SerializeField] float mobileLookSensitivity = 0.5f;
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float rotationSpeed = 14f;
    [SerializeField] float minPitch = -80f;
    [SerializeField] float maxPitch = 70f;
    [SerializeField] float cameraCollisionRadius = 0.2f;
    [SerializeField] float minCameraDistance = 0.15f;
    [SerializeField] float cameraReturnSpeed = 12f;
    [SerializeField] float jumpSpeed = 6f;

    Vector2 moveInput;
    Vector3 moveDir;
    float desiredCameraDistance;
    bool jumpPressed;

    public void SetMoveInput(Vector2 input) => moveInput = input;
    public void RequestJump() => jumpPressed = true;

    public void AddLookDelta(Vector2 delta, bool mobile = false)
    {
        delta *= mobile ? mobileLookSensitivity : lookSensitivity;
        if (delta.sqrMagnitude < 0.0001f)
            return;

        cameraArm.Rotate(Vector3.up, delta.x, Space.World);
        cameraArm.Rotate(Vector3.right, -delta.y, Space.Self);

        Vector3 angles = cameraArm.eulerAngles;
        cameraArm.rotation = Quaternion.Euler(
            Mathf.Clamp(Mathf.DeltaAngle(0f, angles.x), minPitch, maxPitch), angles.y, 0f);
    }

    void Start()
    {
        Transform cam = Camera.main.transform;
        cam.SetParent(cameraPivot);
        cam.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        if (cam.TryGetComponent(out Camera camera))
            camera.nearClipPlane = Mathf.Min(camera.nearClipPlane, 0.15f);

        desiredCameraDistance = Mathf.Abs(cameraPivot.localPosition.z);
    }

    void Update()
    {
        Vector3 forward = Vector3.ProjectOnPlane(cameraArm.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cameraArm.right, Vector3.up).normalized;
        moveDir = Vector3.ClampMagnitude(forward * moveInput.y + right * moveInput.x, 1f);

        bool isMove = moveDir.sqrMagnitude > 0.01f;
        animator.SetBool("IsMove", isMove);
        if (isMove)
            characterBody.rotation = Quaternion.Slerp(
                characterBody.rotation, Quaternion.LookRotation(moveDir), rotationSpeed * Time.deltaTime);
    }

    void LateUpdate()
    {
        float target = desiredCameraDistance;
        if (Physics.SphereCast(cameraArm.position, cameraCollisionRadius, -cameraArm.forward, out RaycastHit hit, desiredCameraDistance)
            && hit.rigidbody != characterRigidbody)
            target = Mathf.Max(hit.distance, minCameraDistance);

        float current = -cameraPivot.localPosition.z;
        float next = target < current ? target : Mathf.MoveTowards(current, target, cameraReturnSpeed * Time.deltaTime);
        if (next != current)
            cameraPivot.localPosition = new Vector3(0f, 0f, -next);
    }

    void FixedUpdate()
    {
        Vector3 velocity = moveDir * moveSpeed;
        velocity.y = characterRigidbody.linearVelocity.y;

        if (jumpPressed)
        {
            jumpPressed = false;
            if (Physics.Raycast(characterRigidbody.position + Vector3.up * 0.2f, Vector3.down, out RaycastHit hit, 0.3f)
                && hit.rigidbody != characterRigidbody)
                velocity.y = jumpSpeed;
        }

        characterRigidbody.linearVelocity = velocity;
    }
}
