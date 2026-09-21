using Unity.Netcode;
using UnityEngine;

public class NetworkSession : MonoBehaviour
{
    void Awake()
    {
        NetworkManager nm = NetworkManager.Singleton;
        if (nm == null)
            return;

        nm.NetworkConfig.ConnectionApproval = true;
        nm.ConnectionApprovalCallback = Approve;
    }

    void Start()
    {
        NetworkManager nm = NetworkManager.Singleton;
        if (nm != null && !nm.IsListening)
            nm.StartHost();
    }

    static void Approve(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        GameObject start = GameObject.Find("StartPoint");
        response.Approved = true;
        response.CreatePlayerObject = true;
        response.Position = start != null ? start.transform.position : Vector3.zero;
        response.Rotation = Quaternion.identity;
    }
}
