using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    //config params
    [SerializeField] int maxSpawnableObjects;
    [SerializeField] int minSpawnDistance;
    [SerializeField] int maxSpawnDistance;
    [SerializeField] int minDistanceBetweenObjects;
    [SerializeField] List<GameObject> spawnableObjects;
    [SerializeField] List<int> spawnableChances;


    //cached references
    Xorn xorn;
    DirtBlocks dirtGrid;
    List<float> spawnThreshold = new List<float>();
    public List<GameObject> spawnedObjects = new List<GameObject>();
    
    //track all previously spawned objects - only set these once
    List<int> spawnedIndex = new List<int>();
    List<Vector3> spawnedPos = new List<Vector3>();
    List<bool> spawnedActive = new List<bool>();
    List<GameObject> spawnedType = new List<GameObject>();
    public List<bool> destroyedGem = new List<bool>();
    public List<bool> maskActive = new List<bool>();
    
    // Start is called before the first frame update
    void Start()
    {
        xorn = FindObjectOfType<Xorn>();
        dirtGrid = FindObjectOfType<DirtBlocks>();
        InitializeSpawnChances();
    }

    private void InitializeSpawnChances()
    {
        int countThreshold = 0;
        for(int i = 0; i < spawnableChances.Count; i++)
        {
            countThreshold += spawnableChances[i];
            spawnThreshold.Add(countThreshold);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DespawnDistantObjects();
        GenerateRandomObjects();
    }

    private void GenerateRandomObjects()
    {
        if (spawnedObjects.Count > maxSpawnableObjects) { return; }
        if (!CheckForPreviousSpawns()) { return; }
        Vector3 randomPosition = GenerateSpawnPos();
        int newSpawnIndex = ValidateSpawnPos(randomPosition);
        if (newSpawnIndex == -1) { return; }
        GameObject objectToSpawn = PickObjectToSpawn();
        if (objectToSpawn == null) { return; }
        var newSpawn = Instantiate(objectToSpawn, randomPosition, Quaternion.identity);
        newSpawn.GetComponent<SpawnedObject>().myIndex = newSpawnIndex;
        spawnedObjects.Add(newSpawn);
        spawnedIndex.Insert(newSpawnIndex, newSpawnIndex);
        spawnedPos.Insert(newSpawnIndex, newSpawn.transform.position);
        spawnedActive.Insert(newSpawnIndex, true);
        destroyedGem.Insert(newSpawnIndex, false);
        maskActive.Insert(newSpawnIndex, false);
        foreach(GameObject gameObject in spawnableObjects)
        {
            if (objectToSpawn == gameObject)
            {
                spawnedType.Insert(newSpawnIndex, gameObject);
            }
        }
        newSpawn.name += newSpawnIndex;
    }

    private GameObject PickObjectToSpawn()
    {
        float sumOfChances = 0;
        GameObject spawnedObject = null;
        foreach(int spawnChance in spawnableChances)
        {
            sumOfChances += spawnChance;
        }
        float pickSpawn = UnityEngine.Random.Range(0, sumOfChances);
        for(int i = 0; i < spawnThreshold.Count; i++)
        {
            if(pickSpawn <= spawnThreshold[i])
            {
                spawnedObject = spawnableObjects[i];
                break;
            }
        }
        return spawnedObject;
    }

    private bool CheckForPreviousSpawns()
    {
        for(int i = 0; i < spawnedPos.Count; i++)
        {
            if (!spawnedActive[i])
            {
                var distToPlayer = Vector3.Magnitude(spawnedPos[i] - xorn.transform.position);
                if (distToPlayer < maxSpawnDistance)
                {
                    CreatePreviousObject(i);
                    return false;
                }
            }
        }
        return true;
    }

    private void CreatePreviousObject(int indexToSpawn)
    {
        var newSpawn = Instantiate(spawnedType[indexToSpawn], spawnedPos[indexToSpawn], Quaternion.identity);
        spawnedObjects.Add(newSpawn);
        newSpawn.name += indexToSpawn;
        SpawnedObject spawnedComponent = newSpawn.GetComponent<SpawnedObject>();
        newSpawn.GetComponent<SpawnedObject>().myIndex = indexToSpawn;
        if(destroyedGem[indexToSpawn])
        {
            newSpawn.GetComponent<SpriteRenderer>().sprite = newSpawn.GetComponent<SpawnedObject>().brokenSprite;
        }
        spawnedActive[indexToSpawn] = true;
    }

    private int ValidateSpawnPos(Vector3 randomPosition)
    {
        for(int index = 0; index < spawnedPos.Count; index++)
        {
            if (randomPosition == spawnedPos[index])
            {
                
                    return -1;
                
            }
            var distanceToOtherSpawn = Vector3.Distance(randomPosition, spawnedPos[index]);
            if (distanceToOtherSpawn < minSpawnDistance)
            {
                return -1;
            }
        }
        return spawnedIndex.Count;
    }

    private Vector3 GenerateSpawnPos()
    {
        int randomX = UnityEngine.Random.Range(-maxSpawnDistance, maxSpawnDistance);
        int randomY = UnityEngine.Random.Range(-maxSpawnDistance, maxSpawnDistance);
        Vector3 randomPosition = new Vector3(randomX, randomY, 0) + xorn.transform.position;
        int newX = Mathf.RoundToInt(randomPosition.x);
        int newY = Mathf.RoundToInt(randomPosition.y);
        Vector3 adjustedPosition = new Vector3(newX, newY, transform.position.z);
        return adjustedPosition;
    }

    private void DespawnDistantObjects()
    {
        foreach(GameObject spawnedObject in spawnedObjects)
        {
            var distFromPlayer = Vector3.Magnitude(spawnedObject.transform.position - xorn.transform.position);
            if (distFromPlayer > maxSpawnDistance + 1)
            {
                int index = -1;
                for (int i = 0; i < spawnedPos.Count; i++)
                {
                    if (spawnedPos[i] == spawnedObject.transform.position)
                    {
                        index = i;
                        break;
                    }
                }
                spawnedObjects.Remove(spawnedObject);
                spawnedActive[index] = false;
                Destroy(spawnedObject);
                return;
            }
        }
        
    }
}
