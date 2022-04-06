using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnedObject : MonoBehaviour
{
    public enum GemColor { red, blue, green };
    
    //config params
    [SerializeField] float sparkleTimerMax;
    [SerializeField] float sparkleTimerMin;
    [SerializeField] GemColor myColor;
    [SerializeField] public Sprite brokenSprite;
    [SerializeField] ParticleSystem mySparkles;
    [SerializeField] ParticleSystem gemDebris;


    //cached refs
    public int myIndex;
    DirtBlocks dirtBlocks;
    Xorn xorn;
    float sparkleTimer = 0f;
    public SpriteMask myMask;
    ObjectGenerator objectGenerator;
    Stomachs stomachs;
    SpriteRenderer myRenderer;

    // Start is called before the first frame update
    void Start()
    {
        xorn = FindObjectOfType<Xorn>();
        dirtBlocks = FindObjectOfType<DirtBlocks>();
        myMask = GetComponentInChildren<SpriteMask>();
        objectGenerator = FindObjectOfType<ObjectGenerator>();
        stomachs = FindObjectOfType<Stomachs>();
        myRenderer = GetComponent<SpriteRenderer>();
        if(objectGenerator.destroyedGem[myIndex])
        {
            myMask.sprite = brokenSprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UncoverGem();
        Sparkle();
        GetEaten();
        myMask.enabled = objectGenerator.maskActive[myIndex];
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

    private void GetEaten()
    {
        float distToXorn = Vector3.Magnitude(transform.position - xorn.transform.position);
        if (distToXorn < 0.9f)
        {
            if (!objectGenerator.destroyedGem[myIndex])
            {
                objectGenerator.destroyedGem[myIndex] = true;
                stomachs.IngestGem(myColor);
                myRenderer.sprite = brokenSprite;
                myMask.sprite = brokenSprite;
                gemDebris.Play();
            }
        }
    }

    private void UncoverGem()
    {
        float distToXorn = Vector3.Magnitude(transform.position - xorn.transform.position);
        if (distToXorn <= 1.1)
        {
            dirtBlocks.ChangeToGemTile(transform.position);
            objectGenerator.maskActive[myIndex] = true;
            myMask.enabled = true;
        }
    }
}
