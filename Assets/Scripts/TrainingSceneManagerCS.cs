using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingSceneManagerCS : MonoBehaviour
{
    public GameObject MainSceneManager;

    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    void Reset()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(MainSceneManager.GetComponent<MainSceneManagerCS>().GetActivateSceneType() == GameSceneType.TrainingScene)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                MainSceneManager.GetComponent<MainSceneManagerCS>().SetActivateScene(GameSceneType.MainScene);
            }
        }
    }
}
