using Unity.Netcode.Components;
using Unity.Netcode;

public class ClientNetworkTransform : NetworkTransform
{
    // You can check isOwner or not in NetworkObject of Unity's Inspector
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        CanCommitToTransform = IsOwner; // Allowed by that player
    }

    protected override void Update()
    {
        base.Update();

        // Check NetworkManager is not null?
        if (NetworkManager != null)
        {
            // If yes, Check NetworkManager conntected to client (have player?) / NetworkManager is listening?
            if (NetworkManager.IsConnectedClient || NetworkManager.IsListening)
            {
                // If yes, Check can commit?
                if (CanCommitToTransform)
                {
                    TryCommitTransformToServer(transform, NetworkManager.LocalTime.Time); // commit transform and local time.
                }
            }
        }
    }

    protected override bool OnIsServerAuthoritative()
    {
        return false; // Not allowed by the server.
    }
}