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
    public GameObject playPreviousButton; // Para yetmeyince çýkan o buton
    public GameObject resetConfirmationPanel;
    public GameObject comingSoonPanel;
    public TextMeshProUGUI globalMoneyText;

    [Header("Country Unlock Objects")]
    public GameObject franceLock;
    public GameObject franceUnlock;
    public GameObject spainLock;
    public GameObject spainUnlock;
    public GameObject italyLock;
    public GameObject italyUnlock;

    [Header("Text Elements")]
    public TextMeshProUGUI costText;
    public TextMeshProUGUI userMoneyText;
    public TextMeshProUGUI warningMessage;

    private string targetScene;
    private int currentCost;
    private string fallbackScene;

    void Start()
    {
        PlayerPrefs.SetInt("GermanyUnlocked", 1);
        UpdateGlobalMoneyDisplay();
        CheckAllUnlocked();
        RefreshVisuals();
    }

    public void UpdateGlobalMoneyDisplay()
    {
        if (globalMoneyText != null)
            globalMoneyText.text = PlayerPrefs.GetInt("totalCoins", 0).ToString();
    }

    void RefreshVisuals()
    {
        bool franceStatus = PlayerPrefs.GetInt("FranceUnlocked", 0) == 1;
        if (franceLock) franceLock.SetActive(!franceStatus);
        if (franceUnlock) franceUnlock.SetActive(franceStatus);

        bool spainStatus = PlayerPrefs.GetInt("SpainUnlocked", 0) == 1;
        if (spainLock) spainLock.SetActive(!spainStatus);
        if (spainUnlock) spainUnlock.SetActive(spainStatus);

        bool italyStatus = PlayerPrefs.GetInt("ItalyUnlocked", 0) == 1;
        if (italyLock) italyLock.SetActive(!italyStatus);
        if (italyUnlock) italyUnlock.SetActive(italyStatus);
    }

    public void ClickGermany() { HandleLogic("Germany", 0, "CardGermany", "None"); }
    public void ClickFrance() { HandleLogic("France", 15, "CardFrance", "CardGermany"); }
    public void ClickSpain() { HandleLogic("Spain", 25, "CardSpain", "CardFrance"); }
    public void ClickItaly() { HandleLogic("Italy", 40, "CardItaly", "CardSpain"); }

    void HandleLogic(string countryName, int cost, string nextScene, string prevScene)
    {
        targetScene = nextScene;
        currentCost = cost;
        fallbackScene = prevScene;

        if (countryName == "Germany")
        {
            StartCoroutine(LoadSceneWithSound("CardGermany"));
            return;
        }

        string prevCountryName = prevScene.Replace("Card", "");
        bool isPreviousUnlocked = (prevCountryName == "Germany") || (PlayerPrefs.GetInt(prevCountryName + "Unlocked", 0) == 1);

        if (!isPreviousUnlocked)
        {
            if (warningMessage != null) warningMessage.text = "First, you need to unlock " + prevCountryName + "!";
            if (playPreviousButton != null) playPreviousButton.SetActive(false); // Kilit açýlmamýþsa bu buton çýkmasýn
            if (notEnoughMoneyPanel != null) notEnoughMoneyPanel.SetActive(true);
            return;
        }

        if (PlayerPrefs.GetInt(countryName + "Unlocked", 0) == 1)
        {
            StartCoroutine(LoadSceneWithSound(GetLatestSceneInCountry(countryName)));
        }
        else
        {
            PlaySound();
            OpenUnlockPanel();
        }
    }

    public void TryUnlock()
    {
        int myMoney = PlayerPrefs.GetInt("totalCoins", 0);
        string currentCountry = targetScene.Replace("Card", "");

        if (myMoney >= currentCost)
        {
            PlayerPrefs.SetInt(currentCountry + "Unlocked", 1);
            PlayerPrefs.SetInt("totalCoins", myMoney - currentCost);
            PlayerPrefs.Save();

            RefreshVisuals();
            UpdateGlobalMoneyDisplay();
            if (unlockPanel != null) unlockPanel.SetActive(false);
            CheckAllUnlocked();
            StartCoroutine(LoadSceneWithSound(targetScene));
        }
        else
        {
            // PARA YETMEDÝÐÝNDE BURASI ÇALIÞIR
            if (unlockPanel != null) unlockPanel.SetActive(false);
            if (warningMessage != null) warningMessage.text = "Not enough coins! Play " + fallbackScene.Replace("Card", "") + " again.";

            if (playPreviousButton != null) playPreviousButton.SetActive(true); // BUTONU GÖSTER
            if (notEnoughMoneyPanel != null) notEnoughMoneyPanel.SetActive(true);
        }
    }

    // ÖNCEKÝ ÜLKEYÝ BAÞTAN OYNATAN O MEÞHUR FONKSÝYON
    public void PlayPreviousCountry()
    {
        if (!string.IsNullOrEmpty(fallbackScene) && fallbackScene != "None")
        {
            string countryName = fallbackScene.Replace("Card", "");

            // Önceki ülkenin ilerlemesini sýfýrlýyoruz ki en baþtan (Card sahnesinden) baþlasýn
            PlayerPrefs.DeleteKey(countryName + "CardDone");
            PlayerPrefs.DeleteKey(countryName + "CatchDone");
            PlayerPrefs.DeleteKey(countryName + "RunnerDone");
            PlayerPrefs.Save();

            StartCoroutine(LoadSceneWithSound(fallbackScene));
        }
    }

    private string GetLatestSceneInCountry(string country)
    {
        if (PlayerPrefs.GetInt(country + "RunnerDone", 0) == 1) return "Runner" + country;
        if (PlayerPrefs.GetInt(country + "CatchDone", 0) == 1) return country + "Runner";
        if (PlayerPrefs.GetInt(country + "CardDone", 0) == 1) return country + "Catch";
        return "Card" + country;
    }

    public void PlaySound() { if (audioSource != null && clickSound != null) audioSource.PlayOneShot(clickSound); }
    private IEnumerator LoadSceneWithSound(string sceneName) { PlaySound(); yield return new WaitForSecondsRealtime(0.15f); SceneManager.LoadScene(sceneName); }

    void OpenUnlockPanel()
    {
        costText.text = "Unlock Cost: " + currentCost;
        userMoneyText.text = "Your Money: " + PlayerPrefs.GetInt("totalCoins", 0);
        unlockPanel.SetActive(true);
    }

    void CheckAllUnlocked() { if (PlayerPrefs.GetInt("ItalyUnlocked", 0) == 1 && comingSoonPanel != null) comingSoonPanel.SetActive(true); }

    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("GermanyUnlocked", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}