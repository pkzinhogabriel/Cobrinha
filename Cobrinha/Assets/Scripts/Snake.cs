using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Snake : NetworkBehaviour
{
    public Transform bodyPrefab;
    public Transform wallPrefab;
    public GameManager gameManager;

    private Vector2 direction;
    private float changeCellTime = 0;

    public List<Transform> body = new List<Transform>();
    public float speed = 10.0f;
    public float cellSize = 0.3f;
    public Vector2 cellIndex = Vector2.zero;

    private float gameWidth;
    private float gameHeight;

    private bool gameOver = false;

    private int[,] wallGrid;

    // Para sincronizar a posição da cabeça da cobra
    private NetworkVariable<Vector3> netPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // Para sincronizar a direção
    private NetworkVariable<Vector2> netDirection = new NetworkVariable<Vector2>(Vector2.up, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            direction = Vector2.up;
        }
        else
        {
            // Para clientes, atualizar posição e direção da cabeça conforme NetworkVariables
            direction = netDirection.Value;
            transform.position = netPosition.Value;
        }
    }

    void Update()
    {
        if (!IsOwner) // Apenas o dono controla o movimento
        {
            // Atualiza posição e direção com dados da rede
            direction = netDirection.Value;
            transform.position = netPosition.Value;
            return;
        }

        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.R)) gameManager.Restart();
            return;
        }

        ChangeDirection();
        Move();
        CheckBodyCollisions();

        // Atualiza as NetworkVariables
        netPosition.Value = transform.position;
        netDirection.Value = direction;
    }

    void ChangeDirection()
    {
        Vector2 newdirection = Vector2.zero;
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (input.y == -1) newdirection = Vector2.down;
        else if (input.y == 1) newdirection = Vector2.up;
        else if (input.x == -1) newdirection = Vector2.left;
        else if (input.x == 1) newdirection = Vector2.right;

        if (newdirection + direction != Vector2.zero && newdirection != Vector2.zero)
        {
            direction = newdirection;
        }
    }

    void Move()
    {
        if (Time.time > changeCellTime)
        {
            for (int i = body.Count - 1; i > 0; i--)
            {
                body[i].position = body[i - 1].position;
            }
            if (body.Count > 0) body[0].position = transform.position;

            transform.position += (Vector3)direction * cellSize;

            changeCellTime = Time.time + 1 / speed;
            cellIndex = (Vector2)transform.position / cellSize;

            CheckWallWrapAround();
        }
    }

    void CheckWallWrapAround()
    {
        if (transform.position.x > gameWidth / 2)
            transform.position = new Vector3(-gameWidth / 2 + 0.01f, transform.position.y, transform.position.z);
        else if (transform.position.x < -gameWidth / 2)
            transform.position = new Vector3(gameWidth / 2 - 0.01f, transform.position.y, transform.position.z);

        if (transform.position.y > gameHeight / 2)
            transform.position = new Vector3(transform.position.x, -gameHeight / 2 + 0.01f, transform.position.z);
        else if (transform.position.y < -gameHeight / 2)
            transform.position = new Vector3(transform.position.x, gameHeight / 2 - 0.01f, transform.position.z);
    }

    [ServerRpc(RequireOwnership = true)]
    public void GrowBodyServerRpc()
    {
        GrowBodyClientRpc();
    }

    [ClientRpc]
    void GrowBodyClientRpc()
    {
        Vector2 position = transform.position;
        if (body.Count != 0)
            position = body[body.Count - 1].position;

        Transform newSegment = Instantiate(bodyPrefab, position, Quaternion.identity);
        body.Add(newSegment);
        gameManager.UpdateScore(1);
    }

    void CheckBodyCollisions()
    {
        if (body.Count < 3) return;
        for (int i = 0; i < body.Count; i++)
        {
            Vector2 index = body[i].position / cellSize;
            if (Mathf.Abs(index.x - cellIndex.x) < 0.00001f && Mathf.Abs(index.y - cellIndex.y) < 0.00001f)
            {
                GameOver();
                break;
            }
        }
    }

    void GameOver()
    {
        gameOver = true;
        gameManager.GameOver();
    }

    public void Restart()
    {
        gameOver = false;

        for (int i = 0; i < body.Count; ++i)
        {
            Destroy(body[i].gameObject);
        }
        body.Clear();

        transform.position = Vector3.zero;
    }

    public float GetWidth()
    {
        return gameWidth;
    }

    public float GetHeight()
    {
        return gameHeight;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetGameArea(float width, float height)
    {
        gameWidth = width;
        gameHeight = height;

        CreateWalls(width, height);
    }

    void CreateWalls(float width, float height)
    {
        int cellX = Mathf.FloorToInt(width / cellSize / 2);
        int cellY = Mathf.FloorToInt(height / cellSize / 2);

        wallGrid = new int[cellX * 2 + 1, cellY * 2 + 1];

        foreach (GameObject wall in GameObject.FindGameObjectsWithTag("Wall"))
        {
            Destroy(wall);
        }

        for (int i = -cellX; i <= cellX; i++)
        {
            Vector2 top = new Vector2(i * cellSize, cellY * cellSize);
            Vector2 bottom = new Vector2(i * cellSize, -cellY * cellSize);

            Instantiate(wallPrefab, top, Quaternion.identity).tag = "Wall";
            Instantiate(wallPrefab, bottom, Quaternion.identity).tag = "Wall";

            wallGrid[i + cellX, cellY] = 1;
            wallGrid[i + cellX, 0] = 1;
        }

        for (int i = -cellY; i <= cellY; i++)
        {
            Vector2 left = new Vector2(-cellX * cellSize, i * cellSize);
            Vector2 right = new Vector2(cellX * cellSize, i * cellSize);

            Instantiate(wallPrefab, left, Quaternion.identity).tag = "Wall";
            Instantiate(wallPrefab, right, Quaternion.identity).tag = "Wall";

            wallGrid[0, i + cellY] = 1;
            wallGrid[cellX * 2, i + cellY] = 1;
        }
    }
}
