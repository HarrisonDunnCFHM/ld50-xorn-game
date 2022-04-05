using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnedObject : MonoBehaviour
{
    //config params
    [SerializeField] Text myNumber;


    //cached refs
    public int myIndex;
    DirtBlocks dirtBlocks;
    Xorn xorn;
    
    // Start is called before the first frame update
    void Start()
    {
        xorn = FindObjectOfType<Xorn>();
        dirtBlocks = FindObjectOfType<DirtBlocks>();
    }

    // Update is called once per frame
    void Update()
    {
        myNumber.text = myIndex.ToString();
        UncoverGem();
    }

    private void UncoverGem()
    {
        float distToXorn = Vector3.Magnitude(transform.position - xorn.transform.position);
        if (distToXorn <= 1.1)
        {
            dirtBlocks.ChangeToGemTile(transform.position);
        }
    }
}
