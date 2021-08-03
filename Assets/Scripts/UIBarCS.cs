using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBarCS : MonoBehaviour
{
    public GameObject Bar;
    public GameObject PlayerID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Reset(string playerID)
    {
        setBar(1.0f);
        setPlayerID(playerID);
    }

    public void setPlayerID(string playerID)
    {
        PlayerID.GetComponent<Text>().text = playerID;
    }

    public void setBar(float ratio)
    {
        Bar.transform.localScale = new Vector3(ratio, 1.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
