using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager3 : MonoBehaviour
{
    public static GameManager3 Instance;

    private static float savedTime = 60f;
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

    [Header("Pause/Start Settings")]
    public GameObject pausePanel;
    public GameObject startPanel;
    private bool isPaused = false;

    [Header("Sound Settings")]
    public AudioSource audioSource;
    public AudioClip coinSound;
    public AudioClip damageSound;
    public AudioClip jumpSound;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }

        
        timeRemaining = savedTime;

        if (shouldShowStartPanel) { Time.timeScale = 0f; }
        else { Time.timeScale = 1f; }
    }

    void Start()
    {
        currentLives = maxLives;
        UpdateHeartsUI();
        UpdateCoinUI();

        if (timeUpPanel != null) timeUpPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (startPanel != null) startPanel.SetActive(shouldShowStartPanel);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { TogglePause(); }

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

    public void PlayCoinSound()
    {
        if (audioSource != null && coinSound != null)
            audioSource.PlayOneShot(coinSound); 
    }

    public void PlayDamageSound()
    {
        if (audioSource != null && damageSound != null)
            audioSource.PlayOneShot(damageSound); 
    }

    public void PlayJumpSound()
    {
        if (audioSource != null && jumpSound != null)
            audioSource.PlayOneShot(jumpSound);
    }

    public void TakeDamage(int amount)
    {
        currentLives -= amount;
        PlayDamageSound(); 

        if (currentLives < 0) currentLives = 0;
        UpdateHeartsUI();

        if (currentLives <= 0) { RestartLevel(); }
    }

    public void AddCoin(int amount)
    {
        totalCoins += amount;
        UpdateCoinUI();
        PlayCoinSound(); 
    }

    void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null) hearts[i].enabled = i < currentLives;
        }
    }

    void UpdateCoinUI() { if (coinText != null) coinText.text = "        " + totalCoins.ToString(); }

    void UpdateTimerUI() { if (timerText != null) timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining).ToString(); }

    void LevelFinished()
    {
        Time.timeScale = 0f;
        if (finalCoinText != null) finalCoinText.text = "Total Collected Coin: " + totalCoins.ToString();
        if (timeUpPanel != null) timeUpPanel.SetActive(true);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        if (pausePanel != null) pausePanel.SetActive(isPaused);
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
        isTimerRunning = false; 
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
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}