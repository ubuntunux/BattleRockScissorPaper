using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneManagerCS : MonoBehaviour
{
    public GameObject Btn_Online;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Btn_Challenge_OnClock()
    {
        SceneManager.LoadScene("ChallengeScene");
    }

    public void Btn_Online_OnClock()
    {
    }
}
