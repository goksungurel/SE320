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
    [Tooltip("Fransa Ýkonlarý")]
    public GameObject franceLock;
    public GameObject franceOpen;
    [Tooltip("Ýspanya Ýkonlarý")]
    public GameObject spainLock;
    public GameObject spainOpen;
    [Tooltip("Ýtalya Ýkonlarý")]
    public GameObject italyLock;
    public GameObject italyOpen;

    private int currentCost;
    private string targetScene;
    private string fallbackScene;
    public GameObject resetConfirmationPanel;

    void Start()
    {
        UpdateMoneyDisplay();
        RefreshMapVisuals(); // Baþlangýçta kim kilitli kim açýk kontrol et
    }

    // --- GÖRSEL GÜNCELLEME ÇEKÝRDEÐÝ ---
    public void RefreshMapVisuals()
    {
        // Fransa: 0 ise kilitli, 1 ise açýk
        bool isFranceUnlocked = PlayerPrefs.GetInt("FranceUnlocked", 0) == 1;
        if (franceLock) franceLock.SetActive(!isFranceUnlocked);
        if (franceOpen) franceOpen.SetActive(isFranceUnlocked);

        // Ýspanya
        bool isSpainUnlocked = PlayerPrefs.GetInt("SpainUnlocked", 0) == 1;
        if (spainLock) spainLock.SetActive(!isSpainUnlocked);
        if (spainOpen) spainOpen.SetActive(isSpainUnlocked);

        // Ýtalya
        bool isItalyUnlocked = PlayerPrefs.GetInt("ItalyUnlocked", 0) == 1;
        if (italyLock) italyLock.SetActive(!isItalyUnlocked);
        if (italyOpen) italyOpen.SetActive(isItalyUnlocked);
    }

    // --- BUTON TIKLAMA FONKSÝYONLARI ---

    public void ClickGermany() { SceneManager.LoadScene("CardGermany"); }

    public void ClickFrance() { HandleLogic("France", 15, "CardFrance", "CardGermany"); }

    public void ClickSpain() { HandleLogic("Spain", 30, "CardSpain", "CardFrance"); }

    public void ClickItaly() { HandleLogic("Italy", 45, "CardItaly", "CardSpain"); }

    void HandleLogic(string countryName, int cost, string nextScene, string prevScene)
    {
        Debug.Log("Butona týklandý! Hedef: " + nextScene + " Önceki: " + prevScene);
        targetScene = nextScene;
        currentCost = cost;
        fallbackScene = prevScene;

        // --- YENÝ: ÖNCEKÝ ÜLKE KONTROLÜ ---
        // Almanya her zaman açýk sayýldýðý için direkt geçiyoruz.
        // Diðerleri için bir önceki ülkenin kilidinin açýk olup olmadýðýna bakýyoruz.
        string prevCountryName = prevScene.Replace("Card", "");
        bool isPreviousUnlocked = (prevCountryName == "Germany") || (PlayerPrefs.GetInt(prevCountryName + "Unlocked", 0) == 1);

        if (!isPreviousUnlocked)
        {
            // Eðer önceki ülke açýlmamýþsa uyarý ver ve paneli açtýrma
            warningMessage.text = "First, you need to unlock " + prevCountryName + "!";
            notEnoughMoneyPanel.SetActive(true);
            return; // Fonksiyonu burada bitir, unlock panelini hiç açma
        }
        // ---------------------------------

        if (PlayerPrefs.GetInt(countryName + "Unlocked", 0) == 1)
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            OpenUnlockPanel();
        }
    }

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
            // Kilidi aç (Sahne isminden "Card" kýsmýný atarak key oluþturur: FranceUnlocked gibi)
            string key = targetScene.Replace("Card", "") + "Unlocked";
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.SetInt("totalCoins", myMoney - currentCost);
            PlayerPrefs.Save();

            unlockPanel.SetActive(false);
            UpdateMoneyDisplay();
            RefreshMapVisuals(); // ANINDA ÝKONU DEÐÝÞTÝR
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            unlockPanel.SetActive(false);
            warningMessage.text = "Not enough coins! Go back to " + fallbackScene.Replace("Card", "") + " to earn more.";
            notEnoughMoneyPanel.SetActive(true);
        }
    }

    public void PlayPreviousCountry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(fallbackScene);
    }

    public void UpdateMoneyDisplay()
    {
        if (globalMoneyText) globalMoneyText.text = PlayerPrefs.GetInt("totalCoins", 0).ToString();
    }
    // OYUNU TAMAMEN SIFIRLAMA FONKSÝYONU
    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteKey("totalCoins");
        PlayerPrefs.DeleteKey("FranceUnlocked");
        PlayerPrefs.DeleteKey("SpainUnlocked");
        PlayerPrefs.DeleteKey("ItalyUnlocked");
        PlayerPrefs.Save();

        UpdateMoneyDisplay();
        RefreshMapVisuals();

        // Ýþlem bitince paneli kapat
        if (resetConfirmationPanel) resetConfirmationPanel.SetActive(false);

        Debug.Log("Sýfýrlandý!");
    }
}