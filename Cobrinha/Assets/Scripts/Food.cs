using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Classe respons�vel por gerar e gerenciar o alimento no jogo.
public class Food : MonoBehaviour
{
    public Transform foodPrefab; // Prefab do alimento a ser instanciado.
    public Snake snake; // Refer�ncia � cobra no jogo.

    private Transform currentFood;
    // M�todo chamado no in�cio do jogo. Gera o primeiro alimento.
    void Start()
    {
        SpawnFood();
    }

    // M�todo chamado a cada frame. Verifica se a cobra comeu o alimento.
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
    // M�todo que verifica se a posi��o est� ocupada pela cobra.
    bool IsPositionOccupied(Vector2 position)
    {
        // Verifica se a posi��o coincide com a posi��o da cabe�a da cobra
        if ((Vector2)snake.transform.position == position)
        {
            return true; // A posi��o est� ocupada pela cabe�a
        }

        // Verifica se a posi��o coincide com algum segmento do corpo da cobra
        foreach (Transform segment in snake.body)
        {
            if ((Vector2)segment.position == position)
            {
                return true; // A posi��o est� ocupada
            }
        }
        return false; // A posi��o est� livre
    }
     // M�todo respons�vel por gerar o alimento em uma posi��o aleat�ria.
    void SpawnFood()
    {
        // Obt�m a largura e altura da �rea de jogo da cobra
        float width = snake.GetWidth();
        float height = snake.GetHeight();

        Vector2 randomPosition;

        // Continua gerando uma nova posi��o at� encontrar uma que n�o esteja ocupada pela cobra
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
