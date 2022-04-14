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
    [SerializeField] ParticleSystem debris;
    [SerializeField] List<AudioClip> myAudioClips;

    //cached references 
    Xorn xorn;
    int maxSpawnX;
    int maxSpawnY;
    public List<Vector3Int> removedTiles = new List<Vector3Int>();
    AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        xorn = FindObjectOfType<Xorn>();
        audioManager = FindObjectOfType<AudioManager>();
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
                }
            }
        }
    }

    private void DigOutTile()
    {
        Vector3Int xornPos = Vector3Int.RoundToInt(xorn.transform.position);
        if(!removedTiles.Contains(xornPos))
        {
            tileMap.SetTile(xornPos, null);
            removedTiles.Add(xornPos);
            debris.gameObject.transform.position = xornPos;
            debris.Play();
            PlayDirtNoise();
        }
    }

    private void PlayDirtNoise()
    {
        int randomIndex = UnityEngine.Random.Range(0,myAudioClips.Count);
        AudioSource.PlayClipAtPoint(myAudioClips[randomIndex], xorn.transform.position, audioManager.sfxVol);
    }

    public void ChangeToGemTile(Vector3 gemLocation)
    {
        Vector3Int gemPos = Vector3Int.RoundToInt(gemLocation);
        if (!removedTiles.Contains(gemPos))
        {
            tileMap.SetTile(gemPos, null);
            debris.gameObject.transform.position = gemPos;
            debris.Play();
            removedTiles.Add(gemPos);
        }
    }
}
