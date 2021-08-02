using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersusCS : MonoBehaviour
{
    public GameObject PlayerA;
    public GameObject PlayerB;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void SetPortraitPlayerA(string name, Sprite sprite)
    {
        PlayerA.GetComponent<VersusPortraitCS>().SetPortrait(name, sprite);
    }

    public void SetPortraitPlayerB(string name, Sprite sprite)
    {
        PlayerB.GetComponent<VersusPortraitCS>().SetPortrait(name, sprite);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
