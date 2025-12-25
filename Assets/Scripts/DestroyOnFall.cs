using UnityEngine;

public class DestroyOnFall : MonoBehaviour
{
    public GameManagerC gameManager;   
    public AudioSource audioSource;    
    public AudioClip missSound;       

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. GoodItem etiketi kontrolü
        if (other.CompareTag("GoodItem"))
        {
            // Çift tetiklemeyi önlemek için collider'ı hemen kapatıyoruz
            other.enabled = false; 

            Debug.Log(other.gameObject.name + " missed!");

            // Ses çalma
            if (audioSource != null && missSound != null)
            {
                audioSource.PlayOneShot(missSound);
            }

            // Can düşürme
            if (gameManager != null)
            {
                gameManager.TakeDamage(1); 
            }

            // Objeyi yok et
            Destroy(other.gameObject); 
        }
        // 2. BadItem etiketi kontrolü
        else if (other.CompareTag("BadItem"))
        {
            // Bomba düştüğünde ceza yok, sadece yok et
            other.enabled = false;
            Destroy(other.gameObject);
        }
    }
}