using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] groundObstacles; 
    public GameObject[] airObstacles;    

    [Header("Spawn Points")]
    public Transform spLow;  
    public Transform spMid;  
    public Transform spHigh; 

    [Header("Timing")]
    public float spawnInterval = 2.0f;

    [Header("Coin Settings")]
    public GameObject coinPrefab;

    private float timer;
    void Start()
    {
        if (spHigh == null)
        {
            spHigh = transform.Find("SP_High"); 
        }

        if (coinPrefab == null)
        {
            
            Debug.LogError("Coin Prefab hala bulunamad�! L�tfen Inspector'dan s�r�kle.");
        }
    }
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnSmart();
            timer = 0f;
        }
    }

    void SpawnSmart()
    {
        bool spawnGround = Random.value > 0.5f;

        if (spawnGround && groundObstacles.Length > 0)
        {
            
            GameObject prefab = groundObstacles[Random.Range(0, groundObstacles.Length)];
            Instantiate(prefab, spLow.position, Quaternion.identity);
            Debug.Log("Yer engeli (Ta�/�al�) SP_Low'da do�du.");
        }
        else if (airObstacles.Length > 0)
        {
        
            GameObject prefab = airObstacles[Random.Range(0, airObstacles.Length)];
            
            
            Transform airPoint = (Random.value > 0.5f) ? spMid : spHigh;
            
            Instantiate(prefab, airPoint.position, Quaternion.identity);
            Debug.Log("Hava engeli (Ku�/Yarasa) havada do�du.");
        }

        if (Random.value > 0.5f) 
        {
            // spMid dolu mu kontrol et, bo�sa hata verme (koruma)
            if (spHigh != null && coinPrefab != null)
            {
                // Engelin biraz �n�ne veya arkas�na atmak i�in Vector3.right kullanabilirsin
                Instantiate(coinPrefab, spHigh.position + Vector3.right * 2f, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("D�KKAT: spMid veya coinPrefab atanmam��!");
            }
        }
    }

}