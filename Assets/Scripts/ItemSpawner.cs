using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    [Header("Item Prefabs")]
    public GameObject[] itemPrefabs; 
    public int coinIndex = 0;
    public int bombIndex = 1;

    [Header("Spawn Settings ")]
    // they will change for every scene
    public int countPerItem = 5; 
    public int totalCoins = 7;   
    public int totalBombs = 5;   
    
    public float minX = -6.0f;
    public float maxX = 6.0f;

    [Header("References")]
    public GameManagerC gameManager; 
    
    private List<int> plannedSpawns = new List<int>();
    private bool isSpawning = false;

    void Start()
    {

        PrepareSpawnList();
    }

    void Update()
    {
        if (!isSpawning && Time.timeScale > 0f)
        {
            StartCoroutine(SpawnRoutine());
            isSpawning = true;
        }
    }

    void PrepareSpawnList()
    {
        plannedSpawns.Clear();

        // it can work on different item arrays
        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            if (i == bombIndex || i == coinIndex) continue;
            // variables from inspector for flexibility
            for (int j = 0; j < countPerItem; j++) plannedSpawns.Add(i);
        }

        // coins
        for (int j = 0; j < totalCoins; j++) plannedSpawns.Add(coinIndex);

        // bombs
        for (int j = 0; j < totalBombs; j++) plannedSpawns.Add(bombIndex);

        for (int i = 0; i < plannedSpawns.Count; i++)
        {
            int temp = plannedSpawns[i];
            int randomIndex = Random.Range(i, plannedSpawns.Count);
            plannedSpawns[i] = plannedSpawns[randomIndex];
            plannedSpawns[randomIndex] = temp;
        }
    }

    IEnumerator SpawnRoutine()
    {
        // control for nullreference error
        if (gameManager == null) {
            Debug.LogError("nullreference");
            yield break;
        }

        float calculatedInterval = gameManager.levelDuration / plannedSpawns.Count;

        while (plannedSpawns.Count > 0)
        {
            int nextItemIndex = plannedSpawns[0];
            plannedSpawns.RemoveAt(0);

            float randomX = Random.Range(minX, maxX);
            Vector3 spawnPos = new Vector3(randomX, transform.position.y, -1f);
            Instantiate(itemPrefabs[nextItemIndex], spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(calculatedInterval);

            while (Time.timeScale == 0f)
            {
                yield return null;
            }
        }
    }
}