using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Snake : MonoBehaviour // Classe responsavel por controlar o comportamento da cobra no jogo.
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
    
    void Start() // Metodo chamado no inicio do jogo.
    {
        direction = Vector2.up;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.R)) gameManager.Restart(); // Reinicia o jogo se apertar a tecla R apos o game over.
            return;
        }

        ChangeDirection();
        Move();
        CheckBodyCollisions();
    }
    void ChangeDirection() // Metodo para alterar a direcao da cobra com base na entrada do jogador.
    {
        Vector2 newdirection = Vector2.zero;
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        // Altera a direcao conforme a tecla pressionada.
        if (input.y == -1) newdirection = Vector2.down;
        else if (input.y == 1) newdirection = Vector2.up;
        else if (input.x == -1) newdirection = Vector2.left;
        else if (input.x == 1) newdirection = Vector2.right;
        if (newdirection + newdirection != Vector2.zero && newdirection != Vector2.zero)
        {
            direction = newdirection;
        }
    }
    void Move() // Metodo que move a cobra na direcao definida.
    {
        if (Time.time > changeCellTime)
        {
            for (int i = body.Count - 1; i > 0; i--)
            {
                body[i].position = body[i - 1].position;
            }
            if (body.Count > 0) body[0].position = (Vector2)transform.position;

            transform.position += (Vector3)direction * cellSize;

            changeCellTime = Time.time + 1 / speed;
            cellIndex = transform.position / cellSize;

            CheckWallWrapAround();
        }
    }
    void CheckWallWrapAround() // Metodo que verifica se a cobra atravessou a parede e ajusta a posicao.
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
    public void GrowBody()  // Metodo responsavel por aumentar o corpo da cobra ao comer alimento.
    {
        Vector2 position = transform.position;
        if (body.Count != 0)
            position = body[body.Count - 1].position;

        body.Add(Instantiate(bodyPrefab, position, Quaternion.identity).transform);
        gameManager.UpdateScore(1);
    }
    void CheckBodyCollisions() // Metodo que verifica se a cobra colidiu com o proprio corpo.
    {
        if (body.Count < 3) return;
        for (int i = 0; i < body.Count; i++)
        {
            Vector2 index = body[i].position / cellSize;
            if (Mathf.Abs(index.x - cellIndex.x)< 0.00001f && Mathf.Abs(index.y - cellIndex.y)<0.00001f)
            {
                GameOver();
                break;
            }
        }
    }
    void GameOver() // Metodo que termina o jogo ao ocorrer uma colisao.
    {
        gameOver = true;
        gameManager.GameOver();

    }
    public void Restart() // Metodo responsavel por reiniciar o jogo e resetar o estado da cobra.
    {
        gameOver = false;

        // Limpar corpo da cobra
        for (int i = 0; i < body.Count; ++i)
        {
            Destroy(body[i].gameObject);
        }
        body.Clear();

        // Resetar posicao da cobra
        transform.position = Vector3.zero;
    }
    public float GetWidth() // Metodo que retorna a largura da area de jogo.
    {
        return gameWidth;
    }
    public float GetHeight()  // Metodo que retorna a altura da area de jogo.
    {
        return gameHeight;
    }
    public void SetSpeed(float newSpeed) // Metodo para ajustar a velocidade da cobra.
    {
        speed = newSpeed;
    }
    public void SetGameArea(float width, float height) // Metodo para ajustar a area de jogo.
    {
        // Ajuste a lógica para criar paredes com base nos novos valores
        CreateWalls(width, height);
    }
    // Adicionar a matriz de grid para paredes


void CreateWalls(float width, float height)
{
    // Armazenar largura e altura da área de jogo
    gameWidth = width;
    gameHeight = height;

    // Calcular os limites da área de jogo
    int cellX = Mathf.FloorToInt(width / cellSize / 2);
    int cellY = Mathf.FloorToInt(height / cellSize / 2);

    // Inicializar a matriz de grid para paredes com base no tamanho do campo
    wallGrid = new int[cellX * 2 + 1, cellY * 2 + 1];

    // Limpar paredes existentes
    foreach (GameObject wall in GameObject.FindGameObjectsWithTag("Wall"))
    {
        Destroy(wall);
    }

    // Criar paredes superior e inferior
    for (int i = -cellX; i <= cellX; i++)
    {
        Vector2 top = new Vector2(i * cellSize, cellY * cellSize);
        Vector2 bottom = new Vector2(i * cellSize, -cellY * cellSize);

        Instantiate(wallPrefab, top, Quaternion.identity).tag = "Wall";
        Instantiate(wallPrefab, bottom, Quaternion.identity).tag = "Wall";

        // Marcar as posições das paredes na matriz
        wallGrid[i + cellX, cellY] = 1;     // Parede superior
        wallGrid[i + cellX, 0] = 1;         // Parede inferior
    }

    // Criar paredes esquerda e direita
    for (int i = -cellY; i <= cellY; i++)
    {
        Vector2 left = new Vector2(-cellX * cellSize, i * cellSize);
        Vector2 right = new Vector2(cellX * cellSize, i * cellSize);

        Instantiate(wallPrefab, left, Quaternion.identity).tag = "Wall";
        Instantiate(wallPrefab, right, Quaternion.identity).tag = "Wall";

        // Marcar as posições das paredes na matriz
        wallGrid[0, i + cellY] = 1;         // Parede esquerda
        wallGrid[cellX * 2, i + cellY] = 1; // Parede direita
    }
}


}
