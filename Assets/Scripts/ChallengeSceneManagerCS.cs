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

public class ChallengeSceneManagerCS : MonoBehaviour
{
    public GameObject MainCamera;
    public GameObject MainSceneManager;
    public GameObject GameManager;
    public GameObject SkinScene;

    public GameObject LayerPortrait;    
    public GameObject LayerVersus;
    public GameObject VersusPortraitPlayerA;
    public GameObject VersusPortraitPlayerB;
    
    // Match Card
    public GameObject LayerMatchCardManager;

    // challenge info
    public GameObject PlayerA;
    public GameObject PlayerB;    
    public GameObject PlayerA_Info;
    public GameObject PlayerB_Info;

    public GameObject[] ChallengePlayers;

    ChallengeState _challengeState = ChallengeState.None;

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

        LayerMatchCardManager.GetComponent<MatchCardManagerCS>().ResetMatchCardManager();
        LayerMatchCardManager.SetActive(true);

        VersusPortraitPlayerB.GetComponent<ChallengePortraitCS>().Reset();
        VersusPortraitPlayerB.GetComponent<ChallengePortraitCS>().SetSelected(true);

        _timer = 0.0f;
        _challengeState = ChallengeState.None;

        // set character info
        SetPlayerCharacterInfo();
        SelectChallengePlayer(0, false);

        PlayerA.SetActive(true);
        PlayerA.GetComponent<PlayerCS>().SetStateIdle();
        
        PlayerB.SetActive(true);
        PlayerB.GetComponent<PlayerCS>().SetStateIdle();
    }

    public void ClearChallengeInfo()
    {
        PlayerA.GetComponent<PlayerCS>()._playerStat.SavePlayerStat();
        MainSceneManager.GetComponent<MainSceneManagerCS>().ClearPlayerStats();
        ResetChallengeScene();
    }

    public void AddChallengeScore(int attackPoint, int hp, bool isWin)
    {
        int score = attackPoint + hp;
        
        // save data
        PlayerA.GetComponent<PlayerCS>()._playerStat._score += score;
        PlayerA.GetComponent<PlayerCS>()._playerStat.SavePlayerStat();

        MainSceneManager.GetComponent<MainSceneManagerCS>().SetScore(PlayerA.GetComponent<PlayerCS>()._playerStat._score);
    }

    public void SetPlayerCharacterInfo()
    {
        PlayerA_Info.GetComponent<PlayerInfoCS>().SetPlayerInfo(PlayerA.GetComponent<PlayerCS>());
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

        VersusPortraitPlayerA.GetComponent<ChallengePortraitCS>().SetChallengePlayerPortrait(PlayerA.GetComponent<PlayerCS>());
        VersusPortraitPlayerB.GetComponent<ChallengePortraitCS>().SetChallengePlayerPortrait(PlayerB.GetComponent<PlayerCS>());
        
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
