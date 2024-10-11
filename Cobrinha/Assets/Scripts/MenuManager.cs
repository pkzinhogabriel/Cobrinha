using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance; 

    public Snake snake; 
    public TMP_InputField widthInput;
    public TMP_InputField heightInput;
    public TMP_InputField speedInput;
    public GameObject startButton; 
    public GameObject panel; 
    private void Awake()
    {
        // Implementa o padrão Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void StartGame()
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
    private bool TryGetInputValues(out float width, out float height, out float speed)
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
