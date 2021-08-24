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

    // GameScenes
    public GameObject MainScene;
    public GameObject FightScene;
    public GameObject ChallenegeScene;
    public GameObject TrainingScene;
    public GameObject SkinScene;

    public GameObject PlayerA;
    public GameObject PlayerB;

    public GameObject Stage;
    public GameObject Gym;

    public GameObject[] skins;
    Dictionary<int, PlayerCS> skinMap = new Dictionary<int, PlayerCS>();

    List<GameObject> _gameSceneList = new List<GameObject>();

    GameSceneType _goalGameSceneType = GameSceneType.None;
    GameSceneType _gameSceneType = GameSceneType.None;
    GameSceneType _gameSceneTypePrev = GameSceneType.None;

    float _fadeInOutTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _gameSceneList.Add(MainScene);
        _gameSceneList.Add(FightScene);
        _gameSceneList.Add(ChallenegeScene);
        _gameSceneList.Add(TrainingScene);
        _gameSceneList.Add(SkinScene);

        int skinCount = skins.Length;
        for(int i = 0; i < skinCount; ++i)
        {
            PlayerCS playerSkin = skins[i].GetComponent<PlayerCS>();
            playerSkin.LoadPlayerStat();
            skinMap.Add(playerSkin.SkinID, playerSkin);
        }

        // create players
        PlayerCreateInfo playerCreateInfoA = new PlayerCreateInfo();
        playerCreateInfoA._name = "PlayerA";
        playerCreateInfoA._isPlayer = true;
        playerCreateInfoA._isLeft = true;
        PlayerA.GetComponent<PlayerCS>().ResetPlayer(null, null, null, playerCreateInfoA);

        PlayerCreateInfo playerCreateInfoB = new PlayerCreateInfo();
        playerCreateInfoB._name = "PlayerB";
        playerCreateInfoB._isPlayer = false;
        playerCreateInfoB._isLeft = false;
        PlayerB.GetComponent<PlayerCS>().ResetPlayer(null, null, null, playerCreateInfoB);

        SetActivateScene(GameSceneType.MainScene);
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

    public PlayerCS GetSkin(int skinID)
    {
        PlayerCS skin = skinMap[skinID];
        skin.LoadPlayerStat();
        return skin;
    }

    public void ClearPlayerStats()
    {
        int skinCount = skins.Length;
        for(int i = 0; i < skinCount; ++i)
        {
            PlayerCS playerSkin = skins[i].GetComponent<PlayerCS>();
            playerSkin.InitializePlayerStat();
            playerSkin.SavePlayerStat();
        }
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

    public GameObject GetSceneObject(GameSceneType sceneType)
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

        GameObject currentGameScene = GetSceneObject(sceneType);
        foreach(GameObject gameScene in _gameSceneList)
        {
            if(gameScene != currentGameScene)
            {
                gameScene.SetActive(false);
            }
        }
        currentGameScene.SetActive(true);
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
