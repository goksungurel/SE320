using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager3 : MonoBehaviour
{
    public static GameManager3 Instance;

    public int maxLives = 3;
    public int currentLives;
    public Image[] hearts;

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
    }

    
    public void TakeDamage(int amount)
    {
        currentLives -= amount;
        if (currentLives < 0)
            currentLives = 0;

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

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
