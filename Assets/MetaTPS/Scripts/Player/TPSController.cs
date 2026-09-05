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
    [SerializeField] float ladderSpeed = 3f;
    [SerializeField] float ladderJumpBack = 4f;

    PhysicsMaterial physicsMaterial;
    PhysicsMaterialCombine frictionCombine;
    Vector2 moveInput;
    Vector3 moveDir;
    float desiredCameraDistance;
    float staticFriction, dynamicFriction;
    float ignoreLadderUntil;
    bool jumpPressed, touchingWall;
    Ladder ladder;
    Collider playerCollider;

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

    void Awake()
    {
        playerCollider = GetComponent<Collider>();
        physicsMaterial = playerCollider.material;
        staticFriction = physicsMaterial.staticFriction;
        dynamicFriction = physicsMaterial.dynamicFriction;
        frictionCombine = physicsMaterial.frictionCombine;
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
        if (ladder != null && ShouldLeaveLadder())
            DropLadder();

        bool onLadder = ladder != null;
        animator.SetBool("IsLadder", onLadder);

        if (onLadder)
        {
            float climb = moveInput.y;
            animator.SetBool("IsMove", Mathf.Abs(climb) > 0.01f);
            animator.SetFloat("LadderSpeed", Mathf.Abs(climb) > 0.01f ? Mathf.Sign(climb) * 1.5f : 0f);

            Vector3 face = Vector3.ProjectOnPlane(ladder.transform.position - transform.position, Vector3.up);
            if (face.sqrMagnitude > 0.001f)
                characterBody.rotation = Quaternion.Slerp(
                    characterBody.rotation, Quaternion.LookRotation(face), rotationSpeed * Time.deltaTime);
            return;
        }

        animator.SetFloat("LadderSpeed", 0f);

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
        if (ladder != null && ShouldLeaveLadder())
            DropLadder();

        if (ladder != null)
        {
            physicsMaterial.staticFriction = 0f;
            physicsMaterial.dynamicFriction = 0f;
            physicsMaterial.frictionCombine = PhysicsMaterialCombine.Minimum;
            characterRigidbody.useGravity = false;

            if (jumpPressed)
            {
                jumpPressed = false;
                Vector3 away = Vector3.ProjectOnPlane(transform.position - ladder.transform.position, Vector3.up);
                if (away.sqrMagnitude < 0.0001f)
                    away = Vector3.ProjectOnPlane(-characterBody.forward, Vector3.up);
                away.Normalize();
                characterRigidbody.useGravity = true;
                characterRigidbody.linearVelocity = away * ladderJumpBack + Vector3.up * jumpSpeed;
                ignoreLadderUntil = Time.time + 0.4f;
                DropLadder();
                return;
            }

            float climb = moveInput.y * ladderSpeed;
            if (climb > 0f && transform.position.y >= ladder.TopY)
            {
                Vector3 p = characterRigidbody.position;
                p.y = ladder.TopY + 0.02f;
                characterRigidbody.position = p;
                characterRigidbody.linearVelocity = Vector3.zero;
                DropLadder();
                return;
            }

            characterRigidbody.linearVelocity = Vector3.up * climb;
            return;
        }

        characterRigidbody.useGravity = true;

        bool wall = touchingWall;
        touchingWall = false;

        bool grounded = Grounded(out RaycastHit hit);
        bool grip = grounded && !jumpPressed && !wall && characterRigidbody.linearVelocity.y <= 0f;

        physicsMaterial.staticFriction = grip ? staticFriction : 0f;
        physicsMaterial.dynamicFriction = grip ? dynamicFriction : 0f;
        physicsMaterial.frictionCombine = grip ? frictionCombine : PhysicsMaterialCombine.Minimum;

        Vector3 velocity = grip
            ? Vector3.ProjectOnPlane(moveDir * moveSpeed, hit.normal)
            : moveDir * moveSpeed;
        if (!grip)
            velocity.y = characterRigidbody.linearVelocity.y;

        if (Time.time < ignoreLadderUntil)
        {
            velocity.x = characterRigidbody.linearVelocity.x;
            velocity.z = characterRigidbody.linearVelocity.z;
        }

        if (jumpPressed)
        {
            jumpPressed = false;
            if (grounded)
                velocity.y = jumpSpeed;
        }

        characterRigidbody.linearVelocity = velocity;
        if (grip)
            characterRigidbody.AddForce(hit.normal * Physics.gravity.y, ForceMode.Acceleration);
    }

    void OnTriggerStay(Collider other)
    {
        if (Time.time < ignoreLadderUntil || ladder != null || !other.TryGetComponent(out Ladder hit))
            return;
        if (Grounded(out RaycastHit ground) && !WantsLadder(hit, ground))
            return;

        ladder = hit;
        Physics.IgnoreCollision(playerCollider, ladder.TopFloor, true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Ladder left) && left == ladder)
            DropLadder();
    }

    bool ShouldLeaveLadder()
    {
        if (!Grounded(out RaycastHit ground))
            return false;
        return OnTopFloor(ladder, ground) || moveInput.y <= 0f;
    }

    bool WantsLadder(Ladder hit, RaycastHit ground)
    {
        if (OnTopFloor(hit, ground))
            return moveInput.y < 0f;
        return moveInput.y > 0f;
    }

    bool OnTopFloor(Ladder l, RaycastHit ground) => ground.point.y >= l.TopY - 0.35f;

    void DropLadder()
    {
        if (ladder != null)
            Physics.IgnoreCollision(playerCollider, ladder.TopFloor, false);
        ladder = null;
        characterRigidbody.useGravity = true;
        animator.SetBool("IsLadder", false);
        animator.SetFloat("LadderSpeed", 0f);
    }

    bool Grounded(out RaycastHit hit)
    {
        return Physics.Raycast(
                   characterRigidbody.position + Vector3.up * 0.2f, Vector3.down, out hit, 0.45f)
               && hit.rigidbody != characterRigidbody
               && hit.normal.y > 0.35f;
    }

    void OnCollisionStay(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y < 0.35f)
            {
                touchingWall = true;
                return;
            }
        }
    }
}
