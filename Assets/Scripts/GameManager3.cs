using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 

public class GameManager3 : MonoBehaviour
{
    public static GameManager3 Instance;

    [Header("Life Settings")]
    public int maxLives = 3;
    public int currentLives;
    public Image[] hearts;

    [Header("Coin Settings")]
    public TextMeshProUGUI coinText; 
    private int totalCoins = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        currentLives = maxLives;
        UpdateHeartsUI();
        UpdateCoinUI();
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
            hearts[i].enabled = i < currentLives;
        }
    }

    public void AddCoin(int amount)
    {
        totalCoins += amount;
        UpdateCoinUI();
        Debug.Log("Altýn Toplandý! Toplam: " + totalCoins);
    }

    void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + totalCoins.ToString();
        }
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}