using UnityEngine;

public class DestroyOffscreen : MonoBehaviour
{
    public Transform player;
    public float destroyBehindX = 20f; // player'ýn 20 birim gerisine düþünce sil

    void Update()
    {
        if (player == null) return;

        if (transform.position.x < player.position.x - destroyBehindX)
        {
            Destroy(gameObject);
        }
    }
}
