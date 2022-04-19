using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Tilemaps;

public class Lava : MonoBehaviour
{
    //config parameters
    [SerializeField] int halfSquareWidth;
    [SerializeField] TileBase ruleTile;
    [SerializeField] Tilemap tileMap;
    [SerializeField] int maxSpawnDistFromPlayer = 15;
    [SerializeField] int minSpawnDistFromPlayer = 10;
    [SerializeField] int floesToGenerate = 3;
    [SerializeField] int maxFloeLength = 10;
    [SerializeField] int minFloeLength = 3;

    //cached references
    Light2D lavaLight;
    Xorn xorn;
    List<Vector3Int> lavaBridges = new List<Vector3Int>();
    List<Vector3Int> activeLavaTiles = new List<Vector3Int>();
    public List<Vector3Int> floeStartPoints = new List<Vector3Int>();

    // Start is called before the first frame update
    void Start()
    {
        lavaLight = GetComponentInChildren<Light2D>();
        xorn = FindObjectOfType<Xorn>();
        GenerateLavaIsland();
    }

    // Update is called once per frame
    void Update()
    {
        GenerateLavaFloes();
        //LavaGlow();
    }

    private void GenerateLavaFloes()
    {
        //update floe counts in range of player
        Vector3Int roundeXornPos = Vector3Int.RoundToInt(xorn.transform.position);
        int floesInRange = 0;
        foreach (Vector3Int floe in floeStartPoints)
        {
            int distToPlayer = Mathf.RoundToInt(Vector3Int.Distance(roundeXornPos, floe));
            if (distToPlayer < maxSpawnDistFromPlayer)
            {
                floesInRange++;
            }
        }
        if (floesInRange > floesToGenerate) { return; }
        //pick starting point
        int randomX = UnityEngine.Random.Range(roundeXornPos.x - maxSpawnDistFromPlayer, roundeXornPos.x + maxSpawnDistFromPlayer + 1);
        int randomY = UnityEngine.Random.Range(roundeXornPos.y - maxSpawnDistFromPlayer, roundeXornPos.y + maxSpawnDistFromPlayer + 1);
        Vector3Int newFloePos = new Vector3Int(randomX, randomY, 0);
        int floeToPlayer = Mathf.RoundToInt(Vector3Int.Distance(roundeXornPos, newFloePos));
        if (newFloePos == roundeXornPos || floeToPlayer < minSpawnDistFromPlayer) { return; }
        floeStartPoints.Add(newFloePos);
        //pick direction
        List<Vector3Int> directions = new List<Vector3Int>();
        if (newFloePos.x > roundeXornPos.x)
        {
            directions.Add(Vector3Int.right);
        }
        else { directions.Add(Vector3Int.left); }
        if(newFloePos.y > roundeXornPos.y)
        {
            directions.Add(Vector3Int.up);
        }
        else { directions.Add(Vector3Int.down); }
        if(directions.Count == 0) { return; }
        int randomDir = UnityEngine.Random.Range(0, directions.Count);
        //pick length
        int randomLength = UnityEngine.Random.Range(minFloeLength, maxFloeLength + 1);
        //generate
        for(int length = 1; length <= randomLength; length++)
        {
            Vector3Int newPos = (directions[randomDir] * length) + newFloePos;
            tileMap.SetTile(newPos, ruleTile);
        }

    }
    private void GenerateLavaIsland()
    {
        //assemble tiles to populate as list
        Vector3Int roundeXornPos = Vector3Int.RoundToInt(xorn.transform.position);
        List<Vector3Int> lavaTileVectors = new List<Vector3Int>();
        for(int x = -halfSquareWidth; x <= halfSquareWidth; x++)
        {
            Vector3Int newTopSideTile = new Vector3Int(roundeXornPos.x + x, roundeXornPos.y + halfSquareWidth, 0);
            Vector3Int newBotSideTile = new Vector3Int(roundeXornPos.x + x, roundeXornPos.y - halfSquareWidth, 0);
            if (!lavaTileVectors.Contains(newTopSideTile) && !lavaBridges.Contains(newTopSideTile) && !activeLavaTiles.Contains(newTopSideTile)) 
            { lavaTileVectors.Add(newTopSideTile); }
            if (!lavaTileVectors.Contains(newBotSideTile) && !lavaBridges.Contains(newBotSideTile) && !activeLavaTiles.Contains(newBotSideTile)) 
            { lavaTileVectors.Add(newBotSideTile); }
        }
        for (int y = -halfSquareWidth; y <= halfSquareWidth; y++)
        {
            Vector3Int newRightSideTile = new Vector3Int(roundeXornPos.x + halfSquareWidth, roundeXornPos.y + y, 0);
            Vector3Int newLeftSideTile = new Vector3Int(roundeXornPos.x - halfSquareWidth, roundeXornPos.y + y, 0);
            if (!lavaTileVectors.Contains(newRightSideTile) && !lavaBridges.Contains(newRightSideTile) && !activeLavaTiles.Contains(newRightSideTile)) 
            { lavaTileVectors.Add(newRightSideTile); }
            if (!lavaTileVectors.Contains(newLeftSideTile) && !lavaBridges.Contains(newLeftSideTile) && !activeLavaTiles.Contains(newLeftSideTile)) 
            { lavaTileVectors.Add(newLeftSideTile); }
        }
        //remove one randomly; don't remove a corner or pick a space that already is a bridge or lava
        int indexToRemove = UnityEngine.Random.Range(0, lavaTileVectors.Count);
        while (lavaTileVectors[indexToRemove] == roundeXornPos + new Vector3Int(-halfSquareWidth,halfSquareWidth,0) 
            || lavaTileVectors[indexToRemove] == roundeXornPos + new Vector3Int(halfSquareWidth, halfSquareWidth, 0)
            || lavaTileVectors[indexToRemove] == roundeXornPos + new Vector3Int(halfSquareWidth, -halfSquareWidth, 0)
            || lavaTileVectors[indexToRemove] == roundeXornPos + new Vector3Int(-halfSquareWidth, -halfSquareWidth, 0)
            || lavaBridges.Contains(lavaTileVectors[indexToRemove])
            || activeLavaTiles.Contains(lavaTileVectors[indexToRemove]))
        {
           indexToRemove = UnityEngine.Random.Range(0, lavaTileVectors.Count);
        }
        lavaBridges.Add(lavaTileVectors[indexToRemove]);
        lavaTileVectors.RemoveAt(indexToRemove);
        //create tiles
        foreach (Vector3Int tilePos in lavaTileVectors)
        {
            tileMap.SetTile(tilePos,ruleTile);
            activeLavaTiles.Add(tilePos);
        }
    }
}
