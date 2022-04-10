using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Stomachs : MonoBehaviour
{
    //config param
    [SerializeField] public Slider blueStomach;
    [SerializeField] int blueMaxCapacity;
    [SerializeField] float blueIngestRate;
    [SerializeField] public Slider greenStomach;
    [SerializeField] int greenMaxCapacity;
    [SerializeField] float greenIngestRate;
    [SerializeField] public Slider redStomach;
    [SerializeField] int redMaxCapacity;
    [SerializeField] float redIngestRate;
    [SerializeField] float decayRate;

    // Start is called before the first frame update
    void Start()
    {
        blueStomach.maxValue = blueMaxCapacity;
        greenStomach.maxValue = greenMaxCapacity;
        redStomach.maxValue = redMaxCapacity;
    }

    // Update is called once per frame
    void Update()
    {
        StomachDecay();
    }

    private void StomachDecay()
    {
        blueStomach.value -= Time.deltaTime * decayRate;
        greenStomach.value -= Time.deltaTime * decayRate;
        redStomach.value -= Time.deltaTime * decayRate;
    }

    public void IngestGem(SpawnedObject.GemColor gemColor)
    {
        switch(gemColor)
        {
            case SpawnedObject.GemColor.blue:
                blueStomach.value += blueIngestRate;
                break;
            case SpawnedObject.GemColor.green:
                greenStomach.value += greenIngestRate;
                break;
            case SpawnedObject.GemColor.red:
                redStomach.value += redIngestRate;
                break;
            default:
                Debug.Log("Invalid color provided - no stomach for this color!");
                break;
        }
    }

    public void TakeHit()
    {
        if(redStomach.value > 0)
        {
            redStomach.value = 0;
        }
        else if (greenStomach.value > 0)
        {
            greenStomach.value = 0;
        }
        else if (blueStomach.value > 0)
        {
            blueStomach.value = 0;
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
