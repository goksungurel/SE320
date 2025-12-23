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

    private float timer;

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
            Debug.Log("Yer engeli (Taþ/Çalý) SP_Low'da doðdu.");
        }
        else if (airObstacles.Length > 0)
        {
           
            GameObject prefab = airObstacles[Random.Range(0, airObstacles.Length)];
            
            
            Transform airPoint = (Random.value > 0.5f) ? spMid : spHigh;
            
            Instantiate(prefab, airPoint.position, Quaternion.identity);
            Debug.Log("Hava engeli (Kuþ/Yarasa) havada doðdu.");
        }
    }
}