using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] obstaclePrefabs;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Timing")]
    public float spawnInterval = 1.2f;

    [Header("Anti-overlap")]
    public LayerMask obstacleLayer;
    public Vector2 checkSize = new Vector2(2.5f, 2.0f);
    public int maxAttempts = 10;

    private float timer;

    void Update()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            TrySpawn();
        }
    }

    void TrySpawn()
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Transform p = spawnPoints[Random.Range(0, spawnPoints.Length)];
            if (p == null) continue;

            if (IsAreaFree(p.position))
            {
                GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                Instantiate(prefab, p.position, Quaternion.identity);
                return;
            }
        }

    }

    bool IsAreaFree(Vector2 pos)
    {
        Collider2D hit = Physics2D.OverlapBox(pos, checkSize, 0f, obstacleLayer);
        return hit == null;
    }

    void OnDrawGizmosSelected()
    {
        if (spawnPoints == null) return;
        Gizmos.color = Color.cyan;

        foreach (var p in spawnPoints)
        {
            if (p == null) continue;
            Gizmos.DrawWireCube(p.position, checkSize);
        }
    }
}
