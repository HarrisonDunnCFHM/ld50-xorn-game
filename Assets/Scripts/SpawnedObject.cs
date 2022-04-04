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
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        myNumber.text = myIndex.ToString();
    }
}
