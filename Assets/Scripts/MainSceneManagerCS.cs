using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameSceneType
{
    None,
    MainScene,
    ChallenegeScene,
    TrainingScene,
    SkinScene,
    FightScene
}

public class MainSceneManagerCS : MonoBehaviour
{
    public GameObject MainCamera;
    public GameObject FadePanel;
    public GameObject MainScene;
    public GameObject FightScene;
    public GameObject ChallenegeScene;
    public GameObject TrainingScene;
    public GameObject SkinScene;

    public GameObject Stage;
    public GameObject Gym;

    public AudioSource Snd_click;

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
            case GameSceneType.SkinScene:
                return SkinScene;
            case GameSceneType.TrainingScene:
                return TrainingScene;
        }
        return null;
    }

    public GameSceneType GetActivateSceneType()
    {
        return _gameSceneType;
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

        Snd_click.Play();

        switch(sceneType)
        {
            case GameSceneType.ChallenegeScene:
            case GameSceneType.FightScene:
                Gym.SetActive(false);
                Stage.SetActive(true);
                break;
            case GameSceneType.TrainingScene:
                Gym.SetActive(true);
                Stage.SetActive(false);
                break;
        }

        _fadeInOutTimer = Constants.FadeTime;
        _gameSceneTypePrev = _gameSceneType;
        _gameSceneType = sceneType;
        
        MainScene.SetActive(GameSceneType.MainScene == sceneType);
        ChallenegeScene.SetActive(GameSceneType.ChallenegeScene == sceneType);
        FightScene.SetActive(GameSceneType.FightScene == sceneType);
        TrainingScene.SetActive(GameSceneType.TrainingScene == sceneType);
    }

    public void Btn_League_OnClick()
    {
        SetActivateScene(GameSceneType.ChallenegeScene);
    }

    public void Btn_Training_OnClick()
    {
        SetActivateScene(GameSceneType.TrainingScene);
    }

    public void Btn_Skin_OnClick()
    {
        SetActivateScene(GameSceneType.SkinScene);
    }

    // Update is called once per frame
    void Update()
    {
        if(GetActivateSceneType() == GameSceneType.MainScene)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(GameSceneType.MainScene == _gameSceneType)
                {
                    Application.Quit();
                }
            }
        }

        // if(0.0f != _fadeInOutTimer)
        // {
        //     float fadeInOutTimer = 1.0f - Mathf.Abs((_fadeInOutTimer / Constants.FadeTime) * 2.0f - 1.0f);
        //     FadePanel.GetComponent<Image>().color = new Color(0,0,0, fadeInOutTimer * 0.5f);

        //     _fadeInOutTimer -= Time.deltaTime;
        // }
    }
}
