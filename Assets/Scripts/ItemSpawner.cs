using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] itemPrefabs; 
    public int coinIndex = 6;
    public int bombIndex = 5;

    public float spawnInterval = 3f; 
    public int minSpawnForEachItem = 5; 
    
    private Dictionary<int, int> spawnCounts = new Dictionary<int, int>();
    private int lastSpawnedIndex = -1; 

    void Start()
    {
        if (itemPrefabs.Length < 1) return;

        // Sözlüğü başlat
        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            spawnCounts.Add(i, 0);
        }

        // Rutini başlat, Time.timeScale 0 iken beklemede kalacak
        StartCoroutine(SpawnItemsRoutine());
    }

    IEnumerator SpawnItemsRoutine()
    {
        while (true) // Oyun aktif olduğu sürece çalışsın
        {
            // Zaman durmuşsa (Start/Pause paneli açıksa) bekle
            if (Time.timeScale == 0f)
            {
                yield return null; 
                continue;
            }

            int itemIndexToSpawn = SelectRandomItem();
            
            if (itemIndexToSpawn != -1)
            {
                float spawnLimitX = 6.0f; 
                float randomX = Random.Range(-spawnLimitX, spawnLimitX);
                Vector3 spawnPosition = new Vector3(randomX, transform.position.y, 0);

                Instantiate(itemPrefabs[itemIndexToSpawn], spawnPosition, Quaternion.identity);
                spawnCounts[itemIndexToSpawn]++;
                lastSpawnedIndex = itemIndexToSpawn;
            }

            yield return new WaitForSeconds(spawnInterval); 
        }
    }

    private int SelectRandomItem()
{
    List<int> pool = new List<int>();

    // 1. ADIM: Eksik olan (kotası dolmamış) eşyaları tespit et
    List<int> missingItems = new List<int>();
    for (int i = 0; i < itemPrefabs.Length; i++)
    {
        if (i == bombIndex) continue;

        int target = (i == coinIndex) ? 8 : minSpawnForEachItem;
        if (spawnCounts[i] < target)
        {
            missingItems.Add(i);
        }
    }

    // 2. ADIM: Havuzu Akıllıca Doldur
    if (missingItems.Count > 0)
    {
        // Eksik olan her eşyayı havuza ekle (Öncelik veriyoruz)
        foreach (int index in missingItems)
        {
            // Şansını artırmak için 3'er kez ekliyoruz
            pool.Add(index);
            pool.Add(index);
            pool.Add(index);
        }

        // Bombayı her zaman havuza ekle (Ama çok boğmasın diye 2 kez)
        pool.Add(bombIndex);
        pool.Add(bombIndex);
    }
    else
    {
        // Eğer her şey toplandıysa, tamamen rastgele bir moda geç
        return Random.Range(0, itemPrefabs.Length);
    }

    // 3. ADIM: Seçim ve Üst Üste Aynı Şeyin Gelmesini Engelleme
    int choice = pool[Random.Range(0, pool.Count)];

    // Eğer seçilen şey bir öncekiyle aynıysa, %70 ihtimalle tekrar seç (Çeşitlilik sağlar)
    if (choice == lastSpawnedIndex && Random.value < 0.7f)
    {
        choice = pool[Random.Range(0, pool.Count)];
    }

    // Üst üste bomba koruması (Katı kural)
    if (choice == bombIndex && lastSpawnedIndex == bombIndex)
    {
        // Bombayı listedeki bir sonraki geçerli eşyaya kaydır
        choice = missingItems[Random.Range(0, missingItems.Count)];
    }

    return choice;
}
}