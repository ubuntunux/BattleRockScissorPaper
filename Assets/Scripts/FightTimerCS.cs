using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightTimerCS : MonoBehaviour
{
    public GameObject Bar;
    public GameObject GloveRed;
    public GameObject GloveBlue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Reset()
    {
        setBar(1.0f);
    }

    public void setBar(float ratio)
    {
        Bar.transform.localScale = new Vector3(ratio, 1.0f, 1.0f);

        if(0.0f == ratio)
        {
            GloveRed.SetActive(false);
        }
        else
        {
            GloveRed.SetActive(true);
        }
        GloveRed.transform.localPosition = new Vector3(Mathf.Lerp(-25.0f, -125.0f, ratio), 1.0f, 1.0f);

        if(0.0f == ratio)
        {
            GloveBlue.SetActive(false);
        }
        else
        {
            GloveBlue.SetActive(true);
        }
        GloveBlue.transform.localPosition = new Vector3(Mathf.Lerp(25.0f, 125.0f, ratio), 1.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
