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
    public float timeRemaining = 60f;
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
    private Dictionary<string, int> collectionTracker = new Dictionary<string, int>();
    private int coinsCollected = 0;

    [Header("Item UI Texts")]
    public TextMeshProUGUI eiffeltext;
    public TextMeshProUGUI louvretext;
    public TextMeshProUGUI baguettetext;
    public TextMeshProUGUI paristext;
    public TextMeshProUGUI francetext;
    public TextMeshProUGUI croissanttext;

    [Header("Item Counts")]
    public int eiffelCount;
    public int louvreCount;
    public int baguetteCount;
    public int parisCount;
    public int franceCount;
    public int croissantCount;

    [Header("Lose Settings")]
    public GameObject losePanel; 
    public TextMeshProUGUI loseReasonMid; 
    public TextMeshProUGUI loseReasonTop;
    public GameObject itemStatsGroup;
    public AudioClip loseSound;
    public GameObject restartButtonTop; 
    public GameObject restartButtonMid; 

    [Header("Start Settings")]
    public GameObject startPanel;

    [Header("Pause Settings")]
    public GameObject pausePanel; 
    private bool isPaused = false;

    [Header("Countdown Settings")]
    public TextMeshProUGUI countdownText;

    [Header("Lose Panel UI")]
    public TextMeshProUGUI loseFranceText;
    public TextMeshProUGUI loseLouvreText;
    public TextMeshProUGUI loseEiffelText;
    public TextMeshProUGUI loseCroissantText;
    public TextMeshProUGUI loseBaguetteText;
    public TextMeshProUGUI loseParisText;
    public TextMeshProUGUI loseCoinText;

    void Start()
{   Time.timeScale = 0f; 
    
    
    if (startPanel != null) startPanel.SetActive(true);

    isGameActive = false;
    
    
    timeRemaining = 60f;
    currentLives = maxLives;

    isGameActive = false; 


    if (victoryPanel != null) victoryPanel.SetActive(false);
    if (losePanel != null) losePanel.SetActive(false);
    

    

    UpdateHeartsUI();
    UpdateScoreUI();
}



public void StartGame()
{
    if (startPanel != null) startPanel.SetActive(false);
    
    // isGameActive burada true olabilir ama zamanı başlatmıyor
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
    // game hala pause da
    if (countdownText != null)
    {
        countdownText.gameObject.SetActive(true);
        
        int count = 3;
        while (count > 0)
        {
            countdownText.text = count.ToString();
            // wait
            yield return new WaitForSecondsRealtime(1f); 
            count--;
        }
        
        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(0.5f);
        countdownText.gameObject.SetActive(false);
    }

    // time goes on
    Time.timeScale = 1f; 
}

public void PauseGame()
{
    Time.timeScale = 0f; // pause
    if (pausePanel != null) pausePanel.SetActive(true); // open panel
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
        
        bool allItemsCollected = (eiffelCount >= 5 && louvreCount >= 5 && parisCount >= 5 && 
                                  baguetteCount >= 5 && croissantCount >= 5 && franceCount >= 5);

        if (score >= 7 && allItemsCollected) 
        {
            WinGame();
        } 
        else 
        {
            GameOver("You did not collect all required items!", true); 
        }
    }
}
    public void CollectItem(string itemTag)
    {
        if (itemTag == "Coin")
        {
            coinsCollected++;
        }
        else if (itemTag != "BadItem") 
        {
            if (!collectionTracker.ContainsKey(itemTag)) 
                collectionTracker[itemTag] = 0;
            
            collectionTracker[itemTag]++;
        }

        CheckWinCondition();
    }

    void CheckWinCondition()
{
    bool isFranceOk = franceCount >= 5;
    bool isLouvreOk = louvreCount >= 5;
    bool isEiffelOk = eiffelCount >= 5;
    bool isCroissantOk = croissantCount >= 5;
    bool isBaguetteOk = baguetteCount >= 5;
    bool isParisOk = parisCount >= 5;

    bool allItemsGoalReached = isFranceOk && isLouvreOk && isEiffelOk && 
                               isCroissantOk && isBaguetteOk && isParisOk;


    if (score >= requiredCoins && allItemsGoalReached)
    {
        WinGame();
    }
}

    void WinGame()
    {
        isGameActive = false;
        Debug.Log("Victory!");

        if (audioSource != null && victorySound != null)
        {
            audioSource.PlayOneShot(victorySound);
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true); 
            if (finalScoreText != null)
            {
                finalScoreText.text = "Total Coins: " + score.ToString(); 
            }

            if (francetext != null) francetext.text = franceCount.ToString();
            if (eiffeltext != null) eiffeltext.text = eiffelCount.ToString();
            if (louvretext != null) louvretext.text = louvreCount.ToString();
            if (paristext != null) paristext.text = parisCount.ToString();
            if (baguettetext != null) baguettetext.text = baguetteCount.ToString();
            if (croissanttext != null) croissanttext.text = croissantCount.ToString();
        }

        Time.timeScale = 0f; 
    }

    void GameOver(string reason, bool showStats)
    {
        isGameActive = false;

        if (audioSource != null && loseSound != null)
        {
            audioSource.PlayOneShot(loseSound);
        }

        if (losePanel != null)
        {
            losePanel.SetActive(true);

            // Önce tüm metinleri ve butonları kapat
            if (loseReasonMid != null) loseReasonMid.gameObject.SetActive(false);
            if (loseReasonTop != null) loseReasonTop.gameObject.SetActive(false);
            if (restartButtonTop != null) restartButtonTop.SetActive(false);
            if (restartButtonMid != null) restartButtonMid.SetActive(false);

            if (showStats) 
            {
                // not enough item
                if (loseReasonTop != null) 
                {
                    loseReasonTop.gameObject.SetActive(true);
                    loseReasonTop.text = reason;
                }
                if (restartButtonTop != null) restartButtonTop.SetActive(true); 
                if (itemStatsGroup != null) itemStatsGroup.SetActive(true);
                UpdateLosePanelStats();
            }
            else 
            {
                // not enough heart
                if (loseReasonMid != null) 
                {
                    loseReasonMid.gameObject.SetActive(true);
                    loseReasonMid.text = reason;
                }
                if (restartButtonMid != null) restartButtonMid.SetActive(true); 
                if (itemStatsGroup != null) itemStatsGroup.SetActive(false);
            }
        }
        Time.timeScale = 0f; 
    }
    void UpdateLosePanelStats()
    {
        if (eiffeltext != null) loseEiffelText.text = eiffelCount.ToString() + "/5";
        if (louvretext != null) loseLouvreText.text = louvreCount.ToString() + "/5";
        if (paristext != null) loseParisText.text = parisCount.ToString() + "/5";
        if (baguettetext != null) loseBaguetteText.text = baguetteCount.ToString() + "/5";
        if (croissanttext != null) loseCroissantText.text = croissantCount.ToString() + "/5";
        if (francetext != null) loseFranceText.text = franceCount.ToString() + "/5";

        if (loseCoinText != null) loseCoinText.text = score + " / 7";
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
        if (currentLives < 0) currentLives = 0;

        UpdateHeartsUI();

        if (currentLives <= 0)
        {
            GameOver("You've run out of lives!",false);
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

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}