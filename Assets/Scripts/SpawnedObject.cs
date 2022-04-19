using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;

public class SpawnedObject : MonoBehaviour
{
    public enum GemColor { red, blue, green, metal };
    
    //config params
    [SerializeField] float sparkleTimerMax;
    [SerializeField] float sparkleTimerMin;
    [SerializeField] GemColor myColor;
    [SerializeField] public Sprite brokenSprite;
    [SerializeField] ParticleSystem mySparkles;
    [SerializeField] ParticleSystem gemDebris;
    [SerializeField] float minFlickerTime = 0.1f;
    [SerializeField] float maxFlickerTime = 0.3f;
    [SerializeField] List<AudioClip> gemClips;


    //cached refs
    public int myIndex;
    DirtBlocks dirtBlocks;
    Xorn xorn;
    float sparkleTimer;
    public SpriteMask myMask;
    ObjectGenerator objectGenerator;
    Stomachs stomachs;
    SpriteRenderer myRenderer;
    Light2D myLight;
    float myDefaultBrightness;
    float flickerTimer = 0f;
    bool sparkling = true;
    SessionManager sessionManager;
    AudioSource myAudioSource;
    AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        xorn = FindObjectOfType<Xorn>();
        dirtBlocks = FindObjectOfType<DirtBlocks>();
        audioManager = FindObjectOfType<AudioManager>();
        myMask = GetComponentInChildren<SpriteMask>();
        objectGenerator = FindObjectOfType<ObjectGenerator>();
        stomachs = FindObjectOfType<Stomachs>();
        myRenderer = GetComponent<SpriteRenderer>();
        myAudioSource = GetComponent<AudioSource>();
        myLight = GetComponentInChildren<Light2D>();
        myDefaultBrightness = myLight.intensity;
        myLight.intensity = 0;
        sparkleTimer = UnityEngine.Random.Range(sparkleTimerMin, sparkleTimerMax * 2);
        sessionManager = FindObjectOfType<SessionManager>();
        if (objectGenerator.destroyedGem[myIndex])
        {
            myMask.sprite = brokenSprite;
            myLight.enabled = false;
            sparkling = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UncoverGem();
        Sparkle();
        GetEaten();
        Flicker();
        myMask.enabled = objectGenerator.maskActive[myIndex];
    }

    private void Flicker()
    {
        if(stomachs.greenStomach.value > 0)
        {
            if(flickerTimer <= 0)
            {
                float newIntensity = UnityEngine.Random.Range(myDefaultBrightness/2, myDefaultBrightness);
                myLight.intensity = newIntensity;
                float randomTime = UnityEngine.Random.Range(minFlickerTime, maxFlickerTime);
                flickerTimer = randomTime;
            }
            else
            {
                flickerTimer -= Time.deltaTime;
            }
        }
        else
        {
            myLight.intensity = 0;
        }
    }

    private void Sparkle()
    {
        if(!sparkling) { return; }
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
                myLight.enabled = false;
                sparkling = false;
                sessionManager.gemCount++;
                int randomClipIndex = UnityEngine.Random.Range(0, gemClips.Count);
                AudioSource.PlayClipAtPoint(gemClips[randomClipIndex], xorn.transform.position, audioManager.masterVolume);
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
