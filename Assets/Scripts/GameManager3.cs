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
                LevelFinished();
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