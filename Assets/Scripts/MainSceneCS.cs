using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneCS : MonoBehaviour
{
    public GameObject MainSceneManager;
    public GameObject PlayerA;
    public GameObject PlayerB;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {
        ResetMainScene();
    }

    void OnDisable()
    {
    }

    public void ResetMainScene()
    {
        PlayerA.SetActive(false);
        PlayerB.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
