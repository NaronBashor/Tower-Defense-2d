using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameManager gameManager; // Reference to GameManager

    public void StartNewGame()
    {
        gameManager.NewGame();
        // Additional code to load the first level or set up the game state
    }
}
