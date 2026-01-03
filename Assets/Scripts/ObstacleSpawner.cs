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
    private float timer;

    [Header("Coin Settings")]
    public GameObject coinPrefab;
    private int obstacleCounter = 0;
    public int spawnCoinEveryXObstacles = 2;

    void Start()
    {
        if (spHigh == null) spHigh = transform.Find("SP_High");
    }

    void Update()
    {
        if (Time.timeScale <= 0) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnSmart();
            timer = 0f;
        }
    }

    void SpawnSmart()
    {
        obstacleCounter++;

        bool spawnGround = Random.value > 0.5f;

        if (spawnGround && groundObstacles.Length > 0)
        {
            GameObject prefab = groundObstacles[Random.Range(0, groundObstacles.Length)];
            Instantiate(prefab, spLow.position, Quaternion.identity);
        }
        else if (airObstacles.Length > 0)
        {
            GameObject prefab = airObstacles[Random.Range(0, airObstacles.Length)];
            Transform airPoint = (Random.value > 0.5f) ? spMid : spHigh;
            Instantiate(prefab, airPoint.position, Quaternion.identity);
        }

        if (obstacleCounter >= spawnCoinEveryXObstacles)
        {
            if (coinPrefab != null)
            {
                
                Vector3 coinPos = spMid.position + Vector3.right * 12f;

                Instantiate(coinPrefab, coinPos, Quaternion.identity);
                obstacleCounter = 0; 
            }
        }
    }
}