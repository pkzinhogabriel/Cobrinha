using Unity.Netcode;
using UnityEngine;

public class Food : NetworkBehaviour
{
    public Transform foodPrefab;
    public Snake snake;

    private Transform currentFood;

    // NetworkVariable para sincronizar posição da comida
    private NetworkVariable<Vector3> netFoodPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SpawnFood();
        }
        else
        {
            UpdateFoodPositionClientRpc(netFoodPosition.Value);
        }
    }

    void Update()
    {
        if (!IsServer) return;

        if (currentFood == null) return;

        Vector2 index = currentFood.position / snake.cellSize;

        if (Mathf.Abs(index.x - snake.cellIndex.x) < 0.5f && Mathf.Abs(index.y - snake.cellIndex.y) < 0.5f)
        {
            snake.GrowBodyServerRpc();

            SpawnFood();
        }
    }

    bool IsPositionOccupied(Vector2 position)
    {
        if ((Vector2)snake.transform.position == position)
            return true;

        foreach (Transform segment in snake.body)
        {
            if ((Vector2)segment.position == position)
                return true;
        }
        return false;
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnFoodServerRpc()
    {
        SpawnFood();
    }

    void SpawnFood()
    {
        float width = snake.GetWidth();
        float height = snake.GetHeight();

        Vector2 randomPosition;

        do
        {
            float x = Random.Range(-width / 2 + snake.cellSize / 2, width / 2 - snake.cellSize / 2);
            float y = Random.Range(-height / 2 + snake.cellSize / 2, height / 2 - snake.cellSize / 2);
            randomPosition = new Vector2(x, y);
        }
        while (IsPositionOccupied(randomPosition));

        if (currentFood != null)
        {
            Destroy(currentFood.gameObject);
        }

        currentFood = Instantiate(foodPrefab, randomPosition, Quaternion.identity);

        netFoodPosition.Value = currentFood.position;

        UpdateFoodPositionClientRpc(currentFood.position);
    }

    [ClientRpc]
    void UpdateFoodPositionClientRpc(Vector3 position)
    {
        if (IsServer) return;

        if (currentFood == null)
        {
            currentFood = Instantiate(foodPrefab, position, Quaternion.identity);
        }
        else
        {
            currentFood.position = position;
        }
    }
}
