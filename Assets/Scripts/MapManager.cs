using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
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
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            OpenUnlockPanel();
        }
    }

    public void TryUnlock()
    {
        int myMoney = PlayerPrefs.GetInt("totalCoins", 0);

        if (myMoney >= currentCost)
        {
            PlayerPrefs.SetInt(targetScene.Replace("Card", "") + "Unlocked", 1);
            PlayerPrefs.SetInt("totalCoins", myMoney - currentCost);
            PlayerPrefs.Save();
            SceneManager.LoadScene(targetScene);
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
        SceneManager.LoadScene("CardGermany");
    }


    public void ClickFrance() { HandleLogic("France", 15, "CardFrance", "CardGermany"); }
    public void ClickSpain() { HandleLogic("Spain", 25, "CardSpain", "CardFrance"); }
    public void ClickItaly() { HandleLogic("Italy", 40, "CardItaly", "CardSpain"); }

    public void OpenResetConfirmation()
    {
        if (resetConfirmationPanel != null) resetConfirmationPanel.SetActive(true);
    }

    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteKey("totalCoins");
        PlayerPrefs.DeleteKey("FranceUnlocked");
        PlayerPrefs.DeleteKey("SpainUnlocked");
        PlayerPrefs.DeleteKey("ItalyUnlocked");
        PlayerPrefs.Save();

        if (resetConfirmationPanel != null) resetConfirmationPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void CheckAllUnlocked()
    {
        
        if (PlayerPrefs.GetInt("ItalyUnlocked", 0) == 1)
        {
            if (comingSoonPanel != null) comingSoonPanel.SetActive(true);
        }
    }
}