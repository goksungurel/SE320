using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;
using TMPro;

public class GameManagerC : MonoBehaviour
{

    public int currentLives ; 
    public int maxLives = 3; 
    public Image[] hearts;

    public int score = 0; 
    public TextMeshProUGUI scoreText; 

    public float timeRemaining = 40f;
    public TextMeshProUGUI timerText;
    public GameObject victoryPanel;
    public TextMeshProUGUI finalScoreText;
    private bool isGameActive = true;

    void Start()
    {
        // Set current lives to maximum at start
        currentLives = maxLives;
        isGameActive = true;

        if (victoryPanel != null) victoryPanel.SetActive(false);

        UpdateHeartsUI();
        UpdateScoreUI();
    }

    void Update()
    {
        if (isGameActive)
        {
            HandleTimer();
        }
    } 

    void HandleTimer()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (timerText != null)
                timerText.text = "Time: " + Mathf.Ceil(timeRemaining).ToString();
        }
        else
        {
            timeRemaining = 0;
            WinGame();
        }
    }

    void WinGame()
    {
        isGameActive = false;
        Debug.Log("Victory!");
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true); // Paneli aktif et
            // Son skoru Victory panelindeki yazıya yazdır
            if (finalScoreText != null)
            {
                finalScoreText.text = "Total Coins: " + score.ToString(); 
            }
        }
        Time.timeScale = 0f; // Oyunu durdur
    }
    void GameOver()
    {
        isGameActive = false;
        Debug.Log("Game Over!");
        RestartGame();
    }

    public void AddScore(int amount)
    {
        if (!isGameActive) return;
        score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString(); 
        }
    }

    public void TakeDamage(int amount)
    {
        if (!isGameActive) return;
        currentLives -= amount;
        if (currentLives < 0)
            currentLives = 0;

        UpdateHeartsUI();

        if (currentLives <= 0)

        {
           
            RestartGame();
        }
    }

    void UpdateHeartsUI()
    {
        if (hearts == null) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
            {
                hearts[i].enabled = i < currentLives;
            }
        }
    }


    void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}