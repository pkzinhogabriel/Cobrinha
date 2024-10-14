using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Snake snake;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI HighScoreText;
    public TextMeshProUGUI gameOverText;

    private int score = 0;
    private int highScore = 0;

    void Start()
    {
        gameOverText.gameObject.SetActive(false);
        ScoreText.gameObject.SetActive(true); 
        HighScoreText.gameObject.SetActive(true); 
        UpdateScore(0);
    }

    public void UpdateScore(int points)
    {
        score += points;
        ScoreText.text = "SCORE: " + score.ToString();

        if (score > highScore)
        {
            highScore = score;
            HighScoreText.text = "HIGH SCORE: " + highScore.ToString();
        }
    }

    public void GameOver()
    {
        gameOverText.gameObject.SetActive(true);
    }

    public void Restart()
    {
        score = 0;
        UpdateScore(0);
        snake.Restart();
        gameOverText.gameObject.SetActive(false);
    }
}
