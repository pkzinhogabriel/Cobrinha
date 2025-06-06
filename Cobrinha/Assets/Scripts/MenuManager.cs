using UnityEngine;
using Unity.Netcode;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public Snake snake;
    public GameObject startButton;
    public GameObject panel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartGame()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            Debug.LogWarning("Only the Host can start the game.");
            return;
        }

        float fixedValue = 8f;

        snake.SetGameArea(fixedValue, fixedValue);
        snake.SetSpeed(fixedValue);

        panel.SetActive(false);
        startButton.SetActive(false);
    }
}
