using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    private NetworkObjectReference player1Ref;
    private NetworkObjectReference player2Ref;

    void Awake() {
        if (Instance == null){Instance = this;} 
        else {Destroy(gameObject);}
    }

    public override void OnNetworkSpawn() {
        if (IsServer) {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId) {
        Debug.Log("Client " + clientId.ToString() + " connected");
        int clientCount = NetworkManager.Singleton.ConnectedClients.Count;

        // Start game once two players
        if (clientCount == 2)
        {
            StartGame();
        }
    }

    private void StartGame() {
        // Set references to player objects
        foreach (var client in NetworkManager.Singleton.ConnectedClients) {
            ulong clientId = client.Key;
            NetworkObject networkObject = client.Value.PlayerObject;

            if (clientId == NetworkManager.ServerClientId) {
                player1Ref = new NetworkObjectReference(networkObject);
            } else {
                player2Ref = new NetworkObjectReference(networkObject);
            }
        }

        // Initialize rope
        Rope.Instance.InitRopeServerRpc(player1Ref, player2Ref);

        // Start waves
        WaveManager.Instance.StartWave();
    }
}
