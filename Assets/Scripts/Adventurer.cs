using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Adventurer : MonoBehaviour
{
    //config params
    [SerializeField] float normalSpeed = 5f;
    [SerializeField] float pursuitSpeed = 10f;
    [SerializeField] float normalMoveCooldown = 0.5f;
    [SerializeField] float pursuitMoveCooldown = 0.1f;
    [SerializeField] float snapDistance = 0.1f;

    //cached references
    Xorn xorn;
    Stomachs stomachs;
    DirtBlocks dirtBlocks;
    AdventurerSpawner mySpawner;
    Animator myAnimator;
    public List<Vector3Int> tilesVisited = new List<Vector3Int>();
    public List<int> tilesVisitedCount = new List<int>();
    public bool moving = false;
    public Vector3Int currentDirection;
    public List<Vector3Int> _directions = new List<Vector3Int>();
    public List<Vector3Int> possibleDirections = new List<Vector3Int>();
    public int myIndex;
    bool xornFound = false;
    float moveSpeed;
    float moveCooldown;
    SessionManager sessionManager;

    // Start is called before the first frame update
    void Start()
    {
        dirtBlocks = FindObjectOfType<DirtBlocks>();
        PickStartingDirection();
        xorn = FindObjectOfType<Xorn>();
        stomachs = FindObjectOfType<Stomachs>();
        mySpawner = FindObjectOfType<AdventurerSpawner>();
        myAnimator = GetComponent<Animator>();
        sessionManager = FindObjectOfType<SessionManager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMoveCooldown();
        SearchForXorn();
        PickNewDirection();
        PursuitMode();
        StartCoroutine(MoveInDirection());
        AttackXorn();
        Animate();
    }

    private void UpdateMoveCooldown()
    {
        moveCooldown = normalMoveCooldown - Mathf.Pow(1.01f,sessionManager.gemCount);
        if(moveCooldown < 0) { moveCooldown = 0; }
    }

    private void Animate()
    {
        if(currentDirection == Vector3Int.up)
        {
            transform.localEulerAngles = new Vector3(0, 0, 180);
        }
        else if (currentDirection == Vector3Int.right)
        {
            transform.localEulerAngles = new Vector3(0, 0, 90);
        }
        else if (currentDirection == Vector3Int.down)
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if (currentDirection == Vector3Int.left)
        {
            transform.localEulerAngles = new Vector3(0, 0, 270);
        }
    }

    private void SearchForXorn()
    {
        if (moving) { return; }
        int xornX = Mathf.RoundToInt(xorn.transform.position.x);
        int xornY = Mathf.RoundToInt(xorn.transform.position.y);
        int myX = Mathf.RoundToInt(transform.position.x);
        int myY = Mathf.RoundToInt(transform.position.y);
        if (xornX == myX) //check if in the same column
        {
            if(xornY > myY) //check if xorn is above me
            {
                for (int i = myY; myY < xornY; i++)
                {
                    Vector3Int posToCheck = new Vector3Int(myX, i, Mathf.RoundToInt(transform.position.z));
                    if(!dirtBlocks.removedTiles.Contains(posToCheck))
                    {
                        break;
                    }
                    currentDirection = Vector3Int.up;
                    xornFound = true;
                }
            }
            else if (xornY < myY) //check if xorn is below me
            {
                for (int i = myY; myY > xornY; i--)
                {
                    Vector3Int posToCheck = new Vector3Int(myX, i, Mathf.RoundToInt(transform.position.z));
                    if (!dirtBlocks.removedTiles.Contains(posToCheck))
                    {
                        break;
                    }
                    currentDirection = Vector3Int.down;
                    xornFound = true;
                }
            }
        }
        else if (xornY == myY) //check if in the same row
        {
            if (xornX > myX) //check if xorn is right of me
            {
                for (int i = myX; myX < xornX; i++)
                {
                    Vector3Int posToCheck = new Vector3Int(i, myY, Mathf.RoundToInt(transform.position.z));
                    if (dirtBlocks.GetComponentInChildren<Tilemap>().HasTile(posToCheck))
                    {
                        break;
                    }
                    currentDirection = Vector3Int.right;
                    xornFound = true;
                }
            }
            else if (xornX < myX) //check if xorn is left of me
            {
                for (int i = myX; myX > xornX; i--)
                {
                    Vector3Int posToCheck = new Vector3Int(i, myY, Mathf.RoundToInt(transform.position.z));
                    if (!dirtBlocks.removedTiles.Contains(posToCheck))
                    {
                        break;
                    }
                    currentDirection = Vector3Int.left;
                    xornFound = true;
                }
            }
        }
        else
        {
            xornFound = false;
        }
    }


    private void PursuitMode()
    {
        if (moving) { return; }
        if(xornFound)
        {
            moveSpeed = pursuitSpeed;
            moveCooldown = pursuitMoveCooldown;
        }
        else
        {
            moveSpeed = normalSpeed;
            moveCooldown = normalMoveCooldown;
        }
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
        if(xornFound)
        {
            foreach(Vector3Int possibleDirection in _directions)
            {
                if (possibleDirection == currentDirection && !dirtBlocks.removedTiles.Contains(possibleDirection + Vector3Int.RoundToInt(transform.position)))
                {
                    xornFound = false;
                    break;
                }
                else if (currentDirection != possibleDirection)
                {
                    possibleDirections.Remove(possibleDirection);
                }
            }
        }
        if (!xornFound)
        {
            possibleDirections = new List<Vector3Int>() { Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left };
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
        }
        if (possibleDirections.Count == 0)
        {
            currentDirection = -currentDirection;
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
        while (destinationDistance > snapDistance && destinationDistance < 1.1f)
        {
            destinationDistance = Vector3.Distance(transform.position, destination);
            transform.position += ((Vector3)currentDirection * moveSpeed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(moveCooldown);
        moving = false;
        transform.position = Vector3Int.RoundToInt(transform.position);
    }

    private void AttackXorn()
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
