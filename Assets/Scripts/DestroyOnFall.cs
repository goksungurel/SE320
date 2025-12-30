using UnityEngine;

public class DestroyOnFall : MonoBehaviour
{
    public GameManagerC gameManager;   
    public AudioSource audioSource;    
    public AudioClip missSound;       

    private void OnTriggerEnter2D(Collider2D other)
    {
        // item control
        if (other.CompareTag("GoodItem"))
        {
            // disable the collider immediately to prevent double triggering
            other.enabled = false; 

            Debug.Log(other.gameObject.name + " missed!");

            // sound
            if (audioSource != null && missSound != null)
            {
                audioSource.PlayOneShot(missSound);
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
    }
}