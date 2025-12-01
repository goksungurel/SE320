using UnityEngine;

public class PlayerCatcher : MonoBehaviour
{
    public float speed = 250f; // speed of playercatcher object
    
    public float padding = 0.5f; 
    
    public GameManagerC gameManager; 
    public AudioSource audioSource; 
    public AudioClip badItemSound;  
    // for camera boundaries
    private float screenLeft;
    private float screenRight;

    void Start()
    {
        float screenHeight = Camera.main.orthographicSize; 
        float screenWidth = screenHeight * Camera.main.aspect;

        // object's boundaries for screen
        screenLeft = -screenWidth;
        screenRight = screenWidth;
    }


    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); 

        Vector3 movement = new Vector3(horizontalInput * speed * Time.fixedDeltaTime, 0, 0);
        Vector3 newPosition = transform.position + movement;

      //karakterler dışarı taşmasın diye
        newPosition.x = Mathf.Clamp(
            newPosition.x, 
            screenLeft + padding, 
            screenRight - padding 
        );
        
        transform.position = newPosition;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // controlling tag for score 
        if (other.CompareTag("GoodItem"))
        {
            Debug.Log("Good Item Catched! Score added!");
            // Burada skor ekleme kodu gelecek

            Destroy(other.gameObject); 
        }
        else if (other.CompareTag("BadItem"))
        {
            Debug.Log("Bad Item catched! Loss of heart :(");
            
          
            if (audioSource != null && badItemSound != null)
            {
                audioSource.PlayOneShot(badItemSound); // Sesi çal
            }

            if (gameManager != null)
            {
                // GameManager'daki can düşürme metodu gelecek sonra

                gameManager.LoseLife(); 
            }
           
            Destroy(other.gameObject); 
        }
    }
    }
