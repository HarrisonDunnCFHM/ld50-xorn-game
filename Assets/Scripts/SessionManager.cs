using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SessionManager : MonoBehaviour
{
    //config params
    [SerializeField] Text gemCountText;

    //cached references
    public int gemCount = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gemCountText.text = "Gems Eaten: " + gemCount;
    }
}
