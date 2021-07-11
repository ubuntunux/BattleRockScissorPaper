using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTimerCS : MonoBehaviour
{
    public GameObject Bar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Reset()
    {
        setTimer(1.0f);
    }

    public void setTimer(float ratio)
    {
        Bar.transform.localScale = new Vector3(ratio, 1.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
