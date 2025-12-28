using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void OpenWorldMap()
    {
        SceneManager.LoadSceneAsync("WorldMap");
    }
    public void QuitGame()
    {
    Application.Quit();
    Debug.Log("Game Quit"); // Editor'da görmek için
    }



}
