using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DirtBlocks : MonoBehaviour
{
    //config params
    [SerializeField] TileBase ruleTile;
    [SerializeField] Tilemap tileMap;
    [SerializeField] Camera mainCamera;

    //cached references 
    Xorn xorn;
    int maxSpawnX;
    int maxSpawnY;
    List<Vector3Int> removedTiles = new List<Vector3Int>();

    // Start is called before the first frame update
    void Start()
    {
        xorn = FindObjectOfType<Xorn>();
    }

    // Update is called once per frame
    void Update()
    {
        GenerateTiles();
        DigOutTile();
    }

    private void GenerateTiles()
    {
        maxSpawnX = (int)Mathf.Ceil(mainCamera.orthographicSize) * 2 + 2;
        maxSpawnY = maxSpawnX * Screen.height / Screen.width + 2;
        for (int x = -maxSpawnX; x < maxSpawnX; x++)
        {
            for (int y = -maxSpawnY; y < maxSpawnY; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, (int)Mathf.Ceil(transform.position.z)) + Vector3Int.RoundToInt(xorn.transform.position);
                bool alreadyDestroyed = false;
                foreach(Vector3Int position in removedTiles)
                {
                    if(position == pos)
                    {
                        alreadyDestroyed = true;
                    }
                }
                if (!tileMap.HasTile(pos) && !alreadyDestroyed)
                {
                    tileMap.SetTile(pos, ruleTile);
                    Debug.Log("generating tiles...");
                }
            }
        }
    }
    private void DigOutTile()
    {
        Vector3Int xornPos = Vector3Int.RoundToInt(xorn.transform.position);
        tileMap.SetTile(xornPos, null);
        removedTiles.Add(xornPos);
    }
}
