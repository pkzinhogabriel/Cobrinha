using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
// Classe responsavel por gerenciar o menu de inicio do jogo.
public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance; // Singleton da classe.

    public Snake snake;// Referencia a cobra no jogo.
    public TMP_InputField widthInput;// Campo de entrada para a largura da area de jogo.
    public TMP_InputField heightInput;// Campo de entrada para a altura da area de jogo.
    public TMP_InputField speedInput;// Campo de entrada para a velocidade da cobra.
    public GameObject startButton; // Botao de inicio do jogo.
    public GameObject panel; // Painel do menu inicial.

    private void Awake() // Metodo chamado ao despertar o objeto. Implementa o padrao Singleton.
    {
        // Implementa o padrão Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Garante que apenas uma instancia de MenuManager exista.
        }
    }
    public void StartGame() // Metodo responsavel por iniciar o jogo ao configurar os parametros e esconder o menu.
    {
        float width, height, speed;

        if (TryGetInputValues(out width, out height, out speed))
        {
            // Configura as dimensões da área de jogo
            snake.SetGameArea(width, height);
            snake.SetSpeed(speed);

            // Desativa o painel, os InputFields e o StartButton
            panel.SetActive(false);
            widthInput.gameObject.SetActive(false);
            heightInput.gameObject.SetActive(false);
            speedInput.gameObject.SetActive(false);
            startButton.SetActive(false);
        }
        else
        {
            Debug.LogError("Por favor, insira valores válidos.");
        }
    }
    private bool TryGetInputValues(out float width, out float height, out float speed) // Metodo que tenta obter e validar os valores de entrada do jogador.
    {
        width = height = speed = 0;

        // Valida cada entrada
        string[] inputs = { widthInput.text, heightInput.text, speedInput.text };
        float[] values = { width, height, speed };

        for (int i = 0; i < inputs.Length; i++)
        {
            if (!float.TryParse(inputs[i], out values[i]))
            {
                return false; // Retorna falso se qualquer entrada for inválida
            }
        }

        // Atribui os valores de volta às variáveis de saída
        width = values[0];
        height = values[1];
        speed = values[2];

        return true; // Retorna verdadeiro se todos os valores forem válidos
    }
}
