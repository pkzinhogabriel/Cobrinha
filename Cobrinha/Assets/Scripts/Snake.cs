using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Snake : NetworkBehaviour
{
    public Transform bodyPrefab;
    public Transform wallPrefab;
    public GameManager gameManager;

    private Vector2 direction;
    private float changeCellTime = 0f;

    public List<Transform> body = new List<Transform>();

    public float speed = 5f; // Inicialize com valor padrão
    public float cellSize = 0.3f;
    public Vector2 cellIndex = Vector2.zero;

    private float gameWidth;
    private float gameHeight;

    private bool gameOver = false;

    private int[,] wallGrid;

    void Start()
    {
        direction = Vector2.up;

        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
    }

    void Update()
    {
        if (!IsOwner) return;
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                gameManager.Restart();
            }
            return;
        }

        ChangeDirection();

        Move();

        CheckBodyCollisions();
    }

    void ChangeDirection()
    {
        Vector2 newDirection = Vector2.zero;
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (input.y == -1) newDirection = Vector2.down;
        else if (input.y == 1) newDirection = Vector2.up;
        else if (input.x == -1) newDirection = Vector2.left;
        else if (input.x == 1) newDirection = Vector2.right;

        // Impede que a cobra inverta a direção diretamente
        if (newDirection != Vector2.zero && newDirection + direction != Vector2.zero)
        {
            direction = newDirection;
        }
    }

    void Move()
    {
        if (Time.time >= changeCellTime)
        {
            Vector3 previousPosition = transform.position;

            // Move o corpo da cobra seguindo a cabeça
            for (int i = body.Count - 1; i > 0; i--)
            {
                body[i].position = body[i - 1].position;
            }

            if (body.Count > 0)
                body[0].position = previousPosition;

            // Move a cabeça na direção atual
            transform.position += (Vector3)(direction * cellSize);

            changeCellTime = Time.time + 1f / speed;

            // Atualiza índice da célula baseado na posição da cabeça
            cellIndex = new Vector2(
                Mathf.Round(transform.position.x / cellSize),
                Mathf.Round(transform.position.y / cellSize)
            );

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

    public void GrowBody()
    {
        Vector2 position = transform.position;

        if (body.Count > 0)
        {
            position = body[body.Count - 1].position;
        }

        Transform newBodyPart = Instantiate(bodyPrefab, position, Quaternion.identity);
        body.Add(newBodyPart);

        gameManager.UpdateScore(1);
    }

    void CheckBodyCollisions()
    {
        if (body.Count < 3) return;

        for (int i = 0; i < body.Count; i++)
        {
            Vector2 index = new Vector2(
                Mathf.Round(body[i].position.x / cellSize),
                Mathf.Round(body[i].position.y / cellSize)
            );

            if (Mathf.Approximately(index.x, cellIndex.x) && Mathf.Approximately(index.y, cellIndex.y))
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

        for (int i = 0; i < body.Count; i++)
        {
            Destroy(body[i].gameObject);
        }

        body.Clear();

        transform.position = Vector3.zero;
        direction = Vector2.up;
        changeCellTime = 0;
    }

    public void SetSpeed(float newSpeed)
    {
        if (newSpeed <= 0f)
        {
            Debug.LogWarning("Speed must be greater than zero!");
            return;
        }
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
    public float GetWidth()
    {
        return gameWidth;
    }

    public float GetHeight()
    {
        return gameHeight;
    }

}
