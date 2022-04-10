using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerSpawner : MonoBehaviour
{
    //config params
    [SerializeField] Adventurer adventurerPrefab;
    [SerializeField] int maxAdventurers = 10;
    [SerializeField] float spawnDistance = 5f;
    [SerializeField] float spawnCooldown = 3f;

    //cached references
    Xorn xorn;
    public int currentSpawned;
    DirtBlocks dirtBlocks;
    Vector3Int lastSpawnedPos;
    float spawnTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        xorn = FindObjectOfType<Xorn>();
        dirtBlocks = FindObjectOfType<DirtBlocks>();
        spawnTimer = spawnCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        SpawnAdventurers();
    }

    private void SpawnAdventurers()
    {
        if (spawnTimer >= spawnCooldown)
        {
            List<Vector3Int> possibleSpawns = new List<Vector3Int>();
            if (currentSpawned < maxAdventurers)
            {
                foreach (Vector3Int tilePos in dirtBlocks.removedTiles)
                {
                    float distToXorn = Vector3.Distance(xorn.transform.position, tilePos);
                    if (distToXorn >= spawnDistance && tilePos != lastSpawnedPos)
                    {
                        possibleSpawns.Add(tilePos);
                    }
                }
            }
            if (possibleSpawns.Count == 0) { return; }
            int newSpawnIndex = UnityEngine.Random.Range(0, possibleSpawns.Count);
            GameObject newAdventurer = Instantiate(adventurerPrefab.gameObject, possibleSpawns[newSpawnIndex], Quaternion.identity);
            lastSpawnedPos = possibleSpawns[newSpawnIndex];
            newAdventurer.transform.SetParent(transform);
            newAdventurer.GetComponent<Adventurer>().myIndex = newSpawnIndex;
            currentSpawned++;
            spawnTimer = 0;
        }
        else
        {
            spawnTimer += Time.deltaTime;
        }
    }
}
