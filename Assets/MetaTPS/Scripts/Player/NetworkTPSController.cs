using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class NetworkTPSController : NetworkBehaviour
{
    [SerializeField] TPSController tpsController;
    [SerializeField] Transform characterBody;
    [SerializeField] Rigidbody characterRigidbody;
    [SerializeField] Animator animator;
    [SerializeField] float interpolation = 18f;

    readonly NetworkVariable<Vector3> netPosition = new(
        writePerm: NetworkVariableWritePermission.Owner);
    readonly NetworkVariable<Quaternion> netBodyRotation = new(
        Quaternion.identity, writePerm: NetworkVariableWritePermission.Owner);
    readonly NetworkVariable<bool> netIsMove = new(
        writePerm: NetworkVariableWritePermission.Owner);
    readonly NetworkVariable<bool> netIsLadder = new(
        writePerm: NetworkVariableWritePermission.Owner);
    readonly NetworkVariable<float> netLadderSpeed = new(
        writePerm: NetworkVariableWritePermission.Owner);

    void Awake()
    {
        if (tpsController == null)
            tpsController = GetComponent<TPSController>();
        if (characterRigidbody == null)
            characterRigidbody = GetComponent<Rigidbody>();
        if (animator == null)
            animator = GetComponent<Animator>();
        if (characterBody == null)
        {
            Transform body = transform.Find("CharacterBody");
            if (body != null)
                characterBody = body;
        }
        tpsController.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        tpsController.enabled = IsOwner;
        characterRigidbody.isKinematic = !IsOwner;
        characterRigidbody.useGravity = IsOwner;

        if (!IsOwner)
            return;

        tpsController.MoveToStartPoint();
        PlayerInputManager.Instance?.SetController(tpsController);
        PushState();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;

        PlayerInputManager.Instance?.SetController(null);
        tpsController.UnbindLocalCamera();
    }

    void LateUpdate()
    {
        if (!IsSpawned)
            return;

        if (IsOwner)
            PushState();
        else
            ApplyRemoteState();
    }

    void PushState()
    {
        netPosition.Value = characterRigidbody.position;
        netBodyRotation.Value = characterBody.rotation;
        netIsMove.Value = animator.GetBool("IsMove");
        netIsLadder.Value = animator.GetBool("IsLadder");
        netLadderSpeed.Value = animator.GetFloat("LadderSpeed");
    }

    void ApplyRemoteState()
    {
        Vector3 target = netPosition.Value;
        if ((characterRigidbody.position - target).sqrMagnitude > 25f)
            characterRigidbody.position = target;
        else
            characterRigidbody.position = Vector3.Lerp(
                characterRigidbody.position, target, 1f - Mathf.Exp(-interpolation * Time.deltaTime));

        characterBody.rotation = Quaternion.Slerp(
            characterBody.rotation, netBodyRotation.Value, 1f - Mathf.Exp(-interpolation * Time.deltaTime));

        animator.SetBool("IsMove", netIsMove.Value);
        animator.SetBool("IsLadder", netIsLadder.Value);
        animator.SetFloat("LadderSpeed", netLadderSpeed.Value);
    }
}
