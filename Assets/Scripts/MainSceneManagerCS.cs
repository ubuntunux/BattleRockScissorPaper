using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using GoogleMobileAds.Api;

public enum GameSceneType
{
    None,
    TitleScene,
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
    public GameObject Btn_Skin;
    public GameObject Popup_Exit;

    // GameScenes
    public GameObject TitleScene;
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

    BannerView bannerView;
    InterstitialAd interstitial;

    float _fadeInOutTimer = 0.0f;
    float _posY = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        RequestBanner();

        _gameSceneList.Add(TitleScene);
        _gameSceneList.Add(MainScene);
        _gameSceneList.Add(FightScene);
        _gameSceneList.Add(ChallenegeScene);
        _gameSceneList.Add(TrainingScene);
        _gameSceneList.Add(SkinScene);

        Popup_Exit.SetActive(false);

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

        LoadPlayerCharacterInfo();

        SetActivateScene(GameSceneType.TitleScene);
    }

    void OnEnable()
    {
        ResetMainSceneManager();
    }

    void OnDisable()
    {
    }

    public void ApplicationQuit()
    {
        Application.Quit();
    }

    void RequestBanner()
    {
        // Create a 320x50 banner at the top of the screen.
        {
            #if UNITY_ANDROID
                string adUnitId = "ca-app-pub-3940256099942544/6300978111";
            #elif UNITY_IPHONE
                string adUnitId = "ca-app-pub-3940256099942544/2934735716";
            #else
                string adUnitId = "unexpected_platform";
            #endif

            bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
            AdRequest request = new AdRequest.Builder().Build();
            bannerView.LoadAd(request);
        }

        {
            #if UNITY_ANDROID
                string adUnitId = "ca-app-pub-3940256099942544/1033173712";
            #elif UNITY_IPHONE
                string adUnitId = "ca-app-pub-3940256099942544/4411468910";
            #else
                string adUnitId = "unexpected_platform";
            #endif

            interstitial = new InterstitialAd(adUnitId);
            AdRequest request = new AdRequest.Builder().Build();
            interstitial.LoadAd(request);
        }
    }

    public void ResetMainSceneManager()
    {
    }

    public void SetScore(int score)
    {
        Text_Score.GetComponent<TextMeshProUGUI>().text = score.ToString();
    }

    public void LoadPlayerCharacterInfo()
    {
        int skinID = SystemValue.GetInt(SystemValue.PlayerSkinIDKey, Constants.DefaultSkinID);
        PlayerCS playerSkin = GetSkin(skinID);
        PlayerA.GetComponent<PlayerCS>().SetSkin(playerSkin);

        int score = PlayerA.GetComponent<PlayerCS>()._playerStat._score;
        SetScore(score);
    }

    public int GetSkinCount()
    {
        return skins.Length;
    }

    public GameObject[] GetSkins()
    {
        return skins;
    }

    public PlayerCS GetSkin(int skinID)
    {
        if(false == skinMap.ContainsKey(skinID))
        {
            skinID = Constants.DefaultSkinID;
        }
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

        PlayerA.GetComponent<PlayerCS>().InitializePlayerStat();
        PlayerA.GetComponent<PlayerCS>().SavePlayerStat();
        
        SystemValue.SetInt(SystemValue.PlayerSkinIDKey, Constants.DefaultSkinID);
        SystemValue.SetInt(SystemValue.PlayerLastStageKey, 0);

        LoadPlayerCharacterInfo();
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
            case GameSceneType.TitleScene:
                return TitleScene;
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
            default:
                Assert.IsTrue(false);
                break;
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

    public void ShowScoreAndSkinButton(bool show)
    {
        Image_Score.SetActive(show);
        //Btn_Skin.SetActive(show);
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
            case GameSceneType.TitleScene:
                ShowScoreAndSkinButton(false);
                break;
            case GameSceneType.MainScene:
                ShowScoreAndSkinButton(true);
                break;
            case GameSceneType.ChallenegeScene:
                ShowScoreAndSkinButton(true);
                Gym.SetActive(false);
                Stage.SetActive(true);
                break;
            case GameSceneType.TrainingScene:
                ShowScoreAndSkinButton(true);
                Gym.SetActive(true);
                Stage.SetActive(false);
                break;
            case GameSceneType.SkinScene:
                Image_Score.SetActive(true);
                //Btn_Skin.SetActive(false);
                break;
            case GameSceneType.FightScene:
                ShowScoreAndSkinButton(false);
                Gym.SetActive(false);
                Stage.SetActive(true);
                break;
            default:
                Assert.IsTrue(false);
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

    public void Btn_TitleScene_OnClick()
    {
        SetActivateScene(GameSceneType.MainScene);
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

    public void Btn_Exit_Cancle_OnClick()
    {
        Popup_Exit.SetActive(false);
    }

    public void Toggle_Popup_Exit()
    {
        Popup_Exit.SetActive(!Popup_Exit.activeInHierarchy);
    }

    // Update is called once per frame
    void Update()
    {
        // if(interstitial.IsLoaded())
        // {
        //     interstitial.Show();
        // }

        //bannerView.SetPosition(0, (int)_posY);

       if(GetActivateSceneType() == GameSceneType.MainScene)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Toggle_Popup_Exit();
            }
        }
    }
}
