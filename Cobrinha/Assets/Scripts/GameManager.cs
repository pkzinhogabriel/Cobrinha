using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour // Classe responsavel por gerenciar o estado do jogo, como pontuacao e game over.
{
    public Snake snake; // Referencia a cobra no jogo.
    public TextMeshProUGUI ScoreText; // Texto para exibir a pontuacao atual.
    public TextMeshProUGUI HighScoreText; // Texto para exibir a maior pontuacao.
    public TextMeshProUGUI gameOverText; // Texto exibido quando o jogo termina.

    private int score = 0; // Pontuacao atual do jogador.
    private int highScore = 0; // Maior pontuacao registrada.

    void Start() // Metodo chamado no inicio do jogo. Inicializa o estado inicial do HUD.
    {
        gameOverText.gameObject.SetActive(false); // Esconde o texto de "game over".
        ScoreText.gameObject.SetActive(true); // Exibe o texto de  pontuacao.
        HighScoreText.gameObject.SetActive(true); // Exibe o texto de maior pontuacao.
        UpdateScore(0); // Inicia a pontuacao com 0.
    }

    public void UpdateScore(int points) // Metodo responsavel por atualizar a pontuacao do jogador.
    {
        score += points; // Adiciona pontos ao total.
        ScoreText.text = "SCORE: " + score.ToString(); // Atualiza o texto de pontuacao.

        if (score > highScore) // Se a pontuacao atual for maior que a maior pontuacao, atualiza o high score.
        {
            highScore = score;
            HighScoreText.text = "HIGH SCORE: " + highScore.ToString(); // Atualiza o texto de maior pontuacao.
        }
    }

    public void GameOver() // Metodo chamado quando o jogo termina.
    {
        gameOverText.gameObject.SetActive(true);// Exibe o texto de "game over".
    }

    public void Restart() // Metodo responsavel por reiniciar o jogo.
    {
        score = 0; // Reseta a pontuacao.
        UpdateScore(0); // Atualiza a pontuacao exibida.
        snake.Restart(); // Reinicia a cobra.
        gameOverText.gameObject.SetActive(false); // Esconde o texto de "game over".
    }
}
