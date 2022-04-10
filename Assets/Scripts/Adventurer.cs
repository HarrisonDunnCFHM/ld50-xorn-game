using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventurer : MonoBehaviour
{
    //config params
    [SerializeField] float moveSpeed = 5;
    [SerializeField] float moveCooldown = 0.5f;
    [SerializeField] float snapDistance = 0.1f;

    //cached references
    Xorn xorn;
    Stomachs stomachs;
    DirtBlocks dirtBlocks;
    AdventurerSpawner mySpawner;
    public List<Vector3Int> tilesVisited = new List<Vector3Int>();
    public List<int> tilesVisitedCount = new List<int>();
    public bool moving = false;
    public Vector3Int currentDirection;
    public List<Vector3Int> _directions = new List<Vector3Int>();
    public List<Vector3Int> possibleDirections = new List<Vector3Int>();
    public int myIndex;

    // Start is called before the first frame update
    void Start()
    {
        dirtBlocks = FindObjectOfType<DirtBlocks>();
        PickStartingDirection();
        xorn = FindObjectOfType<Xorn>();
        stomachs = FindObjectOfType<Stomachs>();
        mySpawner = FindObjectOfType<AdventurerSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        PickNewDirection();
        StartCoroutine(MoveInDirection());
        CheckForXorn();
    }

    private void PickStartingDirection()
    {
        _directions = new List<Vector3Int>() { Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left };
        List<Vector3Int> possibleDirections = new List<Vector3Int>() { Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left };
        foreach(Vector3Int possibleDirection in _directions)
        {
            if(!dirtBlocks.removedTiles.Contains(possibleDirection + Vector3Int.RoundToInt(transform.position)))
            {
                possibleDirections.Remove(possibleDirection);
            }
        }
        int randomChoice = UnityEngine.Random.Range(0, possibleDirections.Count);
        currentDirection = possibleDirections[randomChoice];
    }
    private void PickNewDirection()
    {
        if (moving) { return; }
        _directions = new List<Vector3Int>() { Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left };
        List<Vector3Int> possibleDirections = new List<Vector3Int>() { Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left };
        foreach (Vector3Int possibleDirection in _directions)
        {
            if (!dirtBlocks.removedTiles.Contains(possibleDirection + Vector3Int.RoundToInt(transform.position)))
            {
                possibleDirections.Remove(possibleDirection);
            }
            if (possibleDirection == -currentDirection)
            {
                possibleDirections.Remove(possibleDirection);
            }
        }
        if (possibleDirections.Count == 0)
        {
            currentDirection = -currentDirection;
        }
        else if (possibleDirections.Count == 4)
        {
            return;
        }
        else
        {
            int randomChoice = UnityEngine.Random.Range(0, possibleDirections.Count);
            currentDirection = possibleDirections[randomChoice];
        }
    }

    private IEnumerator MoveInDirection()
    {
        if (moving) { yield break; }
        moving = true; 
        float destinationDistance = 1f;
        var destination = transform.position + currentDirection;
        while (destinationDistance > snapDistance)
        {
            destinationDistance = Vector3.Distance(transform.position, destination);
            transform.position += ((Vector3)currentDirection * moveSpeed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(moveCooldown);
        moving = false;
        transform.position = Vector3Int.RoundToInt(transform.position);
    }

    private void CheckForXorn()
    {
        float distToXorn = Vector3.Distance(transform.position, xorn.transform.position);
        if (distToXorn < xorn.deathDistance)
        {
            stomachs.TakeHit();
            mySpawner.currentSpawned--;
            Destroy(gameObject);
        }
    }
}
