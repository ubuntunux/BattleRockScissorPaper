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

    public GameObject Text_Title;
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

    public bool IsChallengeScene()
    {
        return GameSceneType.ChallenegeScene == MainSceneManager.GetComponent<MainSceneManagerCS>().GetActivateSceneType();
    }

    public bool IsVersusScene()
    {
        return GameSceneType.VersusScene == MainSceneManager.GetComponent<MainSceneManagerCS>().GetActivateSceneType();
    }

    void ResetChallengeScene()
    {
        _timer = 0.0f;
        _challengeState = ChallengeState.None;

        LayerPortrait.SetActive(true);
        LayerVersus.SetActive(false);
        Btn_Back.SetActive(true);

        bool isVersusScene = IsVersusScene();

        Text_Title.GetComponent<TextMeshProUGUI>().text = isVersusScene ? "1P vs 2P" : "Challenge League";

        PlayerA.SetActive(true);
        PlayerA.GetComponent<PlayerCS>().SetPlayerInfo(true, false == isVersusScene);
        PlayerA.GetComponent<PlayerCS>().SetStateIdle();

        PlayerB.SetActive(true);
        PlayerB.GetComponent<PlayerCS>().SetPlayerInfo(isVersusScene, false);
        PlayerB.GetComponent<PlayerCS>().SetStateIdle();

        GameObject[] playerSkins = MainSceneManager.GetComponent<MainSceneManagerCS>().GetSkins();
        LayerSelectPlayerA.GetComponent<MatchCardManagerCS>().ResetMatchCardManager(this, playerSkins, PlayerA);
        LayerSelectPlayerA.SetActive(true);

        LayerSelectPlayerB.GetComponent<MatchCardManagerCS>().ResetMatchCardManager(this, isVersusScene ? playerSkins : ChallengePlayers, PlayerB);
        LayerSelectPlayerB.SetActive(true);

        VersusPortraitPlayerB.GetComponent<ChallengePortraitCS>().Reset();
        VersusPortraitPlayerB.GetComponent<ChallengePortraitCS>().SetSelected(true);

        // select player
        ResetPlayerCharacter(true);
        
        if(isVersusScene)
        {
            ResetPlayerCharacter(false);
        }
        else
        {
            int currentStage = SystemValue.GetInt(SystemValue.PlayerSelectStageKey);
            LayerSelectPlayerB.GetComponent<MatchCardManagerCS>().SelectMatchCardByIndex(currentStage, false);
        }
    }

    public void ClearChallengeInfo()
    {
        PlayerA.GetComponent<PlayerCS>()._playerStat.SavePlayerStat();
        MainSceneManager.GetComponent<MainSceneManagerCS>().ClearPlayerStats();
        ResetChallengeScene();
    }

    void ResetPlayerCharacter(bool isPlayerA)
    {
        int skinID = SystemValue.GetInt(isPlayerA ? SystemValue.PlayerSkinIDKey : SystemValue.PlayerBSkinIDKey, Constants.DefaultSkinID);
        if(isPlayerA)
        {
            LayerSelectPlayerA.GetComponent<MatchCardManagerCS>().SelectMatchCardBySkinID(skinID, false);
            PlayerA_Info.GetComponent<PlayerInfoCS>().SetPlayerInfo(PlayerA.GetComponent<PlayerCS>());
        }
        else
        {
            LayerSelectPlayerB.GetComponent<MatchCardManagerCS>().SelectMatchCardBySkinID(skinID, false);
            PlayerB_Info.GetComponent<PlayerInfoCS>().SetPlayerInfo(PlayerB.GetComponent<PlayerCS>());
        }
    }

    public void SelectChallengePlayer(GameObject player, PlayerCS playerSkin, int matchCardIndex, bool playSound = true)
    {
        bool isVersusScene = IsVersusScene();
        bool isPlayerA = PlayerA == player;
        GameObject playerInfo = isPlayerA ? PlayerA_Info : PlayerB_Info;

        player.SetActive(true);
        player.GetComponent<PlayerCS>().SetSkin(playerSkin);
        playerInfo.SetActive(true);
        playerInfo.GetComponent<PlayerInfoCS>().SetPlayerInfo(player.GetComponent<PlayerCS>());

        if(playSound)
        {
            player.GetComponent<PlayerCS>().PlayCharacterName();
        }

        // save last selected state
        int skinID = player.GetComponent<PlayerCS>()._playerStat._skinID;
        if(isPlayerA)
        {
            SystemValue.SetInt(SystemValue.PlayerSkinIDKey, skinID);
        }
        else
        {
            if(isVersusScene)
            {
                SystemValue.SetInt(SystemValue.PlayerBSkinIDKey, skinID);
            }
            else
            {
                SystemValue.SetInt(SystemValue.PlayerSelectStageKey, matchCardIndex);
            }
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

    public void Btn_Skin_OnClick(GameObject player)
    {
        SkinScene.GetComponent<SkinManagerCS>().SetTargetPlayer(player);
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameSceneType activateSceneType = MainSceneManager.GetComponent<MainSceneManagerCS>().GetActivateSceneType();
            if(GameSceneType.ChallenegeScene == activateSceneType || GameSceneType.VersusScene == activateSceneType)
            {
                Exit();
            }
        }

        if(ChallengeState.Versus == _challengeState)
        {
            if(LayerVersus.GetComponent<VersusCS>().isEnd())
            {
                bool isVersusScene = IsVersusScene();

                MainSceneManager.GetComponent<MainSceneManagerCS>().SetActivateScene(GameSceneType.FightScene);
                
                PlayerCreateInfo playerCreateInfoA = new PlayerCreateInfo();
                playerCreateInfoA._name = PlayerA.GetComponent<PlayerCS>().GetCharacterName();
                playerCreateInfoA._isPlayer = true;
                playerCreateInfoA._usePlayerStat = !isVersusScene;
                playerCreateInfoA._isPlayerA = true;
                playerCreateInfoA._skin = PlayerA.GetComponent<PlayerCS>();
                
                PlayerCreateInfo playerCreateInfoB = new PlayerCreateInfo();
                playerCreateInfoB._name = PlayerB.GetComponent<PlayerCS>().GetCharacterName();
                playerCreateInfoB._isPlayer = isVersusScene;
                playerCreateInfoB._usePlayerStat = false;
                playerCreateInfoB._isPlayerA = false;
                playerCreateInfoB._skin = PlayerB.GetComponent<PlayerCS>();

                GameManager.GetComponent<GameManagerCS>().ResetGameManager(isVersusScene, playerCreateInfoA, playerCreateInfoB);
            }
        }
        _timer += Time.deltaTime;
    }
}
