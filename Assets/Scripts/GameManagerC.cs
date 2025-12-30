using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManagerC : MonoBehaviour
{
    [Header("Life Settings")]
    public int currentLives; 
    public int maxLives = 3; 
    public Image[] hearts;

    [Header("Score & Timer")]
    public int score = 0; 
    public TextMeshProUGUI scoreText; 
    public float levelDuration = 60f;
    public float timeRemaining;
    public TextMeshProUGUI timerText;

    [Header("Victory Settings")]
    public GameObject victoryPanel;
    public TextMeshProUGUI finalScoreText;
    public AudioSource audioSource; 
    public AudioClip victorySound;
    private bool isGameActive = true;

    [Header("Win Conditions")]
    public int requiredCoins = 7;
    public int requiredEachItem = 5;
    // Dictionary'yi hala tag takibi için kullanabilirsin ama asıl kontrolü listelerle yapacağız
    private Dictionary<string, int> collectionTracker = new Dictionary<string, int>();
    private int coinsCollected = 0;

    [Header("Lose Settings")]
    public GameObject losePanel; 
    public TextMeshProUGUI loseReasonMid; 
    public TextMeshProUGUI loseReasonTop;
    public GameObject itemStatsGroup;
    public AudioClip loseSound;
    public GameObject restartButtonTop; 
    public GameObject restartButtonMid; 
    public GameObject quitButtonTop; 
    public GameObject quitButtonMid; 

    [Header("Start Settings")]
    public GameObject startPanel;

    [Header("Pause Settings")]
    public GameObject pausePanel; 
    private bool isPaused = false;

    [Header("Countdown Settings")]
    public TextMeshProUGUI countdownText;

    [Header("Flexible Item Counters")]
    public List<int> itemCounts = new List<int>();

    [Header("Main UI Text List")]
    public List<TMPro.TextMeshProUGUI> itemUITexts = new List<TMPro.TextMeshProUGUI>();

    [Header("Lose Panel UI Text List")]
    public List<TMPro.TextMeshProUGUI> losePanelItemTexts = new List<TMPro.TextMeshProUGUI>();

    public TMPro.TextMeshProUGUI loseCoinText; 

    void Start()
    {   
        Time.timeScale = 0f; 
        if (startPanel != null) startPanel.SetActive(true);
        isGameActive = false;
        
        timeRemaining = levelDuration;
        currentLives = maxLives;

        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        // LİSTE HAZIRLIĞI: itemCounts listesini UI sayısı kadar 0 ile doldur
        itemCounts.Clear();
        for (int i = 0; i < itemUITexts.Count; i++)
        {
            itemCounts.Add(0);
        }

        UpdateHeartsUI();
        UpdateScoreUI();
        UpdateUI(); // Başlangıçta 0/5 yazması için
    }

    public void StartGame()
    {
        if (startPanel != null) startPanel.SetActive(false);
        isGameActive = true; 
        StartCoroutine(CountdownRoutine()); 
    }

    public void ResumeGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        StartCoroutine(CountdownRoutine()); 
    }

    IEnumerator CountdownRoutine()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            int count = 3;
            while (count > 0)
            {
                countdownText.text = count.ToString();
                yield return new WaitForSecondsRealtime(1f); 
                count--;
            }
            countdownText.text = "GO!";
            yield return new WaitForSecondsRealtime(0.5f);
            countdownText.gameObject.SetActive(false);
        }
        Time.timeScale = 1f; 
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; 
        if (pausePanel != null) pausePanel.SetActive(true); 
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
            // ESNEK KONTROL: Listeye göre kazanma kontrolü
            if (score >= requiredCoins && CheckAllItemsCollected()) 
            {
                WinGame();
            }
            else 
            {
                GameOver("You did not collect all required items!", true); 
            }
        }
    }

    // YENİ YARDIMCI FONKSİYON: Tüm eşyalar toplandı mı?
    bool CheckAllItemsCollected()
    {
        if (itemCounts.Count == 0) return false;
        foreach (int count in itemCounts)
        {
            if (count < requiredEachItem) return false;
        }
        return true;
    }

    public void CollectItem(string itemTag, GameObject hitObject)
    {
        if (itemTag == "Coin")
        {
            coinsCollected++;
            score++; // Score'u da artırıyoruz
            UpdateScoreUI();
        }
        else if (itemTag == "GoodItem") 
        {
            ItemHolder holder = hitObject.GetComponent<ItemHolder>();
            if (holder != null && holder.data != null)
            {
                int index = holder.data.itemIndex;
                if (index >= 0 && index < itemCounts.Count)
                {
                    itemCounts[index]++;
                    UpdateUI();
                    if (holder.data.collectSound != null)
                        audioSource.PlayOneShot(holder.data.collectSound);
                }
            }
        }
        CheckWinCondition();
    }

    void CheckWinCondition()
    {
        if (score >= requiredCoins && CheckAllItemsCollected())
        {
            WinGame();
        }
    }

    public void UpdateUI()
    {
        for (int i = 0; i < itemUITexts.Count; i++)
        {
            if (i < itemCounts.Count)
                itemUITexts[i].text = itemCounts[i].ToString() + " / " + requiredEachItem.ToString();
        }
    }

    void WinGame()
    {
        isGameActive = false;
        if (audioSource != null && victorySound != null)
            audioSource.PlayOneShot(victorySound);

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true); 
            if (finalScoreText != null)
                finalScoreText.text = "Total Coins: " + score.ToString(); 
        }
        Time.timeScale = 0f; 
    }

    void GameOver(string reason, bool showStats)
    {
        isGameActive = false;
        if (audioSource != null && loseSound != null)
            audioSource.PlayOneShot(loseSound);

        if (losePanel != null)
        {
            losePanel.SetActive(true);
            if (loseReasonMid != null) loseReasonMid.gameObject.SetActive(!showStats);
            if (loseReasonTop != null) loseReasonTop.gameObject.SetActive(showStats);
            
            if (restartButtonTop != null) restartButtonTop.SetActive(showStats);
            if (quitButtonTop != null) quitButtonTop.SetActive(showStats);
            if (restartButtonMid != null) restartButtonMid.SetActive(!showStats);
            if (quitButtonMid != null) quitButtonMid.SetActive(!showStats);

            if (itemStatsGroup != null) itemStatsGroup.SetActive(showStats);
            
            if (showStats)
            {
                UpdateLosePanelStats();
                if (loseReasonTop != null) loseReasonTop.text = reason;
            }
            else if (loseReasonMid != null)
            {
                loseReasonMid.text = reason;
            }
        }
        Time.timeScale = 0f; 
    }

    void UpdateLosePanelStats()
    {
        for (int i = 0; i < losePanelItemTexts.Count; i++)
        {
            if (i < itemCounts.Count)
                losePanelItemTexts[i].text = itemCounts[i].ToString() + " / " + requiredEachItem;
        }

        if (loseCoinText != null) loseCoinText.text = score + " / " + requiredCoins;
    }

    public void AddScore(int amount)
    {
        if (!isGameActive) return;
        score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = score.ToString(); 
    }

    public void TakeDamage(int amount)
    {
        if (!isGameActive) return;
        currentLives -= amount;
        if (currentLives < 0) currentLives = 0;
        UpdateHeartsUI();

        if (currentLives <= 0)
            GameOver("You've run out of lives!", false);
    }

    void UpdateHeartsUI()
    {
        if (hearts == null) return;
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null) hearts[i].enabled = i < currentLives;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}