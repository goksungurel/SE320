using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    [Header("Item Prefabs")]
    public GameObject[] itemPrefabs; 
    public int coinIndex = 6;
    public int bombIndex = 5;

    [Header("Spawn Settings")]
    public float spawnInterval = 1.2f; 
    public float minX = -6.0f;
    public float maxX = 6.0f;

    private List<int> plannedSpawns = new List<int>();
    private bool isSpawning = false;

    void Start()
    {

        PrepareLevel2List();
    }

    void Update()
    {

        if (!isSpawning && Time.timeScale > 0f)
        {
            StartCoroutine(SpawnRoutine());
            isSpawning = true;
        }
    }

    void PrepareLevel2List()
    {
        plannedSpawns.Clear();

        // 6 ana itemdan 6'şar tane (Toplam 36)
        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            if (i == bombIndex || i == coinIndex) continue;
            for (int j = 0; j < 6; j++) plannedSpawns.Add(i);
        }

        // 9 coin (7 for winning)
        for (int j = 0; j < 9; j++) plannedSpawns.Add(coinIndex);

        // 5 bomb
        for (int j = 0; j < 5; j++) plannedSpawns.Add(bombIndex);


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

    float calculatedInterval = 60f / plannedSpawns.Count; 

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