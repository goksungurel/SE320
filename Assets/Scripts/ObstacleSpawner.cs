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
    public float spawnInterval = 2.5f;
    private float obstacleTimer;

    [Header("Coin Settings")]
    public GameObject coinPrefab;
    public float coinSpawnInterval = 4.0f; 
    private float coinTimer;

    void Update()
    {
        if (Time.timeScale <= 0) return;

        // 1. ENGEL ZAMANLAYICISI
        obstacleTimer += Time.deltaTime;
        if (obstacleTimer >= spawnInterval)
        {
            SpawnObstacle();
            obstacleTimer = 0f;
        }

        
        coinTimer += Time.deltaTime;
        if (coinTimer >= coinSpawnInterval)
        {
            SpawnCoinIndependent();
            coinTimer = 0f;
        }
    }

    void SpawnObstacle()
    {
        bool spawnGround = Random.value > 0.5f;
        if (spawnGround && groundObstacles.Length > 0)
        {
            Instantiate(groundObstacles[Random.Range(0, groundObstacles.Length)], spLow.position, Quaternion.identity);
        }
        else if (airObstacles.Length > 0)
        {
            Transform airPoint = (Random.value > 0.5f) ? spMid : spHigh;
            Instantiate(airObstacles[Random.Range(0, airObstacles.Length)], airPoint.position, Quaternion.identity);
        }
    }

    void SpawnCoinIndependent()
    {
        if (coinPrefab == null) return;

        float randomY = Random.Range(spLow.position.y + 1.5f, spHigh.position.y);
        Vector3 coinPos = new Vector3(spLow.position.x + 25f, randomY, 0);

        Instantiate(coinPrefab, coinPos, Quaternion.identity);
    }
}