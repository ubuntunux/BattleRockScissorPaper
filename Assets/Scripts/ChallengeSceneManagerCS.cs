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
    public AudioSource Snd_MatchCardSelect;
    public GameObject Btn_Back;
    
    // Match Card
    public GameObject LayerSelectPlayerA;
    public GameObject LayerSelectPlayerB;

    // challenge info
    public GameObject PlayerA;
    public GameObject PlayerB;    
    public GameObject PlayerA_Info;
    public GameObject PlayerB_Info;

    public GameObject[] ChallengePlayers;

    ChallengeState _challengeState = ChallengeState.None;

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
        _timer = 0.0f;
        _challengeState = ChallengeState.None;

        LayerPortrait.SetActive(true);
        LayerVersus.SetActive(false);
        Btn_Back.SetActive(true);

        GameObject[] playerSkins = MainSceneManager.GetComponent<MainSceneManagerCS>().GetSkins();
        LayerSelectPlayerA.GetComponent<MatchCardManagerCS>().ResetMatchCardManager(this, playerSkins, PlayerA);
        LayerSelectPlayerA.SetActive(true);

        LayerSelectPlayerB.GetComponent<MatchCardManagerCS>().ResetMatchCardManager(this, ChallengePlayers, PlayerB);
        LayerSelectPlayerB.SetActive(true);

        VersusPortraitPlayerB.GetComponent<ChallengePortraitCS>().Reset();
        VersusPortraitPlayerB.GetComponent<ChallengePortraitCS>().SetSelected(true);

        PlayerA.SetActive(true);
        PlayerA.GetComponent<PlayerCS>().SetStateIdle();

        PlayerB.SetActive(true);
        PlayerB.GetComponent<PlayerCS>().SetStateIdle();

        // 
        SetPlayerCharacter();
        
        int currentStage = SystemValue.GetInt(SystemValue.PlayerLastStageKey, 0);
        LayerSelectPlayerB.GetComponent<MatchCardManagerCS>().SelectMatchCardByIndex(currentStage, false);
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

    void SetPlayerCharacter()
    {
        int skinID = SystemValue.GetInt(SystemValue.PlayerSkinIDKey, Constants.DefaultSkinID);
        LayerSelectPlayerA.GetComponent<MatchCardManagerCS>().SelectMatchCardBySkinID(skinID, false);
        PlayerA_Info.GetComponent<PlayerInfoCS>().SetPlayerInfo(PlayerA.GetComponent<PlayerCS>());
    }

    public void SelectChallengePlayer(GameObject player, PlayerCS playerSkin, int matchCardIndex, bool playSound = true)
    {
        bool isPlayerA = PlayerA == player;
        GameObject playerInfo = isPlayerA ? PlayerA_Info : PlayerB_Info;

        player.SetActive(true);
        player.GetComponent<PlayerCS>().SetSkin(playerSkin);
        playerInfo.SetActive(true);
        playerInfo.GetComponent<PlayerInfoCS>().SetPlayerInfo(player.GetComponent<PlayerCS>());

        if(playSound)
        {
            Snd_MatchCardSelect.Play();
            player.GetComponent<PlayerCS>().PlayCharacterName();
        }

        if(isPlayerA)
        {
            int skinID = player.GetComponent<PlayerCS>()._playerStat._skinID;
            SystemValue.SetInt(SystemValue.PlayerSkinIDKey, skinID);
        }
        else
        {
            SystemValue.SetInt(SystemValue.PlayerLastStageKey, matchCardIndex);
        }
    }

    public void Btn_Fight_OnClick()
    {
        if(false == PlayerB.activeInHierarchy)
        {
            return;
        }

        MainSceneManager.GetComponent<MainSceneManagerCS>().ShowScoreAndSkinButton(false);

        Btn_Back.SetActive(false);
        LayerSelectPlayerA.SetActive(false);
        LayerSelectPlayerB.SetActive(false);
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

    public void Exit()
    {
        if(ChallengeState.Versus != _challengeState)
        {
            MainSceneManager.GetComponent<MainSceneManagerCS>().SetActivateScene(GameSceneType.MainScene);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(MainSceneManager.GetComponent<MainSceneManagerCS>().GetActivateSceneType() == GameSceneType.ChallenegeScene)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Exit();
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
                playerCreateInfoA._isPlayerA = true;
                playerCreateInfoA._skin = PlayerA.GetComponent<PlayerCS>();
                
                PlayerCreateInfo playerCreateInfoB = new PlayerCreateInfo();
                playerCreateInfoB._name = PlayerB.GetComponent<PlayerCS>().GetCharacterName();
                playerCreateInfoB._isPlayer = false;
                playerCreateInfoB._isPlayerA = false;
                playerCreateInfoB._skin = PlayerB.GetComponent<PlayerCS>();

                GameManager.GetComponent<GameManagerCS>().ResetGameManager(playerCreateInfoA, playerCreateInfoB);
            }
        }
        _timer += Time.deltaTime;
    }
}
