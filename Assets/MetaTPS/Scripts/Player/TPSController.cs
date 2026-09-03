using UnityEngine;
using UnityEngine.InputSystem;

public class TPSController : MonoBehaviour
{
    [SerializeField] Transform characterBody;
    [SerializeField] Transform cameraArm;
    [SerializeField] Transform cameraPivot;
    [SerializeField] Animator animator;
    [SerializeField] InputActionAsset inputActionAsset;
    [SerializeField] float lookSensitivity = 0.1f;


    InputAction lookAction;
    InputAction aimAction;
    Vector2 lockedMousePosition;





    void Start()
    {
        var playerMap = inputActionAsset.FindActionMap("Player");
        lookAction = playerMap.FindAction("Look");
        aimAction = playerMap.FindAction("Aim");

        Transform camTransform = Camera.main.transform;
        camTransform.SetPositionAndRotation(cameraPivot.position, cameraPivot.rotation);
        camTransform.SetParent(cameraPivot);
    }

    void Update()
    {
        if (aimAction.WasPressedThisFrame())
        {
            lockedMousePosition = Mouse.current.position.ReadValue();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (aimAction.IsPressed())
        {
            LookAround();
            Mouse.current.WarpCursorPosition(lockedMousePosition);
        }
        
    }

    void LookAround() 
    {
        // Mouse X, Y
        Vector2 mouseDelta = lookAction.ReadValue<Vector2>(); 
        Vector3 camAngle = cameraArm.rotation.eulerAngles;

        float x = camAngle.x - mouseDelta.y;
        x = x < 180f ? Mathf.Clamp(x, -1f, 70f) : Mathf.Clamp(x, 335f, 361f);

        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }
}
