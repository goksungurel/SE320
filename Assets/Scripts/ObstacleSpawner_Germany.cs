using UnityEngine;

public class ObstacleSpawner_Germany : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject groundObstacle; // tart
    public GameObject tower;
    public GameObject[] airObstacles;

    [Header("Spawn Points")]
    public Transform spLow;    // for Tart
    public Transform spTower;  // for Tower (High enough)
    public Transform spMid;    // for Coins & Birds
    public Transform spHigh;

 
    private float spawnInterval = 3.5f; // if too fast, make it 4.0
    private float timer;

    [Header("Coin Settings")]
    public GameObject coinPrefab;
    private int spawnCycle = 0; // to keep track of what to spawn

    void Start()
    {
        if (spHigh == null) spHigh = transform.Find("SP_High");
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            // Reset timer
            timer = 0f;

            // Logic: 1 Obstacle -> 1 Coin -> 1 Obstacle...
            spawnCycle++;

            if (spawnCycle % 2 != 0)
            {
                // ODD CYCLES: SPAWN OBSTACLE
                SpawnObstacleOnly();
            }
            else
            {
                // EVEN CYCLES: SPAWN COIN ONLY
                SpawnCoinOnly();
            }
        }
    }

    void SpawnObstacleOnly()
    {
        // Pick ONLY ONE type of obstacle
        int choice = Random.Range(0, 3); // 0: Tart, 1: Tower, 2: Air

        if (choice == 0) // TART
        {
            Instantiate(groundObstacle, spLow.position, Quaternion.identity);
        }
        else if (choice == 1) // TOWER
        {
            Instantiate(tower, spTower.position, Quaternion.identity);
        }
        else if (choice == 2 && airObstacles.Length > 0) // AIR
        {
            GameObject bird = airObstacles[Random.Range(0, airObstacles.Length)];
            Transform point = (Random.value > 0.5f) ? spMid : spHigh;
            Instantiate(bird, point.position, Quaternion.identity);
        }
    }

    void SpawnCoinOnly()
    {
        if (coinPrefab != null)
        {
            // Spawn coin at a safe middle height, far from ground
            Instantiate(coinPrefab, spMid.position, Quaternion.identity);
        }
    }
}