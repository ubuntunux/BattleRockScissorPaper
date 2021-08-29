using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

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
    public GameObject Image_Score;
    public GameObject Text_Score;

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

        foreach(GameObject gameScene in _gameSceneList)
        {
            gameScene.SetActive(false);
        }

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
        PlayerA.SetActive(false);

        PlayerCreateInfo playerCreateInfoB = new PlayerCreateInfo();
        playerCreateInfoB._name = "PlayerB";
        playerCreateInfoB._isPlayer = false;
        playerCreateInfoB._isLeft = false;
        PlayerB.GetComponent<PlayerCS>().ResetPlayer(null, null, null, playerCreateInfoB);
        PlayerB.SetActive(false);

        SetPlayerCharacterInfo();

        SetActivateScene(GameSceneType.MainScene);
    }

    void OnEnable()
    {
        ResetMainSceneManager();
    }

    void OnDisable()
    {
    }

    public void ResetMainSceneManager()
    {
    }

    public void SetScore(int score)
    {
        Text_Score.GetComponent<TextMeshProUGUI>().text = score.ToString();
    }

    public void SetPlayerCharacterInfo()
    {
        int skinID = SystemValue.GetInt(SystemValue.SkinIDKey, Constants.DefaultSkinID);
        PlayerCS playerSkin = GetSkin(skinID);
        PlayerA.GetComponent<PlayerCS>().SetSkin(playerSkin);

        int score = PlayerA.GetComponent<PlayerCS>()._playerStat._score;
        SetScore(score);
    }

    public int GetSkinCount()
    {
        return skins.Length;
    }

    public PlayerCS GetSkin(int skinID)
    {
        PlayerCS skin = skinMap[skinID];
        skin.LoadPlayerStat();
        return skin;
    }

    public PlayerCS GetSkinByIndex(int index)
    {
        PlayerCS skin = skins[index].GetComponent<PlayerCS>();
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
            case GameSceneType.None:
            case GameSceneType.MainScene:
                Image_Score.SetActive(false);
                break;
            case GameSceneType.ChallenegeScene:
                Image_Score.SetActive(true);
                Gym.SetActive(false);
                Stage.SetActive(true);
                break;
            case GameSceneType.TrainingScene:
                Image_Score.SetActive(true);
                Gym.SetActive(true);
                Stage.SetActive(false);
                break;
            case GameSceneType.SkinScene:
                Image_Score.SetActive(true);
                break;
            case GameSceneType.FightScene:
                Image_Score.SetActive(false);
                Gym.SetActive(false);
                Stage.SetActive(true);
                break;
            default:
                Assert.IsTrue(true);
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
