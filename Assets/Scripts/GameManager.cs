using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // ================= START PANEL =================
    [Header("Start Panel")]
    public GameObject startPanel;
    bool gameStarted = false;

    // ================= MONEY & ECONOMY =================
    [Header("Money & Economy")]
    public int money = 0;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI winGlobalMoneyText; 
    public TextMeshProUGUI loseGlobalMoneyText;
    public TextMeshProUGUI globalMoneyText;
    public int entryFee = 0;

    // ================= LEVEL SETTINGS =================
    [Header("Level Settings")]
    public float levelTime = 30f;

    // ================= TIMER =================
    [Header("Timer")]
    public TextMeshProUGUI timerText;
    float timeLeft;
    bool timerRunning = true;

    // ================= BOARD =================
    [Header("Board")]
    public Transform gridParent;
    public GameObject cardPrefab;
    public Sprite backSprite;
    public List<Sprite> fronts;

    [Range(2, 20)] public int pairs = 6;
    public float hideDelay = 0.6f;

    // ================= WIN UI =================
    [Header("Win UI")]
    public GameObject winPanel;
    public AudioSource winSound;

    // ================= LOSE UI =================
    [Header("Lose UI")]
    public GameObject losePanel;

    // ================= AUDIO =================
    [Header("Background Music")]
    public AudioSource audioSource;
    public AudioClip backgroundMusic;
    public AudioClip buttonClickSound;

    public bool InputLocked { get; private set; }

    Card first, second;
    int foundPairs = 0;
    int totalPairs = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        

        if (winPanel != null)
            winPanel.SetActive(false);

        if (losePanel != null)
            losePanel.SetActive(false);

        if (startPanel != null)
            startPanel.SetActive(true);

        gameStarted = false;
        InputLocked = true;
        timerRunning = false;

        timeLeft = levelTime;

        if (audioSource != null && backgroundMusic != null)
        {
            audioSource.Stop();
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.ignoreListenerPause = true;
            audioSource.volume = 0.3f;
            audioSource.Play();
        }

        UpdateTimerUI();
        UpdateMoneyUI();
        UpdateGlobalMoneyUI();
    }

    public void PlayClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
            audioSource.PlayOneShot(buttonClickSound);
    }

    // ================= START GAME =================
    public void StartGame()
    {
        if (startPanel != null)
            startPanel.SetActive(false);

        BuildBoard();

        gameStarted = true;
        InputLocked = false;
        timerRunning = true;
    }



    void Update()
    {
        if (timerRunning && timeLeft > 0f)
        {
            timeLeft -= Time.unscaledDeltaTime;
            if (timeLeft < 0f) timeLeft = 0f;
        }

        UpdateTimerUI();

        if (timerRunning && timeLeft <= 0f)
        {
            Lose();
        }
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = money.ToString();
    }


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
        if (loseGlobalMoneyText != null)
        {
        loseGlobalMoneyText.text = "Total Coins: " + currentMoney.ToString();
        }
        
    
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyUI();
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            TimeSpan time = TimeSpan.FromSeconds(timeLeft);
            timerText.text = "TIME: " + time.ToString(@"mm\:ss");

            int currentSeconds = Mathf.CeilToInt(timeLeft);
            timerText.color = currentSeconds <= 5 ? Color.red : Color.white;
        }
    }

    // ================= GAME LOGIC =================
    void BuildBoard()
    {
        foundPairs = 0;
        var deck = new List<(Sprite spr, int id)>();
        int usable = Mathf.Min(pairs, fronts.Count);
        totalPairs = usable;

        for (int i = 0; i < usable; i++)
        {
            deck.Add((fronts[i], i));
            deck.Add((fronts[i], i));
        }

        for (int i = deck.Count - 1; i > 0; i--)
        {
            int k = UnityEngine.Random.Range(0, i + 1);
            (deck[i], deck[k]) = (deck[k], deck[i]);
        }

        foreach (var (spr, id) in deck)
        {
            GameObject go = Instantiate(cardPrefab, gridParent);
            Card card = go.GetComponent<Card>();
            card.Init(spr, backSprite, id);
        }
    }

    public void OnCardRevealed(Card c)
    {
        if (InputLocked) return;

        if (first == null)
        {
            first = c;
            return;
        }

        if (second == null && c != first)
        {
            second = c;
            StartCoroutine(CheckMatch());
        }
    }

    System.Collections.IEnumerator CheckMatch()
    {
        InputLocked = true;

        if (first.Id == second.Id)
        {
            first.SetMatched();
            second.SetMatched();
            foundPairs++;
            AddMoney(1);

            if (foundPairs >= totalPairs)
                Win();
        }
        else
        {
            first.FlashWrong();
            second.FlashWrong();
            yield return new WaitForSeconds(hideDelay);
            first.Hide();
            second.Hide();
        }

        first = null;
        second = null;
        InputLocked = false;
    }

    void Win()
    {
        timerRunning = false;

        int currentTotal = PlayerPrefs.GetInt("totalCoins", 0);
        PlayerPrefs.SetInt("totalCoins", currentTotal + money);

        string countryName = SceneManager.GetActiveScene().name.Replace("Card", "");
        PlayerPrefs.SetInt(countryName + "_Card_Done", 1);
        
        PlayerPrefs.Save();

        UpdateGlobalMoneyUI();

        if (winPanel != null)
            winPanel.SetActive(true);

        if (winSound != null)
            winSound.Play();
    }

    void Lose()
    {
        timerRunning = false;
        InputLocked = true;

        if (losePanel != null)
            losePanel.SetActive(true);

        Time.timeScale = 0f;
    }

    // ================= UI BUTTONS =================
    public void RestartLevel()
    {
        StartCoroutine(LoadSceneRoutine(SceneManager.GetActiveScene().buildIndex));
    }

    public void GoToMapMenu()
    {
        StartCoroutine(LoadSceneRoutine(1)); 
    }

    public void GoToNextScene()
    {
        StartCoroutine(LoadSceneRoutine(SceneManager.GetActiveScene().buildIndex + 1));
    }


    private System.Collections.IEnumerator LoadSceneRoutine(int sceneIndex)
    {
        PlayClickSound(); 
        yield return new WaitForSecondsRealtime(0.2f); 
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneIndex); 
    }
}
