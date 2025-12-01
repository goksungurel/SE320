using UnityEngine;

public class DestroyOnFall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("GoodItem") || other.CompareTag("BadItem"))//this part is optional but maybe it can be useful for later
        {
            Debug.Log(other.gameObject.name + " objesi düştü ve yok edildi.");
            
            Destroy(other.gameObject); //destroys object
        }
    }
}