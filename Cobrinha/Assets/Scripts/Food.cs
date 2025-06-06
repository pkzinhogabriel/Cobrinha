using UnityEngine;

public class Food : MonoBehaviour
{
    public Transform foodPrefab;
    public Snake snake;

    private Transform currentFood;

    void Start()
    {
        SpawnFood();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Cabeça da cobra com tag "Player"
        {
            snake.GrowBody();
            SpawnFood();
        }
    }

    bool IsPositionOccupied(Vector2 position)
    {
        if ((Vector2)snake.transform.position == position)
        {
            return true;
        }
        foreach (Transform segment in snake.body)
        {
            if ((Vector2)segment.position == position)
            {
                return true;
            }
        }
        return false;
    }

    void SpawnFood()
    {
        float width = snake.GetWidth();  // certifique que GetWidth() é public na Snake
        float height = snake.GetHeight(); // certifique que GetHeight() é public na Snake

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

        currentFood = Instantiate(foodPrefab, randomPosition, Quaternion.identity).transform;
    }
}
