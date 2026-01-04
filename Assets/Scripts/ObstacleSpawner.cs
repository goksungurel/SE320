using UnityEngine;
using UnityEngine.SceneManagement;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] groundObstacles;
    public GameObject[] airObstacles;
    public GameObject projectilePrefab;
    public Transform spLow; public Transform spMid; public Transform spHigh;
    public float spawnInterval = 2.5f;
    private float obstacleTimer;
    public GameObject coinPrefab;
    public float coinSpawnInterval = 4.0f;
    private float coinTimer;

    void Update()
    {
        if (Time.timeScale <= 0) return;
        obstacleTimer += Time.deltaTime;
        if (obstacleTimer >= spawnInterval) { SpawnObstacle(); obstacleTimer = 0f; }
        coinTimer += Time.deltaTime;
        if (coinTimer >= coinSpawnInterval) { SpawnCoinIndependent(); coinTimer = 0f; }
    }

    void SpawnObstacle()
    {
        bool isAir = Random.value > 0.5f;
        if (!isAir && groundObstacles.Length > 0)
            Instantiate(groundObstacles[Random.Range(0, groundObstacles.Length)], spLow.position, Quaternion.identity);
        else if (airObstacles.Length > 0)
        {
            Transform airPoint = (Random.value > 0.5f) ? spMid : spHigh;
            GameObject airObj = Instantiate(airObstacles[Random.Range(0, airObstacles.Length)], airPoint.position, Quaternion.identity);
            string sn = SceneManager.GetActiveScene().name;
            if (sn.Contains("Spain")) airObj.AddComponent<SpainFloating>();
            if (sn.Contains("Italy"))
            {
                var s = airObj.AddComponent<ItalyShooter>();
                s.bullet = projectilePrefab;
            }
        }
    }

    void SpawnCoinIndependent()
    {
        float rY = Random.Range(spLow.position.y + 1.5f, spHigh.position.y);
        Instantiate(coinPrefab, new Vector3(spLow.position.x + 25f, rY, 0), Quaternion.identity);
    }
}

public class SpainFloating : MonoBehaviour
{
    float startY;
    void Start() { startY = transform.position.y; }
    void Update()
    {
        float nY = startY + Mathf.Sin(Time.time * 3f) * 1.2f;
        nY = Mathf.Max(nY, -2.5f); // YER SINIRI: -2.5f deðerini sahnene göre ayarla!
        transform.position = new Vector3(transform.position.x, nY, transform.position.z);
    }
}

public class ItalyShooter : MonoBehaviour
{
    public GameObject bullet;
    void Start() { Invoke("Shoot", 1.0f); }
    void Shoot()
    {
        if (bullet)
        {
            GameObject b = Instantiate(bullet, transform.position, Quaternion.identity);

            // BURAYI GÜNCELLEDÝK: Artýk tag "Bullet"
            b.tag = "Bullet";

            Rigidbody2D rb = b.GetComponent<Rigidbody2D>() ?? b.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.left * 5f;

            // Eðer BulletLogic kullanýyorsan oradaki tag kontrolünü de "Bullet" yapabilirsin
            Destroy(b, 4f);
        }
    }
}

public class BulletLogic : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager3.Instance.TakeDamage(1); // Mermi deðince can gitsin
            GameManager3.Instance.PlayBulletHitSound(); // Mermi çarpma sesi
            Destroy(gameObject);
        }
    }
}