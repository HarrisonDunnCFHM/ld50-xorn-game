using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnedObject : MonoBehaviour
{
    //config params
    [SerializeField] float sparkleTimerMax;
    [SerializeField] float sparkleTimerMin;

    //cached refs
    public int myIndex;
    DirtBlocks dirtBlocks;
    Xorn xorn;
    ParticleSystem mySparkles;
    float sparkleTimer = 0f;
    SpriteMask myMask;
    
    // Start is called before the first frame update
    void Start()
    {
        xorn = FindObjectOfType<Xorn>();
        dirtBlocks = FindObjectOfType<DirtBlocks>();
        mySparkles = GetComponentInChildren<ParticleSystem>();
        myMask = GetComponentInChildren<SpriteMask>();
        myMask.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        UncoverGem();
        Sparkle();
    }

    private void Sparkle()
    {
        if(sparkleTimer <= 0f)
        {
            mySparkles.Play();
            float newTimer = UnityEngine.Random.Range(sparkleTimerMin, sparkleTimerMax);
            sparkleTimer = newTimer;
        }
        else
        {
            sparkleTimer -= Time.deltaTime;
        }
    }

    private void UncoverGem()
    {
        float distToXorn = Vector3.Magnitude(transform.position - xorn.transform.position);
        if (distToXorn <= 1.1)
        {
            dirtBlocks.ChangeToGemTile(transform.position);
            myMask.enabled = true;
        }
    }
}
