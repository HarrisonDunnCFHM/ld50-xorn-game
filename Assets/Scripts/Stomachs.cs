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
    public float blueDecayRate;
    [SerializeField] public Slider greenStomach;
    [SerializeField] int greenMaxCapacity;
    [SerializeField] float greenIngestRate;
    public float greenDecayRate;
    [SerializeField] public Slider redStomach;
    [SerializeField] int redMaxCapacity;
    [SerializeField] float redIngestRate;
    public float redDecayRate;

    //cached references
    Lava lava;
    Xorn xorn;

    // Start is called before the first frame update
    void Start()
    {
        blueStomach.maxValue = blueMaxCapacity;
        greenStomach.maxValue = greenMaxCapacity;
        redStomach.maxValue = redMaxCapacity;
        lava = FindObjectOfType<Lava>();
        xorn = FindObjectOfType<Xorn>();
    }

    // Update is called once per frame
    void Update()
    {
        StomachDecay();
        Lava();
    }

    private void StomachDecay()
    {
        blueStomach.value -= Time.deltaTime * greenDecayRate;
        greenStomach.value -= Time.deltaTime * greenDecayRate;
        redStomach.value -= Time.deltaTime * greenDecayRate;
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
            redStomach.value -= redDecayRate;
        }
        else if (greenStomach.value > 0 || blueStomach.value > 0)
        {
            greenStomach.value = 0;
            blueStomach.value = 0;
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void Lava()
    {
        Vector3Int roundedXornPos = Vector3Int.RoundToInt(xorn.transform.position);
        {
            if (lava.tileMap.HasTile(roundedXornPos))
            {
                if (redStomach.value > 0 || greenStomach.value > 0 || blueStomach.value > 0) 
                { 
                    redStomach.value = 0;
                    greenStomach.value = 0;
                    blueStomach.value = 0;
                    lava.tileMap.SetTile(roundedXornPos, null);
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }
    }
}
