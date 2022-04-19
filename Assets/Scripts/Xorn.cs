using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Xorn : MonoBehaviour
{

    //configuration parameters
    [SerializeField] float moveSpeed;
    [SerializeField] float defaultMoveTimer;
    [SerializeField] float fedMoveTimer;
    [Tooltip("Distance from destination until snap-to-grid occurs.")]
    [SerializeField] float snapDistance;
    [Tooltip("Amount of time xorn will wait at each space before moving while holding a direction.")]
    [SerializeField] public float deathDistance = 0.9f;

    //cached references
    public bool moving = false; //check if xorn is moving
    ObjectGenerator objectGenerator;
    Animator myAnimator;
    SpriteMask myMask;
    SpriteRenderer myRenderer;
    Transform myBody;
    Stomachs stomachs;
    float moveTimer;
    public float destinationDistance;
    // Start is called before the first frame update
    void Start()
    {
        objectGenerator = FindObjectOfType<ObjectGenerator>();
        myAnimator = GetComponentInChildren<Animator>();
        myMask = GetComponentInChildren<SpriteMask>();
        myRenderer = GetComponentInChildren<SpriteRenderer>();
        myBody = GetComponentInChildren<Transform>();
        stomachs = FindObjectOfType<Stomachs>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckMoveTimer();
        ProcessMoveInput();
        ControlAnimation();
    }

    private void CheckMoveTimer()
    {
        if(stomachs.blueStomach.value > 0)
        {
            moveTimer = fedMoveTimer;
        }
        else
        {
            moveTimer = defaultMoveTimer;
        }
    }

    private void SnapToGrid()
    {
        if (moving) { return; }
        int newX = Mathf.RoundToInt(transform.position.x);
        int newY = Mathf.RoundToInt(transform.position.y);
        transform.position = new Vector3(newX, newY, transform.position.z);
    }

    private void ProcessMoveInput()
    {
        if (Input.GetButton("up"))
        {
            StartCoroutine(MoveDirection(Vector3.up, transform.position + Vector3.up));
            myBody.localEulerAngles = new Vector3(0, 0, 180);
        }
        if (Input.GetButton("down"))
        {
            StartCoroutine(MoveDirection(Vector3.down, transform.position + Vector3.down));
            myBody.localEulerAngles = new Vector3(0, 0, 0);
        }
        if (Input.GetButton("left"))
        {
            StartCoroutine(MoveDirection(Vector3.left, transform.position + Vector3.left));
            myBody.localEulerAngles =  new Vector3(0, 0, 270);
        }
        if (Input.GetButton("right"))
        {
            StartCoroutine(MoveDirection(Vector3.right, transform.position + Vector3.right));
            myBody.localEulerAngles = new Vector3(0, 0, 90);
        }
    }

    private IEnumerator MoveDirection(Vector3 direction, Vector3 destination)
    {
        if (moving) { yield break; }
        moving = true;
        destinationDistance = Vector3.Distance(transform.position, destination);
        while (destinationDistance > snapDistance && destinationDistance < 1.1f)
        {
            destinationDistance = Vector3.Distance(transform.position, destination);
            transform.position += (direction * moveSpeed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(moveTimer);
        stomachs.blueStomach.value -= stomachs.blueDecayRate;
        moving = false;
        SnapToGrid();
    }

    public void ControlAnimation()
    {
        myMask.sprite = myRenderer.sprite;
        if(moving)
        {
            myAnimator.StopPlayback();
        }
        else
        {
            myAnimator.StartPlayback();
        }
    }


}
