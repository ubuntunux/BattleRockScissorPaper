using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersusPortraitCS : MonoBehaviour
{
    public GameObject Text_Name;
    public GameObject Image_Portrait;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetPortrait(string name, Sprite sprite)
    {
        Text_Name.GetComponent<Text>().text = name;
        Image_Portrait.GetComponent<Image>().sprite = sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
