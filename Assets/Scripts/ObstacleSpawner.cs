using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] groundObstacles; // stones and bushes
    public GameObject[] airObstacles;    // birds and bats

    [Header("Spawn Points")]
    public Transform spLow;
    public Transform spMid;
    public Transform spHigh;

    [Header("Timing")]
    public float spawnInterval = 2.0f; // how fast objects appear

    [Header("Coin Settings")]
    public GameObject coinPrefab;

    private float timer;

    void Start()
    {
        // check if high spawn point is there
        if (spHigh == null)
        {
            spHigh = transform.Find("SP_High");
        }

        // if i forgot coin prefab, show error in console
        if (coinPrefab == null)
        {
            Debug.LogError("Coin Prefab still missing! please drag it in inspector.");
        }
    }

    void Update()
    {
        timer += Time.deltaTime; // timer starts counting here

        if (timer >= spawnInterval)
        {
            SpawnSmart(); // make magic happen
            timer = 0f; // reset timer for next round
        }
    }

    void SpawnSmart()
    {
        // choose between ground or air randomly
        bool spawnGround = Random.value > 0.5f;

        if (spawnGround && groundObstacles.Length > 0)
        {
            // spawn a random eiffel tower or bagget at low point
            GameObject prefab = groundObstacles[Random.Range(0, groundObstacles.Length)];
            Instantiate(prefab, spLow.position, Quaternion.identity);
            Debug.Log("Ground obstacle spawned at SP_Low.");
        }
        else if (airObstacles.Length > 0)
        {
            // pick a random bird and spawn at mid or high point
            GameObject prefab = airObstacles[Random.Range(0, airObstacles.Length)];
            Transform airPoint = (Random.value > 0.5f) ? spMid : spHigh;

            Instantiate(prefab, airPoint.position, Quaternion.identity);
            Debug.Log("Air obstacle spawned in sky.");
        }

        // maybe spawn a coin too? 50% chance
        if (Random.value > 0.4f)
        {
            if (spHigh != null && coinPrefab != null)
            {
                // put coin a bit in front of the obstacle (Vector3.right * 2)
                Instantiate(coinPrefab, spHigh.position + Vector3.right * 2f, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Watch out: spMid or coinPrefab not set!");
            }
        }
    }
}