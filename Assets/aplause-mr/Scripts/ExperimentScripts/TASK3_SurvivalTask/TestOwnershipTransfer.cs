using Unity.Netcode;
using UnityEngine;

public class TestOwnershipTransfer : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void RequestOwnership(NetworkObject networkObject)
    {
        if (networkObject == null || !networkObject.IsSpawned)
        {
            Debug.LogWarning("NetworkObject is not valid.");
            return;
        }

        RequestOwnershipServerRpc(networkObject.NetworkObjectId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(ulong objectId, ServerRpcParams rpcParams = default)
    {
        Debug.Log("Got ownership request");

        // Ensure the object exists
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject networkObject))
        {
            // TODO check if grabbed

            // Change ownership to the requesting client
            networkObject.ChangeOwnership(rpcParams.Receive.SenderClientId);
        }
        else
        {
            Debug.LogWarning("Requested NetworkObject not found.");
        }
    }

    private void TrytoTransferOwnership()
    {
        if (8 != GetComponent<NetworkObject>().NetworkObjectId)
        {
            return;
        }

        if (!GetComponent<NetworkBehaviour>().IsOwner)
        {
            Debug.Log("not the owner");
            RequestOwnership(GetComponent<NetworkObject>());
        }
        else
        {
            Debug.Log("owner");

            transform.position = transform.position + new Vector3(0.1f, 0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            TrytoTransferOwnership();
        }
    }
}
