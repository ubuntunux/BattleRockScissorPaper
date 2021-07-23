using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameSceneType
{
    None,
    MainScene,
    ChallenegeScene,
    FightScene
}

public class MainSceneManagerCS : MonoBehaviour
{
    public GameObject MainCamera;
    public GameObject MainCanvas;
    public GameObject MainScene;
    public GameObject FightScene;
    public GameObject ChallenegeScene;

    GameSceneType _goalGameSceneType = GameSceneType.None;
    GameSceneType _gameSceneType = GameSceneType.None;
    GameSceneType _gameSceneTypePrev = GameSceneType.None;

    float _fadeInOutTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    void OnEnable()
    {   
        Reset();
    }

    void OnDisable()
    {
    }

    public void Reset()
    {
        _fadeInOutTimer = 0.0f;
        SetActivateScene(GameSceneType.MainScene);
    }

    public void SetBackGroundToSolidColor(Color color)
    {
        MainCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        MainCamera.GetComponent<Camera>().backgroundColor = color;
    }

    public void SetBackGroundToSkyBox()
    {
        MainCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
    }

    public GameObject SetSceneObject(GameSceneType sceneType)
    {
        switch(sceneType)
        {
            case GameSceneType.MainScene:
                return MainScene;
            case GameSceneType.ChallenegeScene:
                return ChallenegeScene;
            case GameSceneType.FightScene:
                return FightScene;
        }
        return null;
    }

    public void SetActivateScenePrev()
    {
        SetActivateScene(_gameSceneTypePrev);
    }

    public void SetActivateScene(GameSceneType sceneType)
    {
        if(sceneType == _gameSceneType)
        {
            return;
        }

        _gameSceneTypePrev = _gameSceneType;
        _gameSceneType = sceneType;
        
        MainScene.SetActive(GameSceneType.MainScene == sceneType);
        ChallenegeScene.SetActive(GameSceneType.ChallenegeScene == sceneType);
        FightScene.SetActive(GameSceneType.FightScene == sceneType);
    }

    public void Btn_Challenge_OnClick()
    {
        SetActivateScene(GameSceneType.ChallenegeScene);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameSceneType.MainScene == _gameSceneType)
            {
                Application.Quit();
            }
        }
    }
}
