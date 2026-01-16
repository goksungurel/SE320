using UnityEngine;

public class DestroyOnFall : MonoBehaviour
{
    public GameManagerC gameManager;   
    public AudioSource audioSource;    
    public AudioClip missSound;     
    public GameObject dustEffect;  

    private void OnTriggerEnter2D(Collider2D other)
    {
        // item control
        if (other.CompareTag("GoodItem"))
        {
            if (dustEffect != null)
            Instantiate(dustEffect, other.transform.position, Quaternion.identity);
            // disable the collider immediately to prevent double triggering
            other.enabled = false; 

            Debug.Log(other.gameObject.name + " missed!");

            // sound
            if (audioSource != null && missSound != null)
            {
                audioSource.PlayOneShot(missSound,0.3f);
            }

            // takedamage part
            if (gameManager != null)
            {
                gameManager.TakeDamage(1); 
            }

            // destroy object
            Destroy(other.gameObject); 
        }
        // item control
        else if (other.CompareTag("BadItem"))
        {
            // when bomb falls, just destroy
            other.enabled = false;
            Destroy(other.gameObject);
        }
        //coin will not cause to loss of heart
        else if(other.CompareTag("Coin"))
        {
            if (dustEffect != null)
            Instantiate(dustEffect, other.transform.position, Quaternion.identity);
        }
    }
}