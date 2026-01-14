using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource; 
    public AudioClip clickSound;    

    [Header("UI Panels")]
    public GameObject optionsPanel; 

    // --- PLAY BUTONU İÇİN (Sahne No: 13) ---
    public void PlayGame()
    {
        Debug.Log("Play Butonuna Basıldı!");
        StartCoroutine(LoadSceneByNumber(13));
    }

    // --- GENEL SAHNE YÜKLEME (İsimle çağırmak istersen) ---
    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadSceneByString(sceneName));
    }

    // Sahne Numarası ile Yükleme
    private IEnumerator LoadSceneByNumber(int sceneIndex)
    {
        PlaySound();
        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneIndex);
    }

    // Sahne İsmi ile Yükleme
    private IEnumerator LoadSceneByString(string sceneName)
    {
        PlaySound();
        yield return new WaitForSecondsRealtime(0.15f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    private void PlaySound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
            Debug.Log("Ses çalındı.");
        }
    }

    public void OpenOptions()
    {
        PlaySound();
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void ClosePanel(GameObject panel)
    {
        PlaySound();
        if (panel != null)
        {
            panel.SetActive(false);
            Time.timeScale = 1f; 
        }
    }

    public void QuitGame()
    {
        PlaySound();
        Debug.Log("Oyun kapatılıyor...");
        Application.Quit();
    }
}