using UnityEngine;

public class PlayerCatcher : MonoBehaviour
{
    public float speed = 250f; // speed of playercatcher object
    
    public float padding = 0.5f; 
    
    public GameManagerC gameManager; 
    public AudioSource audioSource; 
    public AudioClip badItemSound;  
    public AudioClip goodItemSound;
    public AudioClip coinSound;
    // for camera boundaries
    private float screenLeft;
    private float screenRight;

    private Animator animator;
    public GameObject dustEffect;
    void Start()
    {
        animator = GetComponent<Animator>();
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
    if (other.CompareTag("GoodItem"))
    {
        if (animator != null) animator.SetTrigger("Catch");
        //itemdatadan direkt anlaşılacak
        if (gameManager != null)
        {
            gameManager.CollectItem(other.tag, other.gameObject); 
        }

        if (audioSource != null && goodItemSound != null)
        {
            audioSource.PlayOneShot(goodItemSound, 1.5f);
        }

        Destroy(other.gameObject); 
    }
    else if (other.CompareTag("BadItem"))
    {
        if (dustEffect != null)
            Instantiate(dustEffect, transform.position, Quaternion.identity);
        if (audioSource != null && badItemSound != null)
        {
            audioSource.PlayOneShot(badItemSound, 0.3f);
        }

        if (gameManager != null)
        {
            gameManager.TakeDamage(1); 
        }

        Destroy(other.gameObject); 
    }
    else if (other.CompareTag("Coin"))
    {
        if (animator != null) animator.SetTrigger("Catch");

        if (gameManager != null)
        {
            gameManager.CollectItem("Coin", other.gameObject); 
        }
        
        if (audioSource != null && coinSound != null)
        {
            audioSource.PlayOneShot(coinSound, 1.1f);
        }
        
        Destroy(other.gameObject); 
    }
}}