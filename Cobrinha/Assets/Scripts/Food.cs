using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SpawnFood();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
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
