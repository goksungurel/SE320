using UnityEngine;
using System; // TimeSpan için gerekli
using UnityEngine.SceneManagement; 
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManagerC : MonoBehaviour
{
    //inspectora tanımlamak için
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

    [Header("Countdown Settings")]
    public TextMeshProUGUI countdownText;

    [Header("Flexible Item Counters")]
    public List<int> itemCounts = new List<int>();

    [Header("Main UI Text List")]
    public List<TMPro.TextMeshProUGUI> itemUITexts = new List<TMPro.TextMeshProUGUI>();

    [Header("Lose Panel UI Text List")]
    public List<TMPro.TextMeshProUGUI> losePanelItemTexts = new List<TMPro.TextMeshProUGUI>();
    public TMPro.TextMeshProUGUI loseCoinText; 

    [Header("Money System")]
    // EntryFee kaldırıldı
    public TextMeshProUGUI globalMoneyText; 
    public TextMeshProUGUI winGlobalMoneyText; 
    public TextMeshProUGUI loseGlobalMoneyMid; 
    public TextMeshProUGUI loseGlobalMoneyTop; 
    
    [Header("Background Music")]
    public AudioClip backgroundMusic;
    public AudioClip buttonClickSound;

    //-----START OF LEVEL------
    void Start()
    {   
        UpdateGlobalMoneyUI();
        //bgm çalması
        if (audioSource != null && backgroundMusic != null)
        {
            audioSource.Stop();
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.ignoreListenerPause = true; 
            audioSource.volume = 0.25f;
            audioSource.Play();
        }
        //start paneli açıp item arrayini temizliyoruz.
        Time.timeScale = 0f; 
        if (startPanel != null) startPanel.SetActive(true);
        isGameActive = false;
        
        currentLives = maxLives;

        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        itemCounts.Clear();
        for (int i = 0; i < itemUITexts.Count; i++)
        {
            itemCounts.Add(0);
        }
        //ui updateleri
        UpdateHeartsUI();
        UpdateScoreUI();
        UpdateUI(); 
    }
    //time intervals for all levels
    void Awake()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.Contains("Germany")) levelDuration = 45f;
        else if (sceneName.Contains("France")) levelDuration = 60f;
        else if (sceneName.Contains("Spain")) levelDuration = 90f;
        else if (sceneName.Contains("Italy")) levelDuration = 120f;
        
        timeRemaining = levelDuration; 

    }

    //if game is active, timer starts
    void Update()
    {
        if (isGameActive)
        {
            HandleTimer();
        }
    } 


    //en baştaki geriye sayma kodu
    IEnumerator CountdownRoutine()
    {
        if (countdownText != null)
        {
            string sceneName = SceneManager.GetActiveScene().name;

            // color settings for scenes
            if (sceneName.Contains("Spain") || sceneName.Contains("Italy"))
            {
                countdownText.color = Color.white;
            }
            else if (sceneName.Contains("Germany") || sceneName.Contains("France"))
            {
                countdownText.color = Color.black;
            }

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


    void HandleTimer()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            if (timerText != null)
            {
                TimeSpan time = TimeSpan.FromSeconds(timeRemaining);
                timerText.text = "TIME: " + time.ToString(@"mm\:ss");
            }
        }
        else
        {
            timeRemaining = 0;
            if (timerText != null) timerText.text = "TIME: 00:00";

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

    //------LOGIC------

    //checks item constraints for win
    bool CheckAllItemsCollected()
    {
        if (itemCounts.Count == 0) return false;
        foreach (int count in itemCounts)
        {
            if (count < requiredEachItem) return false;
        }
        return true;
    }

    //counts items from their tags
    public void CollectItem(string itemTag, GameObject hitObject)
    {
        if (itemTag == "Coin")
        {
            coinsCollected++;
            score++; 
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

    //checks contition
    void CheckWinCondition()
    {
        if (score >= requiredCoins && CheckAllItemsCollected())
        {
            WinGame();
        }
    }

    
    //it is about what will code do after conditions provided
    void WinGame()
    {
        isGameActive = false;
        
        // bu bölümde toplanan paraları genel bakiyeye ekle
        int currentTotal = PlayerPrefs.GetInt("totalCoins", 0);
        int newTotal = currentTotal + score; 
        PlayerPrefs.SetInt("totalCoins", newTotal);

        // --- YENİ EKLEME: Seviye İlerleme Kaydı ---
        //sahnenin adını al
        string sceneName = SceneManager.GetActiveScene().name;
        
        // her ülke için key yapıyoruz
        if (sceneName.Contains("France")) PlayerPrefs.SetInt("France_Catcher_Done", 1);
        else if (sceneName.Contains("Spain")) PlayerPrefs.SetInt("Spain_Catcher_Done", 1);
        else if (sceneName.Contains("Italy")) PlayerPrefs.SetInt("Italy_Catcher_Done", 1);
        else if (sceneName.Contains("Germany")) PlayerPrefs.SetInt("Germany_Catcher_Done", 1);
        PlayerPrefs.Save();

        Debug.Log("KAYIT YAPILDI: " + sceneName + " için _Catcher_Done anahtarı 1 yapıldı.");
        
        UpdateGlobalMoneyUI(); 

        Debug.Log("Collected Coins " + newTotal); //konsola seviyede toplanan coinleri yazdırıyoruz

        if (audioSource != null && victorySound != null)
            audioSource.PlayOneShot(victorySound);

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true); 
            if (finalScoreText != null)
                finalScoreText.text = "Collected Coins: " + score.ToString(); 
        }
        
        Time.timeScale = 0f; 
    }

    //these are for losing game
    void GameOver(string reason, bool showStats)
    {
        isGameActive = false;
        UpdateGlobalMoneyUI();
        if (audioSource != null && loseSound != null)
            audioSource.PlayOneShot(loseSound);

        if (losePanel != null)
        {
            if (loseGlobalMoneyMid != null) loseGlobalMoneyMid.gameObject.SetActive(!showStats); 
        
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



    //adds coins
    public void AddScore(int amount)
    {
        if (!isGameActive) return;
        score += amount;
        UpdateScoreUI();
    }

    //heartlose
    public void TakeDamage(int amount)
    {
        if (!isGameActive) return;
        currentLives -= amount;
        if (currentLives < 0) currentLives = 0;
        UpdateHeartsUI();

        if (currentLives <= 0)
            GameOver("You've run out of lives!", false);
    }

    //------UPDATEUI------

    //updates global money for all situations
    public void UpdateGlobalMoneyUI()
    {
        int currentMoney = PlayerPrefs.GetInt("totalCoins", 0);
        
        if (globalMoneyText != null)
        {
            globalMoneyText.text = "Total Coins: " + currentMoney.ToString();
        }

        if (winGlobalMoneyText != null)
        {
            winGlobalMoneyText.text = "Total Coins: " + currentMoney.ToString();
        }
        if (loseGlobalMoneyMid != null)
        {
        loseGlobalMoneyMid.text = "Total Coins: " + currentMoney.ToString();
        }
        if (loseGlobalMoneyTop != null)
        {
        loseGlobalMoneyTop.text = "Total Coins: " + currentMoney.ToString();
        }
    }
    public void UpdateUI()
    {
        for (int i = 0; i < itemUITexts.Count; i++)
        {
            if (i < itemCounts.Count)
                itemUITexts[i].text = itemCounts[i].ToString();
        }
    }
    //updates lose panel with collected item stats
    void UpdateLosePanelStats()
    {
        for (int i = 0; i < losePanelItemTexts.Count; i++)
        {
            if (i < itemCounts.Count)
                losePanelItemTexts[i].text = itemCounts[i].ToString() + " / " + requiredEachItem;
        }

        if (loseCoinText != null) loseCoinText.text = score + " / " + requiredCoins;
    }
    //updates uı for score(coin)
    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = score.ToString(); 
    }
    //heart update 
    void UpdateHeartsUI()
    {
        if (hearts == null) return;
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null) hearts[i].enabled = i < currentLives;
        }
    }

    //-------BUTTON FUNCTIONS-------

     //click sounds for buttons
    public void PlayClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    //start game button
    public void StartGame()
    {
        if (startPanel != null) startPanel.SetActive(false);
        isGameActive = true; 
        StartCoroutine(CountdownRoutine()); 
        
        UpdateGlobalMoneyUI();
    }
    //restart button
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    //quit button (to map menu)
    public void QuitGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
    //next level button
    public void GoToNextScene()
    {
        Time.timeScale = 1f; 
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    //pause button
    public void PauseGame()
    {
        Time.timeScale = 0f; 
        if (pausePanel != null) pausePanel.SetActive(true); 
    }
    //resume button
    public void ResumeGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        StartCoroutine(CountdownRoutine()); 
    }

}
