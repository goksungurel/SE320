using UnityEngine;

public class DestroyOnFall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("GoodItem") || other.CompareTag("BadItem")|| other.CompareTag("Coin"))//this part is optional but maybe it can be useful for later
        {
            Debug.Log(other.gameObject.name + " object destroyed.");
            
            Destroy(other.gameObject); //destroys object
        }
    }
}