using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum ChallengeState
{
    None,
    Versus,
}

public class ChallengeInfo
{
    public int _skinID = Constants.DefaultSkinID;
    public int _score = 0;

    public void LoadData()
    {
        _skinID = SystemValue.GetInt(SystemValue.SkinIDKey, Constants.DefaultSkinID);
        _score = SystemValue.GetInt(SystemValue.ChallengeScoreKey);
    }
    
    public void SaveData()
    {
        SystemValue.SetInt(SystemValue.SkinIDKey, _skinID);
        SystemValue.SetInt(SystemValue.ChallengeScoreKey, _score);
    }
}

public class ChallengeSceneManagerCS : MonoBehaviour
{
    public GameObject MainCamera;
    public GameObject MainSceneManager;
    public GameObject GameManager;
    public GameObject SkinScene;

    public GameObject LayerPortrait;    
    public GameObject LayerVersus;
    public GameObject PortraitSelected;
    
    // Match Card
    public GameObject LayerMatchCardManager;

    // challenge info
    public GameObject PlayerA;
    public GameObject PlayerB;
    public GameObject Text_Score;
    public GameObject PlayerA_Info;
    public GameObject PlayerB_Info;

    public GameObject[] ChallengePlayers;

    ChallengeState _challengeState = ChallengeState.None;

    ChallengeInfo _challengeInfo = new ChallengeInfo();
    int _currentStage = 0;

    float _timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    void OnEnable()
    {
        ResetChallengeScene();
    }

    void OnDisable()
    {
    }

    void ResetChallengeScene()
    {
        LayerPortrait.SetActive(true);
        LayerVersus.SetActive(false);

        LayerMatchCardManager.GetComponent<MatchCardManagerCS>().Reset();
        LayerMatchCardManager.SetActive(true);

        PortraitSelected.GetComponent<ChallengePortraitCS>().Reset();
        PortraitSelected.GetComponent<ChallengePortraitCS>().SetSelected(true);

        _timer = 0.0f;
        _challengeState = ChallengeState.None;

        // Load challenge info
        _challengeInfo.LoadData();
        SetChallengeInfo(_challengeInfo);

        // set character skin
        SetPlayerCharacterSkin();
        SelectChallengePlayer(0, false);

        PlayerA.SetActive(true);
        PlayerA.GetComponent<PlayerCS>().SetStateIdle();
        
        PlayerB.SetActive(true);
        PlayerB.GetComponent<PlayerCS>().SetStateIdle();
    }

    public void ClearChallengeInfo()
    {
        _challengeInfo = new ChallengeInfo();
        _challengeInfo.SaveData();

        PlayerStat playerStat = new PlayerStat();
        bool isPlayer = true;
        playerStat.InitializePlayerStat(null, isPlayer);
        playerStat.SavePlayerStat();

        MainSceneManager.GetComponent<MainSceneManagerCS>().ClearPlayerStats();

        ResetChallengeScene();
    }

    public void SetChallengeInfo(ChallengeInfo challengeInfo)
    {
        Text_Score.GetComponent<TextMeshProUGUI>().text = challengeInfo._score.ToString();
    }

    public void AddChallengeScore(int attackPoint, int hp, bool isWin)
    {
        int score = attackPoint + hp;

        _challengeInfo._score += score;

        // save data
        _challengeInfo.SaveData();
    }

    public void SetPlayerCharacterSkin()
    {
        int skinID = SystemValue.GetInt(SystemValue.SkinIDKey, _challengeInfo._skinID);
        PlayerCS playerSkin = MainSceneManager.GetComponent<MainSceneManagerCS>().GetSkin(skinID);
        PlayerA.GetComponent<PlayerCS>().SetSkin(playerSkin);
        PlayerA_Info.GetComponent<PlayerInfoCS>().SetPlayerInfo(PlayerA.GetComponent<PlayerCS>());
    }

    public void SetChallengePlayerPortraitByStage(GameObject challengePortrait, int stage)
    {
        if(0 <= stage && stage < ChallengePlayers.Length)
        {
            challengePortrait.SetActive(true);
            challengePortrait.GetComponent<ChallengePortraitCS>().SetChallengePlayerPortrait(
                ChallengePlayers[stage].GetComponent<PlayerCS>()
            );
        }
        else
        {
            challengePortrait.SetActive(false);
        }
        challengePortrait.GetComponent<ChallengePortraitCS>().SetPortraitState(ChallengePortraitState.None);
    }

    public void SelectChallengePlayer(int stage, bool playSound = true)
    {
        int stageLimit = ChallengePlayers.Length - 1;
        if(stage < 0)
        {
            stage = 0;
        }
        else if(stageLimit <= stage)
        {
            stage = stageLimit;
        }

        playSound = playSound && _currentStage != stage;

        _currentStage = stage;
        
        SetChallengePlayerPortraitByStage(PortraitSelected, stage);

        PlayerCS challengePlayerSkin = ChallengePlayers[stage].GetComponent<PlayerCS>();
        PlayerB.GetComponent<PlayerCS>().SetSkin(challengePlayerSkin);
        PlayerB_Info.GetComponent<PlayerInfoCS>().SetPlayerInfo(PlayerB.GetComponent<PlayerCS>());

        if(playSound)
        {
            PlayerB.GetComponent<PlayerCS>().PlayCharacterName();
        }
    }

    public void LayerMatchCardClick()
    {
        LayerMatchCardManager.SetActive(false);
    }

    public void PortraitLeftOnClick()
    {
        SelectChallengePlayer(_currentStage - 1);
    }

    public void PortraitRightOnClick()
    {
        SelectChallengePlayer(_currentStage + 1);
    }

    public void Btn_Fight_OnClick()
    {
        LayerPortrait.SetActive(false);
        LayerVersus.SetActive(true);
        LayerVersus.GetComponent<VersusCS>().ResetVersus(PlayerA.GetComponent<PlayerCS>(), PlayerB.GetComponent<PlayerCS>());
        
        _timer = 0.0f;
        _challengeState = ChallengeState.Versus;
    }

    public void Btn_Skin_OnClick()
    {
        MainSceneManager.GetComponent<MainSceneManagerCS>().SetActivateScene(GameSceneType.SkinScene);
    }

    // Update is called once per frame
    void Update()
    {
        if(MainSceneManager.GetComponent<MainSceneManagerCS>().GetActivateSceneType() == GameSceneType.ChallenegeScene)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(ChallengeState.Versus != _challengeState)
                {
                    MainSceneManager.GetComponent<MainSceneManagerCS>().SetActivateScene(GameSceneType.MainScene);
                }
            }
        }

        if(ChallengeState.Versus == _challengeState)
        {
            if(LayerVersus.GetComponent<VersusCS>().isEnd())
            {
                MainSceneManager.GetComponent<MainSceneManagerCS>().SetActivateScene(GameSceneType.FightScene);
                
                PlayerCreateInfo playerCreateInfoA = new PlayerCreateInfo();
                playerCreateInfoA._name = PlayerA.GetComponent<PlayerCS>().GetCharacterName();
                playerCreateInfoA._isPlayer = true;
                playerCreateInfoA._isLeft = true;
                playerCreateInfoA._skin = PlayerA.GetComponent<PlayerCS>();

                PlayerCreateInfo playerCreateInfoB = new PlayerCreateInfo();
                playerCreateInfoB._name = PlayerB.GetComponent<PlayerCS>().GetCharacterName();
                playerCreateInfoB._isPlayer = false;
                playerCreateInfoB._isLeft = false;
                playerCreateInfoB._skin = PlayerB.GetComponent<PlayerCS>();

                GameManager.GetComponent<GameManagerCS>().ResetGameManager(playerCreateInfoA, playerCreateInfoB);
            }
        }
        _timer += Time.deltaTime;
    }
}
