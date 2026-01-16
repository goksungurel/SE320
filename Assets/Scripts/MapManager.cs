using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class MapManager : MonoBehaviour
{   
    [Header("Audio Settings")]
    public AudioSource audioSource; 
    public AudioClip clickSound;

    [Header("UI Panels")]
    public GameObject unlockPanel;
    public GameObject notEnoughMoneyPanel;
    public GameObject playPreviousButton;
    public GameObject resetConfirmationPanel;

    [Header("Text Elements")]
    public TextMeshProUGUI costText;
    public TextMeshProUGUI userMoneyText;
    public TextMeshProUGUI warningMessage;

    [Header("End Game")]
    public GameObject comingSoonPanel;

    private string targetScene;
    private int currentCost;
    private string fallbackScene;

    // --- SESİN KESİLMEMESİ İÇİN YARDIMCI COROUTINE ---
    private IEnumerator LoadSceneWithSound(string sceneName)
    {
        PlaySound();
        yield return new WaitForSecondsRealtime(0.15f); 
        SceneManager.LoadScene(sceneName);
    }

    void HandleLogic(string countryName, int cost, string nextScene, string prevScene)
    {
        targetScene = nextScene;
        currentCost = cost;
        fallbackScene = prevScene;

        string prevCountryName = prevScene.Replace("Card", "");
        bool isPreviousUnlocked = (prevCountryName == "Germany") || (PlayerPrefs.GetInt(prevCountryName + "Unlocked", 0) == 1);

        if (!isPreviousUnlocked)
        {
            warningMessage.text = "First, you need to unlock " + prevCountryName + "!";
            if (playPreviousButton != null) playPreviousButton.SetActive(false);
            notEnoughMoneyPanel.SetActive(true);
            return;
        }

        if (PlayerPrefs.GetInt(countryName + "Unlocked", 0) == 1)
        {
            // Ülke açıksa direkt yükle ama sesle beraber
            StartCoroutine(LoadSceneWithSound(nextScene));
        }
        else
        {
            // Ülke kapalıysa panel aç (panel açılırken ses çalması için PlaySound ekledik)
            PlaySound();
            OpenUnlockPanel();
        }
    }

    public void TryUnlock()
    {
        PlaySound();
        int myMoney = PlayerPrefs.GetInt("totalCoins", 0);

        if (myMoney >= currentCost)
        {
            PlayerPrefs.SetInt(targetScene.Replace("Card", "") + "Unlocked", 1);
            PlayerPrefs.SetInt("totalCoins", myMoney - currentCost);
            PlayerPrefs.Save();
            StartCoroutine(LoadSceneWithSound(targetScene));
        }
        else
        {
            unlockPanel.SetActive(false);
            warningMessage.text = "Not enough coins! Play " + fallbackScene.Replace("Card", "") + " to earn more.";
            if (playPreviousButton != null) playPreviousButton.SetActive(true);
            notEnoughMoneyPanel.SetActive(true);
        }
    }

    void OpenUnlockPanel()
    {
        costText.text = "Unlock Cost: " + currentCost;
        userMoneyText.text = "Your Money: " + PlayerPrefs.GetInt("totalCoins", 0);
        unlockPanel.SetActive(true);
    }

    public void ClickGermany()
    {
        StartCoroutine(LoadSceneWithSound("CardGermany"));
    }

    public void ClickFrance() { HandleLogic("France", 15, "CardFrance", "CardGermany"); }
    public void ClickSpain() { HandleLogic("Spain", 25, "CardSpain", "CardFrance"); }
    public void ClickItaly() { HandleLogic("Italy", 40, "CardItaly", "CardSpain"); }

    public void OpenResetConfirmation()
    {
        PlaySound();
        if (resetConfirmationPanel != null) resetConfirmationPanel.SetActive(true);
    }

    public void ResetAllProgress()
    {
        PlaySound();
        PlayerPrefs.DeleteKey("totalCoins");
        PlayerPrefs.DeleteKey("FranceUnlocked");
        PlayerPrefs.DeleteKey("SpainUnlocked");
        PlayerPrefs.DeleteKey("ItalyUnlocked");
        PlayerPrefs.Save();

        if (resetConfirmationPanel != null) resetConfirmationPanel.SetActive(false);
        // Reset sonrası sahne yenilenirken ses için bekleme
        StartCoroutine(LoadSceneWithSound(SceneManager.GetActiveScene().name));
    }

    void CheckAllUnlocked()
    {
        if (PlayerPrefs.GetInt("ItalyUnlocked", 0) == 1)
        {
            if (comingSoonPanel != null) comingSoonPanel.SetActive(true);
        }
    }

    public void BackToMenu()
    { 
        PlaySound();
        Time.timeScale = 1f;
        StartCoroutine(LoadSceneWithSound("MainMenu")); 
    }

    public void PlaySound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
    public void PlayPreviousCountry()
    {
        Time.timeScale = 1f;

        string countryName = fallbackScene.Replace("Card", "");

        PlayerPrefs.DeleteKey(countryName + "_Card_Done");
        PlayerPrefs.DeleteKey(countryName + "_Catcher_Done");
        PlayerPrefs.Save();

        Debug.Log(countryName + " deleting progress.");

        SceneManager.LoadScene(fallbackScene);
    } 
}