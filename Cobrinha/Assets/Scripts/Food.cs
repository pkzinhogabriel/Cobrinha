using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Classe responsável por gerar e gerenciar o alimento no jogo.
public class Food : MonoBehaviour
{
    public Transform foodPrefab; // Prefab do alimento a ser instanciado.
    public Snake snake; // Referência à cobra no jogo.

    private Transform currentFood;
    // Método chamado no início do jogo. Gera o primeiro alimento.
    void Start()
    {
        SpawnFood();
    }

    // Método chamado a cada frame. Verifica se a cobra comeu o alimento.
    void Update()
    {
        // Verifica se a cobra comeu o alimento
        Vector2 index = currentFood.position / snake.cellSize;
        if (Mathf.Abs(index.x - snake.cellIndex.x) < 0.5f && Mathf.Abs(index.y - snake.cellIndex.y) < 0.5f)
        {
            // Se a cobra comeu, gera um novo alimento
            SpawnFood();
            snake.GrowBody(); // Aumenta o corpo da cobra
        }
    }
    // Método que verifica se a posição está ocupada pela cobra.
    bool IsPositionOccupied(Vector2 position)
    {
        // Verifica se a posição coincide com a posição da cabeça da cobra
        if ((Vector2)snake.transform.position == position)
        {
            return true; // A posição está ocupada pela cabeça
        }

        // Verifica se a posição coincide com algum segmento do corpo da cobra
        foreach (Transform segment in snake.body)
        {
            if ((Vector2)segment.position == position)
            {
                return true; // A posição está ocupada
            }
        }
        return false; // A posição está livre
    }
     // Método responsável por gerar o alimento em uma posição aleatória.
    void SpawnFood()
    {
        // Obtém a largura e altura da área de jogo da cobra
        float width = snake.GetWidth();
        float height = snake.GetHeight();

        Vector2 randomPosition;

        // Continua gerando uma nova posição até encontrar uma que não esteja ocupada pela cobra
        do
        {
            float x = Random.Range(-width / 2 + snake.cellSize / 2, width / 2 - snake.cellSize / 2);
            float y = Random.Range(-height / 2 + snake.cellSize / 2, height / 2 - snake.cellSize / 2);
            randomPosition = new Vector2(x, y);
        }
        while (IsPositionOccupied(randomPosition));

        // Remove o alimento anterior, se existir
        if (currentFood != null)
        {
            Destroy(currentFood.gameObject);
        }

        // Instancia um novo alimento
        currentFood = Instantiate(foodPrefab, randomPosition, Quaternion.identity).transform;
    }
}
