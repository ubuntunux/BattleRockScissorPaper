using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBarCS : MonoBehaviour
{
    public GameObject Bar;
    public GameObject PlayerID;

    float _goalRatio = 1.0f;

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
        _goalRatio = ratio;
    }

    // Update is called once per frame
    void Update()
    {
        float ratio = Bar.transform.localScale.x;
        if(_goalRatio != ratio)
        {
            if(_goalRatio < ratio)
            {
                ratio -= Time.deltaTime * 2.0f;
            }

            if(ratio < _goalRatio)
            {
                ratio = _goalRatio;
            }

            Bar.transform.localScale = new Vector3(ratio, 1.0f, 1.0f);
        }
    }
}
