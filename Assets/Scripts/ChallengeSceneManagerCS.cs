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
    List<GameObject> ChallengePlayerList = new List<GameObject>();
    List<GameObject> PlayerSkinList = new List<GameObject>();

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

    void SortPlayerList()
    {
        // sort challenge players
        ChallengePlayerList.Clear();
        foreach(GameObject player in ChallengePlayers)
        {
            ChallengePlayerList.Add(player);
        }

        ChallengePlayerList.Sort(delegate (GameObject a, GameObject b)
        {
            return a.GetComponent<PlayerCS>()._playerStat._rank < b.GetComponent<PlayerCS>()._playerStat._rank ? 1 : -1;
        });

        // sort player skins
        PlayerSkinList.Clear();
        GameObject[] playerSkins = MainSceneManager.GetComponent<MainSceneManagerCS>().GetSkins();
        foreach(GameObject player in playerSkins)
        {
            PlayerSkinList.Add(player);
        }
        
        PlayerSkinList.Sort(delegate (GameObject a, GameObject b)
        {
            bool purchasedA = a.GetComponent<PlayerCS>()._playerStat._purchased;
            bool purchasedB = b.GetComponent<PlayerCS>()._playerStat._purchased;

            if(purchasedA != purchasedB)
            {
                return purchasedA ? -1 : 1;
            }
            return 0;
        });
    }

    void ResetChallengeScene()
    {
        _timer = 0.0f;
        _challengeState = ChallengeState.None;

        SortPlayerList();

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
        LayerSelectPlayerA.GetComponent<MatchCardManagerCS>().ResetMatchCardManager(this, PlayerSkinList, PlayerA);
        LayerSelectPlayerA.SetActive(true);

        LayerSelectPlayerB.GetComponent<MatchCardManagerCS>().ResetMatchCardManager(this, isVersusScene ? PlayerSkinList : ChallengePlayerList, PlayerB);
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
        MainSceneManager.GetComponent<MainSceneManagerCS>().ClearPlayerStats();
        ResetChallengeScene();
    }

    public void CalcChallengePlayersRank()
    {
        int maxRoundCount = 3;
        int maxWinCount = 2;

        List<GameObject> playerList = new List<GameObject>();
        foreach(GameObject player in ChallengePlayers)
        {
            playerList.Add(player);
        }
        
        // fight simulation
        foreach(GameObject playerA in ChallengePlayers)
        {
            foreach(GameObject playerB in ChallengePlayers)
            {
                if(playerA != playerB)
                {
                    int roundIndex = 0;
                    int winCountA = 0;
                    int winCountB = 0;

                    PlayerCS playerCS_A = playerA.GetComponent<PlayerCS>();
                    PlayerCS playerCS_B = playerB.GetComponent<PlayerCS>();
                    
                    for(int i = 0; i < maxRoundCount; ++i)
                    {
                        int hpA = playerCS_A.GetMaxHP();
                        int hpB = playerCS_B.GetMaxHP();

                        while(0 < hpA && 0 < hpB)
                        {
                            float powerGuageA = 1.0f - Mathf.Pow(Random.value, 2.0f);
                            float powerGuageB = 1.0f - Mathf.Pow(Random.value, 2.0f);

                            int powerA = playerCS_A.GetPowerWithGuage(powerGuageA);
                            int powerB = playerCS_B.GetPowerWithGuage(powerGuageB);

                            AttackType attackTypeA = (AttackType)(Random.Range(0, 3) + 1);
                            AttackType attackTypeB = (AttackType)(Random.Range(0, 3) + 1);

                            if(AttackType.None != attackTypeB && (attackTypeB == attackTypeA || GameManagerCS.checkLose(attackTypeB, attackTypeA)))
                            {
                                hpA -= powerB;
                            }

                            if(AttackType.None != attackTypeA && (attackTypeA == attackTypeB || GameManagerCS.checkLose(attackTypeA, attackTypeB)))
                            {
                                hpB -= powerA;
                            }

                            if(hpB <= 0 && 0 < hpA)
                            {
                                ++winCountA;
                            }
                            else if(hpA <= 0 && 0 < hpB)
                            {
                                ++winCountB;
                            }
                        }

                        if(maxWinCount <= winCountA || maxWinCount <= winCountB)
                        {
                            break;
                        }
                    }

                    // round result
                    if(winCountB < winCountA)
                    {
                        ++playerCS_A._playerStat._win;
                        ++playerCS_B._playerStat._lose;
                    }
                    else if(winCountA < winCountB)
                    {
                        ++playerCS_B._playerStat._win;
                        ++playerCS_A._playerStat._lose;
                    }
                    else
                    {
                        ++playerCS_A._playerStat._draw;
                        ++playerCS_B._playerStat._draw;
                    }
                }
            }
        }

        // sort by win, draw, lose
        playerList.Sort(delegate (GameObject a, GameObject b)
        {
            PlayerStat playerStatA = a.GetComponent<PlayerCS>()._playerStat;
            PlayerStat playerStatB = b.GetComponent<PlayerCS>()._playerStat;

            if(playerStatA._win != playerStatB._win)
            {
                return playerStatA._win < playerStatB._win ? 1 : -1;
            }

            if(playerStatA._draw != playerStatB._draw)
            {
                return playerStatA._draw < playerStatB._draw ? 1 : -1;
            }

            if(playerStatA._lose != playerStatB._lose)
            {
                return playerStatA._lose < playerStatB._lose ? -1 : 1;
            }
            return 0;
        });

        // set rank
        for(int i = 0; i < playerList.Count; ++i)
        {
            playerList[i].GetComponent<PlayerCS>()._playerStat._rank = i;
            playerList[i].GetComponent<PlayerCS>().SavePlayerStat();
        }
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
