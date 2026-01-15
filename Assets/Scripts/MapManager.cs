using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class MapManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource; 
    public AudioClip clickSound;    

    [Header("Global Money Settings")]
    public TMP_Text moneyText;

    void Start()
    {
        UpdateMoneyDisplay();
    }

    public void UpdateMoneyDisplay()
    {
        
        int currentMoney = PlayerPrefs.GetInt("totalCoins", 0);
        
        if (moneyText != null)
        {
            moneyText.text = currentMoney.ToString();
        }
    }

    public void StartFranceQuest()
    {
        StartCoroutine(LoadSceneRoutine("CardFrance"));
    }


    public void BackToMenu()
    {

        StartCoroutine(LoadSceneRoutine("MainMenu")); 
    }


    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
        yield return new WaitForSecondsRealtime(0.25f);


        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneName);
    }
}