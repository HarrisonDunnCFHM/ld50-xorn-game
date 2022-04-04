using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RepeatingBackground : MonoBehaviour
{
    //config params
    [SerializeField] GameObject backgroundTile;
    [SerializeField] Camera mainCamera;


    //cached references
    Xorn xorn;
    public List<GameObject> spawnedTiles = new List<GameObject>(9);
    float tileWidth;
    float tileHeight;
    float tileDiagonal;
    GameObject activeTile;
    Vector2 screenBounds;
    List<Vector3> directions = new List<Vector3>();
    Color currentBGColor;
    bool tilesCreated = false;
    
    // Start is called before the first frame update
    void Start()
    {
        xorn = FindObjectOfType<Xorn>();
        tileWidth = backgroundTile.GetComponent<SpriteRenderer>().bounds.size.x;
        tileHeight = backgroundTile.GetComponent<SpriteRenderer>().bounds.size.y;
        tileDiagonal = Mathf.Sqrt(Mathf.Pow(tileWidth, 2) + Mathf.Pow(tileHeight, 2));
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        directions.Insert(0,Vector3.zero);
        directions.Insert(1,Vector3.up);
        directions.Insert(2,Vector3.Normalize(Vector3.up * tileHeight + Vector3.right * tileWidth));
        directions.Insert(3,Vector3.right);
        directions.Insert(4,Vector3.Normalize(Vector3.down * tileHeight + Vector3.right * tileWidth));
        directions.Insert(5,Vector3.down);
        directions.Insert(6,Vector3.Normalize(Vector3.down * tileHeight + Vector3.left * tileWidth));
        directions.Insert(7,Vector3.left);
        directions.Insert(8,Vector3.Normalize(Vector3.up * tileHeight + Vector3.left * tileWidth));
        currentBGColor = backgroundTile.GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        //UnloadTiles();
        LoadTiles();
    }

    private void LoadTiles()
    {
        if(!tilesCreated) { InstantiateTiles(); }
        float xornX = xorn.transform.position.x;
        float xornY = xorn.transform.position.y;
        float tileX = activeTile.transform.position.x;
        float tileY = activeTile.transform.position.y;
        if(xornX > tileX + tileWidth/2 || xornX < tileX - tileWidth/2 || xornY > tileY + tileHeight/2 || xornY < tileY - tileHeight/2 )
        {
            tilesCreated = false;
            foreach(GameObject tile in spawnedTiles)
            {
                float spawnedTileX = tile.transform.position.x;
                float spawnedTileY = tile.transform.position.y;
                if (xornX < spawnedTileX + tileWidth / 2 
                    && xornX > spawnedTileX - tileWidth / 2 
                    && xornY < spawnedTileY + tileHeight / 2 
                    && xornY > spawnedTileY - tileHeight / 2)
                {
                    activeTile = tile;
                    Debug.Log("new active tile is " + tile.name);
                    break;
                }

            }
        }
    }

    private void InstantiateTiles()
    {
        foreach(Vector3 direction in directions)
        {
            GameObject newTile;
            if (direction == Vector3.zero && activeTile == null)
            {
                newTile = Instantiate(backgroundTile, xorn.transform.position, Quaternion.identity);
                spawnedTiles.Insert(0, newTile);
                newTile.name += spawnedTiles.IndexOf(newTile);
                newTile.transform.SetParent(gameObject.transform);
                activeTile = newTile;
            }
            else if (direction == Vector3.zero)
            {
                newTile = Instantiate(backgroundTile, activeTile.transform.position, Quaternion.identity);
                Destroy(spawnedTiles[0]);
                Destroy(activeTile);
                if (spawnedTiles.ElementAtOrDefault(0) != null)
                {
                    Destroy(spawnedTiles[0]);
                }
                else
                {
                    spawnedTiles.Insert(0, newTile);
                }
                spawnedTiles[0] = newTile; 
                newTile.name += spawnedTiles.IndexOf(newTile);
                newTile.transform.SetParent(gameObject.transform);
                activeTile = newTile;
            }
            else if (direction == Vector3.up || direction == Vector3.down)
            {
                newTile = Instantiate(backgroundTile, activeTile.transform.position + (direction * tileHeight), Quaternion.identity);
                if (direction == Vector3.up)
                {
                    if (spawnedTiles.ElementAtOrDefault(1) != null)
                    {
                        Destroy(spawnedTiles[1]);
                    }
                    else
                    {
                        spawnedTiles.Insert(1, newTile);
                    }
                    spawnedTiles[1] = newTile;
                }
                else if (direction == Vector3.down)
                {
                    if (spawnedTiles.ElementAtOrDefault(5) != null)
                    {
                        Destroy(spawnedTiles[5]);
                    }
                    else
                    {
                        spawnedTiles.Insert(5, newTile);
                    }
                    spawnedTiles[5] = newTile;
                }
                newTile.name += spawnedTiles.IndexOf(newTile);
                newTile.transform.SetParent(gameObject.transform);
            }
            else if (direction == Vector3.left || direction == Vector3.right)
            {
                newTile = Instantiate(backgroundTile, activeTile.transform.position + (direction * tileWidth), Quaternion.identity);
                if (direction == Vector3.left)
                {
                    if (spawnedTiles.ElementAtOrDefault(7) != null)
                    {
                        Destroy(spawnedTiles[7]);
                    }
                    else
                    {
                        spawnedTiles.Insert(7, newTile);
                    }
                    spawnedTiles[7] = newTile;
                }
                else if (direction == Vector3.right)
                {
                    if (spawnedTiles.ElementAtOrDefault(3) != null)
                    {
                        Destroy(spawnedTiles[3]);
                    }
                    else
                    {
                        spawnedTiles.Insert(3, newTile);
                    }
                    spawnedTiles[3] = newTile;
                }
                newTile.name += spawnedTiles.IndexOf(newTile);
                newTile.transform.SetParent(gameObject.transform);
            }
            else
            {
                newTile = Instantiate(backgroundTile, activeTile.transform.position + (direction * tileDiagonal), Quaternion.identity);
                if (direction == directions[2])
                {
                    if (spawnedTiles.ElementAtOrDefault(2) != null)
                    {
                        Destroy(spawnedTiles[2]);
                    }
                    else
                    {
                        spawnedTiles.Insert(2, newTile);
                    }
                    spawnedTiles[2] = newTile;
                }
                else if (direction == directions[4])
                {
                    if (spawnedTiles.ElementAtOrDefault(4) != null)
                    {
                        Destroy(spawnedTiles[4]);
                    }
                    else
                    {
                        spawnedTiles.Insert(4, newTile);
                    }
                    spawnedTiles[4] = newTile;
                }
                else if (direction == directions[6])
                {
                    if (spawnedTiles.ElementAtOrDefault(6) != null)
                    {
                        Destroy(spawnedTiles[6]);
                    }
                    else
                    {
                        spawnedTiles.Insert(6, newTile);
                    }
                    spawnedTiles[6] = newTile;
                }
                else if (direction == directions[8])
                {
                    if (spawnedTiles.ElementAtOrDefault(8) != null)
                    {
                        Debug.Log("Destroying "  + spawnedTiles[8].name);
                        Destroy(spawnedTiles[8]);
                    }
                    else
                    {
                        spawnedTiles.Insert(8, newTile);
                    }
                    spawnedTiles[8] = newTile;
                }
                newTile.name += spawnedTiles.IndexOf(newTile);
                newTile.transform.SetParent(gameObject.transform);
            }
        }
        tilesCreated = true;
    }


    private void InstantiateTile()
    {
        GameObject newTile;
        if (activeTile == null) //if there are no tiles yet, generate one under the player then stop
        {
            newTile = Instantiate(backgroundTile, xorn.transform.position, Quaternion.identity);
            spawnedTiles.Add(newTile);
            newTile.name += spawnedTiles.IndexOf(newTile);
            newTile.transform.SetParent(gameObject.transform);
            return;
        }
        bool placeablePosFound = false; //next, look for a valid location to place a new tile
        Vector3 targetPos = Vector3.zero;
        foreach (Vector3 direction in directions) //look at a direction
        {
            foreach (GameObject tile in spawnedTiles) //then check if there's a tile already there
            {
                Vector3 tilePos = tile.transform.position;
                if (direction == Vector3.up || direction == Vector3.down)
                {
                    Vector3 yPos = activeTile.transform.position + direction * tileHeight;
                    if (tilePos == yPos)
                    {
                        Debug.Log("tile already exists");
                        break;
                    }
                    placeablePosFound = true;
                    Debug.Log("free space!");
                    break; //once a blank spot is found, break this foreach
                }
                else if (direction == Vector3.left || direction == Vector3.right)
                {
                    Vector3 xPos = activeTile.transform.position + direction * tileWidth;
                    if (tilePos == xPos)
                    {
                        Debug.Log("tile already exists");
                        break;
                    }
                    placeablePosFound = true;
                    break; //once a blank spot is found, break this foreach
                }
                else if (direction == directions[4] || direction == directions[5] || direction == directions[6] || direction == directions[7])
                {
                    Vector3 diaPos = activeTile.transform.position + direction * tileDiagonal;
                    if(tilePos == diaPos)
                    {
                        Debug.Log("tile already exists");
                        break;
                    }
                    placeablePosFound = true;
                    break; //once a blank spot is found, break this foreach
                }
                else if ( tile.transform.position == activeTile.transform.position)
                {
                    Debug.Log("tile already exists");
                    break;
                }
            }
            if (placeablePosFound)
            {
                targetPos = direction;
                break;
            }
        }
        if (!placeablePosFound) { return; } //then, if valid place was found, place it based on direction
        if (targetPos == Vector3.left || targetPos == Vector3.right)
        {
            newTile = Instantiate(backgroundTile, activeTile.transform.position + (targetPos * tileWidth), Quaternion.identity);
            spawnedTiles.Add(newTile);
            newTile.name += spawnedTiles.IndexOf(newTile);
            newTile.transform.SetParent(gameObject.transform);
        }
        else if (targetPos == Vector3.up || targetPos == Vector3.down)
        {
            newTile = Instantiate(backgroundTile, activeTile.transform.position + (targetPos * tileHeight), Quaternion.identity);
            spawnedTiles.Add(newTile);
            newTile.name += spawnedTiles.IndexOf(newTile);
            newTile.transform.SetParent(gameObject.transform);
        }
        else
        {
            newTile = Instantiate(backgroundTile, activeTile.transform.position + (targetPos * tileDiagonal), Quaternion.identity);
            spawnedTiles.Add(newTile);
            newTile.name += spawnedTiles.IndexOf(newTile);
            newTile.transform.SetParent(gameObject.transform);
        }
    }

    private void UnloadTiles()
    {
        if(activeTile == null) { return; }
        Vector3 activeTilePos = activeTile.transform.position;
        foreach(GameObject tile in spawnedTiles)
        {
            Vector3 tilePos = tile.transform.position;
            if (activeTile == tile) { break; }
            foreach (Vector3 direction in directions)
            {
                if(tilePos == activeTilePos + (direction * tileHeight) 
                    || tilePos == activeTilePos + (direction * tileWidth) 
                    || tilePos == activeTilePos + (direction * tileDiagonal))
                {
                    break;
                }
                else
                {
                    spawnedTiles.Remove(tile);
                    Destroy(tile);
                    return;
                }
            }
            /*
            Vector3 upTile = activeTile.transform.position + Vector3.up * tileHeight;
            Vector3 downTile = activeTile.transform.position + Vector3.down * tileHeight;
            Vector3 leftTile = activeTile.transform.position + Vector3.left * tileWidth;
            Vector3 rightTile = activeTile.transform.position + Vector3.right * tileWidth;
            Vector3 activePos = activeTile.transform.position; 
            Vector3 myPos = tile.transform.position;
            if (myPos != upTile && myPos != downTile && myPos != leftTile && myPos != rightTile && myPos != activePos)
            {
                spawnedTiles.Remove(tile);
                Debug.Log("Spawned tile list count is " + spawnedTiles.Count);
                Destroy(tile);
                return;
            }*/
        }
    }
}
