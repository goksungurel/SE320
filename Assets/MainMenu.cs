using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(13);
    }

    public void QuitGame()
    {
    Application.Quit();
    Debug.Log("Game Quit"); // Editor'da görmek için
    }



}
