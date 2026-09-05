using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoSingleton<PlayerInputManager>
{
    public enum InputMode { PC, Mobile }

    [SerializeField] InputMode inputMode = InputMode.PC;
    [SerializeField] bool detectPlatform = true;
    [SerializeField] TPSController tpsController;
    [SerializeField] InputActionAsset inputActionAsset;
    [SerializeField] GameObject mobileControls;
    [SerializeField] RectTransform leftZone, joystickRoot, joystickHandle;
    [SerializeField] MobilePointerZone leftPointer, rightPointer, jumpPointer;

    InputAction lookAction, aimAction, moveAction, jumpAction;
    Vector2 lastLookPosition, lockedMousePosition;
    float joystickRange;
    int movePointerId = -1, lookPointerId = -1;

    public InputMode CurrentMode => inputMode;

    void Awake()
    {
        if (detectPlatform)
            inputMode = Application.isMobilePlatform ? InputMode.Mobile : InputMode.PC;

        var map = inputActionAsset.FindActionMap("Player");
        lookAction = map.FindAction("Look");
        aimAction = map.FindAction("Aim");
        moveAction = map.FindAction("Move");
        jumpAction = map.FindAction("Jump");
        map.Enable();

        joystickRange = joystickRoot.sizeDelta.x * 0.5f;
        leftPointer.Pressed = OnMoveDown;
        leftPointer.Dragged = OnMoveDrag;
        leftPointer.Released = OnMoveUp;
        rightPointer.Pressed = OnLookDown;
        rightPointer.Dragged = OnLookDrag;
        rightPointer.Released = OnLookUp;
        jumpPointer.Pressed = _ => tpsController.RequestJump();
        Apply();
    }

    void OnDisable() => Cursor.lockState = CursorLockMode.None;

    void OnValidate()
    {
        if (Application.isPlaying)
            Apply();
    }

    public void SetInputMode(InputMode mode)
    {
        inputMode = mode;
        Apply();
    }

    void Update()
    {
        if (inputMode == InputMode.Mobile)
            return;

        tpsController.SetMoveInput(moveAction.ReadValue<Vector2>());
        if (jumpAction.WasPressedThisFrame())
            tpsController.RequestJump();

        if (Mouse.current == null)
            return;

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

        if (aimAction.IsPressed())
            tpsController.AddLookDelta(lookAction.ReadValue<Vector2>());
    }

    void Apply()
    {
        tpsController.SetMoveInput(Vector2.zero);
        movePointerId = lookPointerId = -1;
        Cursor.lockState = CursorLockMode.None;
        joystickRoot.gameObject.SetActive(false);
        mobileControls.SetActive(inputMode == InputMode.Mobile);
        if (inputMode == InputMode.Mobile)
            jumpPointer.transform.SetAsLastSibling();
    }

    void OnMoveDown(PointerEventData e)
    {
        if (inputMode != InputMode.Mobile || movePointerId != -1)
            return;

        movePointerId = e.pointerId;
        if (ToLocal(leftZone, e, out Vector2 local))
        {
            Rect rect = leftZone.rect;
            local.x = Mathf.Clamp(local.x, rect.xMin + joystickRange, rect.xMax - joystickRange);
            local.y = Mathf.Clamp(local.y, rect.yMin + joystickRange, rect.yMax - joystickRange);
            joystickRoot.anchoredPosition = local;
        }

        joystickHandle.anchoredPosition = Vector2.zero;
        joystickRoot.gameObject.SetActive(true);
        tpsController.SetMoveInput(Vector2.zero);
    }

    void OnMoveDrag(PointerEventData e)
    {
        if (inputMode != InputMode.Mobile || e.pointerId != movePointerId || !ToLocal(joystickRoot, e, out Vector2 local))
            return;

        Vector2 clamped = Vector2.ClampMagnitude(local, joystickRange);
        joystickHandle.anchoredPosition = clamped;
        tpsController.SetMoveInput(clamped / joystickRange);
    }

    void OnMoveUp(PointerEventData e)
    {
        if (e.pointerId != movePointerId)
            return;

        movePointerId = -1;
        joystickHandle.anchoredPosition = Vector2.zero;
        joystickRoot.gameObject.SetActive(false);
        tpsController.SetMoveInput(Vector2.zero);
    }

    void OnLookDown(PointerEventData e)
    {
        if (inputMode != InputMode.Mobile || lookPointerId != -1)
            return;

        lookPointerId = e.pointerId;
        lastLookPosition = e.position;
    }

    void OnLookDrag(PointerEventData e)
    {
        if (inputMode != InputMode.Mobile || e.pointerId != lookPointerId)
            return;

        tpsController.AddLookDelta(e.position - lastLookPosition, true);
        lastLookPosition = e.position;
    }

    void OnLookUp(PointerEventData e)
    {
        if (e.pointerId == lookPointerId)
            lookPointerId = -1;
    }

    bool ToLocal(RectTransform rect, PointerEventData e, out Vector2 local) =>
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, e.position, e.pressEventCamera, out local);
}
