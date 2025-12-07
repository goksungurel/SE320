using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager3 : MonoBehaviour
{
    public int maxLives = 3;      // Inspector'dan 3
    public Image[] hearts;        // Heart1, Heart2, Heart3

    int currentLives;

    void Start()
    {
        currentLives = maxLives;
        UpdateHearts();
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            // kaç can varsa o kadar kalp açýk
            hearts[i].enabled = i < currentLives;
        }
    }

    public void TakeDamage()
    {
        if (currentLives <= 0) return;

        currentLives--;
        UpdateHearts();

        if (currentLives <= 0)
        {
            // can bitince sahneyi baþtan yükle
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
