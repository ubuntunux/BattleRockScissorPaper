using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingSceneManagerCS : MonoBehaviour
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
        ResetTrainingScene();
    }

    void OnDisable()
    {
    }

    void ResetTrainingScene()
    {
        PlayerA.SetActive(true);
        PlayerA.GetComponent<PlayerCS>().SetStateIdle();
        
        PlayerB.SetActive(false);
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
