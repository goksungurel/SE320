using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager3 : MonoBehaviour
{
    public static GameManager3 Instance;

    private static float savedTime = 30f;
    private static bool isFirstStart = true;

    [Header("Life Settings")]
    public int maxLives = 3;
    public int currentLives;
    public Image[] hearts;

    [Header("Coin Settings")]
    public TextMeshProUGUI coinText;
    public GameObject coinPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 1.5f;
    public float obstacleCheckRadius = 0.5f; 
    private int totalCoins = 0;

    [Header("Timer Settings")]
    public TextMeshProUGUI timerText;
    public GameObject timeUpPanel;
    public TextMeshProUGUI finalCoinText;
    public float timeRemaining;
    private bool isTimerRunning = true;

    void Awake()
    {
        Time.timeScale = 1f;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Zaman yönetimi
        if (isFirstStart)
        {
            timeRemaining = 30f;
            isFirstStart = false;
        }
        else
        {
            timeRemaining = savedTime;
        }
    }

    void Start()
    {
        currentLives = maxLives;
        UpdateHeartsUI();
        UpdateCoinUI();

        if (timeUpPanel != null)
            timeUpPanel.SetActive(false);

        
        InvokeRepeating("SpawnCoin", 1f, spawnInterval);
    }

    void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                savedTime = timeRemaining;
                UpdateTimerUI();
            }
            else
            {
                timeRemaining = 0;
                savedTime = 30f;
                isTimerRunning = false;
                CancelInvoke("SpawnCoin");
                LevelFinished();
            }
        }
    }

    void SpawnCoin()
    {
        if (isTimerRunning && spawnPoints.Length > 0 && coinPrefab != null)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Vector3 spawnPos = spawnPoints[randomIndex].position;

            
            Collider2D hit = Physics2D.OverlapCircle(spawnPos, obstacleCheckRadius, LayerMask.GetMask("Obstacle"));

            if (hit == null) 
            {
                Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        currentLives -= amount;
        if (currentLives < 0) currentLives = 0;
        UpdateHeartsUI();

        if (currentLives <= 0)
        {
            RestartLevel();
        }
    }

    void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
                hearts[i].enabled = i < currentLives;
        }
    }

    public void AddCoin(int amount)
    {
        totalCoins += amount;
        UpdateCoinUI();
    }

    void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + totalCoins.ToString();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining).ToString();
        }
    }

    void LevelFinished()
    {
        Time.timeScale = 0f;

        if (finalCoinText != null)
        {
            finalCoinText.text = "Total Collected Coin: " + totalCoins.ToString();
        }

        if (timeUpPanel != null)
        {
            timeUpPanel.SetActive(true);
        }
    }

   
    public void BackToMap()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("WorldMap"); 
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResetGameFully()
    {
        isFirstStart = true;
        savedTime = 30f;
        RestartLevel();
    }
}