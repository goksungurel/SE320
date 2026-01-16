using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class MapManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject unlockPanel;
    public GameObject notEnoughMoneyPanel;
    public TMP_Text costText;
    public TMP_Text userMoneyText;
    public TMP_Text warningMessage;

    [Header("Global Stats")]
    public TMP_Text globalMoneyText;

    [Header("Country UI Icons")]
    [Tooltip("Fransa konlar")]
    public GameObject franceLock;
    public GameObject franceOpen;
    [Tooltip("spanya konlar")]
    public GameObject spainLock;
    public GameObject spainOpen;
    [Tooltip("talya konlar")]
    public GameObject italyLock;
    public GameObject italyOpen;

    private int currentCost;
    private string targetScene;
    private string fallbackScene;
    public GameObject resetConfirmationPanel;

    void Start()
    {
        UpdateMoneyDisplay();
        RefreshMapVisuals(); // Balangta kim kilitli kim ak kontrol et
    }
    void HandleLogic(string countryName, int cost, string nextScene, string prevScene)
    {
        Debug.Log("Butona tıklandı! Hedef: " + nextScene + " Önceki: " + prevScene);
        targetScene = nextScene;
        currentCost = cost;
        fallbackScene = prevScene;

        // ülke kontrolü
        string prevCountryName = prevScene.Replace("Card", "");
        bool isPreviousUnlocked = (prevCountryName == "Germany") || (PlayerPrefs.GetInt(prevCountryName + "Unlocked", 0) == 1);

        if (!isPreviousUnlocked)
        {
            warningMessage.text = "First, you need to unlock " + prevCountryName + "!";
            notEnoughMoneyPanel.SetActive(true);
            return; 
        }

        //seviye ilerleme kontrolü
        if (PlayerPrefs.GetInt(countryName + "Unlocked", 0) == 1)
        {
            CheckProgressionAndLoad(countryName, nextScene);
        }
        else
        {
            OpenUnlockPanel();
        }
    }
    // --- GRSEL GNCELLEME EKRDE ---
    public void RefreshMapVisuals()
    {
        // Fransa: 0 ise kilitli, 1 ise ak
        bool isFranceUnlocked = PlayerPrefs.GetInt("FranceUnlocked", 0) == 1;
        if (franceLock) franceLock.SetActive(!isFranceUnlocked);
        if (franceOpen) franceOpen.SetActive(isFranceUnlocked);

        // spanya
        bool isSpainUnlocked = PlayerPrefs.GetInt("SpainUnlocked", 0) == 1;
        if (spainLock) spainLock.SetActive(!isSpainUnlocked);
        if (spainOpen) spainOpen.SetActive(isSpainUnlocked);

        // talya
        bool isItalyUnlocked = PlayerPrefs.GetInt("ItalyUnlocked", 0) == 1;
        if (italyLock) italyLock.SetActive(!isItalyUnlocked);
        if (italyOpen) italyOpen.SetActive(isItalyUnlocked);
    }

    // --- BUTON TIKLAMA FONKSYONLARI ---

    // Almanya için de senin mantığını aktif ettim
    public void ClickGermany() { CheckProgressionAndLoad("Germany", "CardGermany"); }

    public void ClickFrance() { HandleLogic("France", 15, "CardFrance", "CardGermany"); }

    public void ClickSpain() { HandleLogic("Spain", 30, "CardSpain", "CardFrance"); }

    public void ClickItaly() { HandleLogic("Italy", 45, "CardItaly", "CardSpain"); }


    void OpenUnlockPanel()
    {
        costText.text = "Unlock Cost: " + currentCost;
        userMoneyText.text = "Your Money: " + PlayerPrefs.GetInt("totalCoins", 0);
        unlockPanel.SetActive(true);
    }

    public void TryUnlock()
    {
        int myMoney = PlayerPrefs.GetInt("totalCoins", 0);
        if (myMoney >= currentCost)
        {

            string countryName = targetScene.Replace("Card", "");
            string key = countryName + "Unlocked";
            
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.SetInt("totalCoins", myMoney - currentCost);
            PlayerPrefs.Save();

            unlockPanel.SetActive(false);
            UpdateMoneyDisplay();
            RefreshMapVisuals(); 

            CheckProgressionAndLoad(countryName, targetScene);
        }
        else
        {
            unlockPanel.SetActive(false);
            // fallbackScene isminden "Card"ı temizleyerek kullanıcıya daha temiz bir mesaj veriyoruz
            string cleanFallback = fallbackScene.Replace("Card", "");
            warningMessage.text = "Not enough coins! Go back to " + cleanFallback + " to earn more.";
            notEnoughMoneyPanel.SetActive(true);
        }
    }

    void CheckProgressionAndLoad(string country, string defaultScene)
    {
        // catcher bittiyse runnerdan başla
        if (PlayerPrefs.GetInt(country + "_Catcher_Done", 0) == 1)
        {
            SceneManager.LoadScene(country + "Runner");
        }
        // card done ise catcherdan başla
        else if (PlayerPrefs.GetInt(country + "_Card_Done", 0) == 1)
        {
            SceneManager.LoadScene(country + "Catch" );
        }
        // hiçbir şey bitmediyse default aç
        else
        {
            SceneManager.LoadScene(defaultScene);
        }
    }

    public void PlayPreviousCountry()
    {
        Time.timeScale = 1f;

        string countryName = fallbackScene.Replace("Card", "");

        // deleting progresses of levels
        PlayerPrefs.DeleteKey(countryName + "_Card_Done");
        PlayerPrefs.DeleteKey(countryName + "_Catcher_Done");
        PlayerPrefs.Save();

        Debug.Log(countryName + " deleting progress.");

        SceneManager.LoadScene(fallbackScene);
    }

    public void UpdateMoneyDisplay()
    {
        if (globalMoneyText) globalMoneyText.text = PlayerPrefs.GetInt("totalCoins", 0).ToString();
    }
    // OYUNU TAMAMEN SIFIRLAMA FONKSYONU
    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteKey("totalCoins");
        PlayerPrefs.DeleteKey("FranceUnlocked");
        PlayerPrefs.DeleteKey("SpainUnlocked");
        PlayerPrefs.DeleteKey("ItalyUnlocked");

        //deleting keys for control
        PlayerPrefs.DeleteKey("Germany_Card_Done");
        PlayerPrefs.DeleteKey("Germany_Catcher_Done");

        PlayerPrefs.DeleteKey("France_Card_Done");
        PlayerPrefs.DeleteKey("France_Catcher_Done");

        PlayerPrefs.DeleteKey("Spain_Card_Done");
        PlayerPrefs.DeleteKey("Spain_Catcher_Done");

        PlayerPrefs.DeleteKey("Italy_Card_Done");
        PlayerPrefs.DeleteKey("Italy_Catcher_Done");


        PlayerPrefs.Save();

        UpdateMoneyDisplay();
        RefreshMapVisuals();

        // lem bitince paneli kapat
        if (resetConfirmationPanel) resetConfirmationPanel.SetActive(false);

        Debug.Log("Sfrland! Tüm ilerleme ve kilitler temizlendi.");
    }
}