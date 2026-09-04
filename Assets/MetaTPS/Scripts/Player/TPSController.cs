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
    [SerializeField] float lookSensitivity = 0.1f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 10f;


    InputAction lookAction;
    InputAction aimAction;
    InputAction moveAction;
    Vector2 lockedMousePosition;
    Vector3 moveDir;





    void Start()
    {
        var playerMap = inputActionAsset.FindActionMap("Player");
        lookAction = playerMap.FindAction("Look");
        aimAction = playerMap.FindAction("Aim");
        moveAction = playerMap.FindAction("Move");
        
        Transform camTransform = Camera.main.transform;
        camTransform.SetPositionAndRotation(cameraPivot.position, cameraPivot.rotation);
        camTransform.SetParent(cameraPivot);
    }

    void Update()
    {
        Move();
        RotateCameraArm();
    }

    void FixedUpdate() 
    {
        Vector3 velocity = moveDir * moveSpeed;
        velocity.y = characterRigidbody.linearVelocity.y;
        characterRigidbody.linearVelocity = velocity;
    }

    void RotateCameraArm() 
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
            LookAround();
        }
    }

    void LookAround() 
    {
        // Mouse X, Y
        Vector2 mouseDelta = lookAction.ReadValue<Vector2>() * lookSensitivity; 
        Vector3 camAngle = cameraArm.rotation.eulerAngles;

        float x = camAngle.x - mouseDelta.y;
        x = x < 180f ? Mathf.Clamp(x, -1f, 70f) : Mathf.Clamp(x, 335f, 361f);

        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }

    void Move()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        bool isMove = moveInput.magnitude > 0f;
        animator.SetBool("IsMove", isMove);

        if (isMove) 
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
            moveDir = (lookForward * moveInput.y + lookRight * moveInput.x).normalized;

            Quaternion cameraWorldRot = cameraArm.rotation;
            characterBody.rotation = Quaternion.Slerp(characterBody.rotation, 
                Quaternion.LookRotation(moveDir), Time.deltaTime * rotationSpeed);
            cameraArm.rotation = cameraWorldRot;
        }
        else 
        {
            moveDir = Vector3.zero;
        }
    }
}
