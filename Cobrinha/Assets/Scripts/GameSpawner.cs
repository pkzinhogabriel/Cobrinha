using Unity.Netcode;
using UnityEngine;

public class GameSpawner : NetworkBehaviour
{
    public GameObject snakePrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Spawn para o Host
            SpawnPlayer(NetworkManager.Singleton.LocalClientId);

            NetworkManager.Singleton.OnClientConnectedCallback += clientId =>
            {
                SpawnPlayer(clientId);
            };
        }
    }

    void SpawnPlayer(ulong clientId)
    {
        Vector3 spawnPosition = Vector3.zero;

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            spawnPosition = new Vector3(-1f, 0f, 0f); // Exemplo: spawn do player 1 à esquerda
        }
        else
        {
            spawnPosition = new Vector3(1f, 0f, 0f); // Exemplo: spawn do player 2 à direita
        }

        GameObject snake = Instantiate(snakePrefab, spawnPosition, Quaternion.identity);
        snake.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }
}
