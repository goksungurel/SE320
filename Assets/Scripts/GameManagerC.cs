
using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro; 

public class GameManagerC : MonoBehaviour
{

    public int currentLives = 3; 
    public int maxBadItems = 3; 


    public TextMeshProUGUI bombCountText; 


    void Start()
    {

        UpdateBombCountUI();
    }



    public void LoseLife()
    {
        currentLives--;
        Debug.Log("Loss of heart. Current live: " + currentLives);
    UpdateBombCountUI();
       
    }

    void UpdateBombCountUI()
{

        if (currentLives <= 0)
        {
            Debug.Log("Game Over! ");
            RestartGame();
        }
    if (bombCountText != null)
    {
     

        bombCountText.text = $"Can: {currentLives} / {maxBadItems}"; 
    }
}

    

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }
}