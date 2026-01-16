using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections; // IEnumerator için şart

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
    public TextMeshProUGUI globalMoneyText;

    [Header("Text Elements")]
    public TextMeshProUGUI costText;
    public TextMeshProUGUI userMoneyText;
    public TextMeshProUGUI warningMessage;

    [Header("End Game")]
    public GameObject comingSoonPanel;

    private string targetScene;
    private int currentCost;
    private string fallbackScene;

    void Start()
    {
        PlayerPrefs.SetInt("GermanyRunnerDone", 1);
        UpdateGlobalMoneyDisplay();
    }

    public void UpdateGlobalMoneyDisplay()
    {
        if (globalMoneyText != null)
        {
            int totalCoins = PlayerPrefs.GetInt("totalCoins", 0);
            globalMoneyText.text = totalCoins.ToString();
        }
    }

    private IEnumerator LoadSceneWithSound(string sceneName)
    {
        PlaySound();
        yield return new WaitForSecondsRealtime(0.15f); 
        SceneManager.LoadScene(sceneName);
    }

    public void ClickGermany() { HandleLogic("Germany", 0, "CardGermany", "None"); }
    public void ClickFrance()  { HandleLogic("France", 15, "CardFrance", "CardGermany"); }
    public void ClickSpain()   { HandleLogic("Spain", 25, "CardSpain", "CardFrance"); }
    public void ClickItaly()   { HandleLogic("Italy", 40, "CardItaly", "CardSpain"); }

    void HandleLogic(string countryName, int cost, string nextScene, string prevScene)
    {
        targetScene = nextScene;
        currentCost = cost;
        fallbackScene = prevScene;

        string prevCountryName = prevScene.Replace("Card", "");
        
        bool isPreviousFinished = (prevCountryName == "Germany" && PlayerPrefs.GetInt("GermanyRunnerDone", 0) == 1) 
                               || (PlayerPrefs.GetInt(prevCountryName + "RunnerDone", 0) == 1)
                               || (prevCountryName == "None");

        if (!isPreviousFinished && countryName != "Germany") 
        {
            if(warningMessage != null) warningMessage.text = "First, you need to finish all levels of " + prevCountryName + "!";
            if(notEnoughMoneyPanel != null) notEnoughMoneyPanel.SetActive(true);
            if (playPreviousButton != null) playPreviousButton.SetActive(true);
            return;
        }

        if (PlayerPrefs.GetInt(countryName + "Unlocked", 0) == 1 || countryName == "Germany")
        {
            string sceneToLoad = GetLatestSceneInCountry(countryName);
            StartCoroutine(LoadSceneWithSound(sceneToLoad));
        }
        else
        {
            PlaySound();
            OpenUnlockPanel();
        }
    }

    private string GetLatestSceneInCountry(string country)
    {
        if (PlayerPrefs.GetInt(country + "RunnerDone", 0) == 1) { return "Runner" + country; }
        if (PlayerPrefs.GetInt(country + "CatchDone", 0) == 1)  { return country + "Runner"; }
        if (PlayerPrefs.GetInt(country + "CardDone", 0) == 1)   { return country + "Catch"; }
        return "Card" + country;
    }

    public void TryUnlock()
{
    int myMoney = PlayerPrefs.GetInt("totalCoins", 0);

    if (myMoney >= currentCost)
    {
        // Ülke kilidini aç
        PlayerPrefs.SetInt(targetScene.Replace("Card", "") + "Unlocked", 1);
        PlayerPrefs.SetInt("totalCoins", myMoney - currentCost);
        PlayerPrefs.Save();

        // Paneli kapat ve sahneyi yükle
        if (unlockPanel != null) unlockPanel.SetActive(false);
        SceneManager.LoadScene(targetScene);
    }
    else
    {
        // Para yetersizse paneli kapat ve uyarıyı göster
        if (unlockPanel != null) unlockPanel.SetActive(false);
        
        if (warningMessage != null)
        {
            warningMessage.text = "Not enough coins! Play " + fallbackScene.Replace("Card", "") + " to earn more.";
        }

        if (playPreviousButton != null) 
        {
            playPreviousButton.SetActive(true);
        }

        if (notEnoughMoneyPanel != null) 
        {
            notEnoughMoneyPanel.SetActive(true);
        }
    }
}

    void OpenUnlockPanel()
    {
        if(costText != null) costText.text = "Unlock Cost: " + currentCost;
        if(userMoneyText != null) userMoneyText.text = "Your Money: " + PlayerPrefs.GetInt("totalCoins", 0);
        if(unlockPanel != null) unlockPanel.SetActive(true);
    }

    public void PlaySound()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }

    public void PlayPreviousCountry()
    {
        string countryName = fallbackScene.Replace("Card", "");
        PlayerPrefs.DeleteKey(countryName + "CardDone");
        PlayerPrefs.DeleteKey(countryName + "CatchDone");
        PlayerPrefs.Save();
        SceneManager.LoadScene(fallbackScene);
    }
}