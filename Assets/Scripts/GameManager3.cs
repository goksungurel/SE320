using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager3 : MonoBehaviour
{
    public static GameManager3 Instance;

    private static float savedTime = 60f;
    private static bool shouldShowStartPanel = true;

    [Header("Movement Settings")]
    public float currentForwardSpeed = 7f; // Player h�z� buraya bakacak
    public float speedIncreaseRate = 0.05f; // Fransa i�in saniyelik art��

    [Header("Life Settings")]
    public int maxLives = 3;
    public int currentLives;
    public Image[] hearts;

    [Header("Coin Settings")]
    public TextMeshProUGUI coinText;
    private int totalCoins = 0;

    [Header("Global Money System")]
    public TextMeshProUGUI globalMoneyText;
    public TextMeshProUGUI winGlobalMoneyText;

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
    public AudioClip coinSound;     // Coin toplay�nca
    public AudioClip damageSound;   // Engele �arp�nca
    public AudioClip jumpSound;     // Z�play�nca
    public AudioClip bulletHitSound; // Mermi �arp�nca (�talya)

    [Header("Background Music")]
    public AudioClip backgroundMusic;
    public AudioClip buttonClickSound;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }

        // LEVEL BAZLI S�RE AYARI
        if (shouldShowStartPanel)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName.Contains("Germany")) savedTime = 45f;
            else if (sceneName.Contains("France")) savedTime = 60f;
            else if (sceneName.Contains("Spain")) savedTime = 90f;
            else if (sceneName.Contains("Italy")) savedTime = 120f;
            else savedTime = 60f;

            timeRemaining = savedTime;
            Time.timeScale = 0f;
        }
        else
        {
            timeRemaining = savedTime;
            Time.timeScale = 1f;
        }
    }
    public void PlayClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    void Start()
{
    currentLives = maxLives;
    UpdateHeartsUI();
    UpdateCoinUI();
    UpdateGlobalMoneyUI();

    //background music added
    if (audioSource != null && backgroundMusic != null)
    {
        audioSource.Stop();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.ignoreListenerPause = true; 
        audioSource.volume = 0.3f; 
        audioSource.Play();
    }

    if (timeUpPanel != null) timeUpPanel.SetActive(false);
    if (pausePanel != null) pausePanel.SetActive(false);
    if (startPanel != null) startPanel.SetActive(shouldShowStartPanel);
}

    public void UpdateGlobalMoneyUI()
    {
        int currentMoney = PlayerPrefs.GetInt("totalCoins", 0);
        if (globalMoneyText != null) globalMoneyText.text = currentMoney.ToString();
        if (winGlobalMoneyText != null) winGlobalMoneyText.text = "Total Coins: " + currentMoney.ToString();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { TogglePause(); }

        if (isTimerRunning && Time.timeScale > 0)
        {
            // FRANSA HIZLANMA MEKAN���
            if (SceneManager.GetActiveScene().name.Contains("France"))
            {
                currentForwardSpeed += speedIncreaseRate * Time.deltaTime;
            }

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
        UpdateGlobalMoneyUI();
    }

    // SES �ALMA FONKS�YONLARI
    public void PlayCoinSound() { if (audioSource && coinSound) audioSource.PlayOneShot(coinSound); }
    public void PlayDamageSound() { if (audioSource && damageSound) audioSource.PlayOneShot(damageSound); }
    public void PlayJumpSound() { if (audioSource && jumpSound) audioSource.PlayOneShot(jumpSound); }
    public void PlayBulletHitSound() { if (audioSource && bulletHitSound) audioSource.PlayOneShot(bulletHitSound); }

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

    void UpdateHeartsUI() { for (int i = 0; i < hearts.Length; i++) { if (hearts[i] != null) hearts[i].enabled = i < currentLives; } }
    void UpdateCoinUI() { if (coinText != null) coinText.text = "        " + totalCoins.ToString(); }
    
    void UpdateTimerUI() 
    { 
        if (timerText != null) 
        {
            TimeSpan time = TimeSpan.FromSeconds(timeRemaining);
            timerText.text = "TIME: " + time.ToString(@"mm\:ss");

            int currentSeconds = Mathf.CeilToInt(timeRemaining);
            timerText.color = currentSeconds <= 5 ? Color.red : Color.white;
        }
    }

    void LevelFinished()
    {
        int currentTotal = PlayerPrefs.GetInt("totalCoins", 0);
        PlayerPrefs.SetInt("totalCoins", currentTotal + totalCoins);
        PlayerPrefs.Save();
        UpdateGlobalMoneyUI();

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

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void MapMenu()
    {
        shouldShowStartPanel = true;
        savedTime = 60f;
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
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