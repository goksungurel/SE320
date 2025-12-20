using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameManagerC : MonoBehaviour
{
    [Header("Life Settings")]
    public int currentLives = 3; 
    public int maxLives = 3; 

    void Start()
    {
        // Set current lives to maximum at start
        currentLives = maxLives;
    }

    public void LoseLife()
    {
        currentLives--;
        Debug.Log("Loss of heart. Current live: " + currentLives);

        // Check if game should restart
        if (currentLives <= 0)
        {
            Debug.Log("Game Over!");
            RestartGame();
        }
    }

    void RestartGame()
    {
        // Reloads the active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}