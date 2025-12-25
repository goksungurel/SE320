using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
 
    public GameObject[] itemPrefabs; // Düşecek tüm eşya Prefab'ları
    

    public float gameDuration = 40f; //game duration
    public float spawnInterval = 4f; // eşya düşme sıklığı
    public int minSpawnCount = 4; // min number for fallen objects
    
    public float screenWidthUnits = 8.89f; //screen width
    
    public float safetyPadding = 4.0f; // padding for objects preventing them being half inside of screen


    private Dictionary<int, int> spawnCounts = new Dictionary<int, int>();
    private float startTime;
    private float spawnRangeX; 

    void Start()
    {
        // control for preventing NullReferenceException
        if (itemPrefabs.Length == 0)
        {
             Debug.LogError("Item Prefabs list is empty");
             return;
        }


        spawnRangeX = screenWidthUnits; 

        if (safetyPadding >= spawnRangeX)
        {
            safetyPadding = 0.5f; 
            Debug.LogWarning("Safety Padding value is bigger than expected! system uses predicted value");
        }


        startTime = Time.time;
        

        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            spawnCounts.Add(i, 0);
        }

        StopAllCoroutines(); 

        StartCoroutine(SpawnItemsRoutine());
    }

    IEnumerator SpawnItemsRoutine()
    {
        // Oyun süresi boyunca döngüyü sürdür
        while (Time.time < startTime + gameDuration)
        {
            if (itemPrefabs.Length > 0)
            {
                int itemIndexToSpawn = SelectRandomItem();
            
                if (itemIndexToSpawn != -1)
                {

                    float minX = -spawnRangeX + safetyPadding;
                    float maxX = spawnRangeX - safetyPadding;
                    
              
                    float randomX = Random.Range(minX, maxX);


                    Vector3 spawnPosition = new Vector3(
                        randomX,
                        transform.position.y,
                        0
                    );

                    Instantiate(itemPrefabs[itemIndexToSpawn], spawnPosition, Quaternion.identity);
                    spawnCounts[itemIndexToSpawn]++;
                }
            }

            yield return new WaitForSeconds(spawnInterval); 
        }
        
        Debug.Log("Time is over!");
    }
    
    private int SelectRandomItem()
    {
        List<int> availableIndices = new List<int>();

        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            if (spawnCounts[i] < minSpawnCount)
            {
                availableIndices.Add(i);
            }
        }

        if (availableIndices.Count > 0)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            return availableIndices[randomIndex];
        }
        
        return Random.Range(0, itemPrefabs.Length);
    }

}