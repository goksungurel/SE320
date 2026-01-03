using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager3 : MonoBehaviour
{
    public static GameManager3 Instance;

    private static float savedTime = 60f;
    private static bool isFirstStart = true;
    private static bool shouldShowStartPanel = true;

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

    [Header("Pause Settings")]
    public GameObject pausePanel;
    private bool isPaused = false;

    [Header("Start Settings")]
    public GameObject startPanel;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }

        timeRemaining = savedTime;

        if (shouldShowStartPanel)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    void Start()
    {
        currentLives = maxLives;
        UpdateHeartsUI();
        UpdateCoinUI();

        if (timeUpPanel != null) timeUpPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        if (startPanel != null)
        {
            startPanel.SetActive(shouldShowStartPanel);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (isTimerRunning && Time.timeScale > 0)
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
                savedTime = 60f;
                isTimerRunning = false;
                LevelFinished();
            }
        }
    }

    public void StartGame()
    {
        shouldShowStartPanel = false;
        Time.timeScale = 1f;
        if (startPanel != null) startPanel.SetActive(false);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pausePanel != null) pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            if (pausePanel != null) pausePanel.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        currentLives -= amount;
        if (currentLives < 0) currentLives = 0;
        UpdateHeartsUI();

        if (currentLives <= 0) { RestartLevel(); }
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
        if (coinText != null) { coinText.text = "        " + totalCoins.ToString(); }
    }

    void UpdateTimerUI()
    {
        if (timerText != null) { timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining).ToString(); }
    }

    void LevelFinished()
    {
        Time.timeScale = 0f;
        if (finalCoinText != null) { finalCoinText.text = "Total Collected Coin: " + totalCoins.ToString(); }
        if (timeUpPanel != null) { timeUpPanel.SetActive(true); }
    }

    public void MapMenu()
    {
        shouldShowStartPanel = true;
        savedTime = 60f;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MapMenu");
    }

    public void RestartLevel()
    {
        shouldShowStartPanel = false;
        savedTime = timeRemaining; 
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResetGameFully()
    {
        isTimerRunning = false;     
        shouldShowStartPanel = true;
        savedTime = 60f;            
        timeRemaining = 60f;        
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}