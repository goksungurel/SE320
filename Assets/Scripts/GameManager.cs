using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    // ================= MONEY =================
    [Header("Money")]
    public int money = 0;
    public TextMeshProUGUI moneyText;


    // ================= LEVEL SETTINGS =================
    [Header("Level Settings")]
    public float levelTime = 30f;   // Scene 1: 30 | Scene 2: 60 | Scene 3: 90

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
    public TextMeshProUGUI winText;
    public AudioSource winSound;

    public bool InputLocked { get; private set; }

    Card first, second;
    int foundPairs = 0;
    int totalPairs = 0;

    // ================= UNITY LIFECYCLE =================

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

        timeLeft = levelTime;     // ⬅️ SAHNEYE ÖZEL SÜRE
        UpdateTimerUI();

        BuildBoard();

        UpdateMoneyUI();
    }

    void Update()
    {
        if (!timerRunning) return;

        if (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            timeLeft = 0f;
            timerRunning = false;
            TimeIsUp();
        }
    }
    void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = money.ToString() ;
    }

public void AddMoney(int amount)
{
    money += amount;
    UpdateMoneyUI();
}

    // ================= TIMER METHODS =================

    void UpdateTimerUI()
{
    if (timerText == null) return;

    int seconds = Mathf.CeilToInt(timeLeft);

    // Süre bittiyse
    if (seconds <= 0)
    {
        timerText.text = "TIME UP!";
        timerText.color = Color.red;
        return;
    }

    // Son 5 saniye → kırmızı
    if (seconds <= 5)
    {
        timerText.color = Color.red;
    }
    else
    {
        timerText.color = Color.white; // normal renk
    }

    timerText.text = "TIME: 00:" + seconds.ToString("00");
}


    void TimeIsUp()
    {
        Debug.Log("Süre bitti!");
        InputLocked = true;
        timerRunning = false;

        // burada istersen Lose Panel / Restart logic eklenir
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
            int k = Random.Range(0, i + 1);
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
        timerRunning = false;   // ⏸ süre durur

        if (winPanel != null)
        {
            winPanel.SetActive(true);
            winPanel.transform.SetAsLastSibling();

            var img = winPanel.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
            {
                Color c = img.color;
                c.a = 0.75f;
                img.color = c;
            }
        }

        if (winSound != null)
            winSound.Play();
    }
}
