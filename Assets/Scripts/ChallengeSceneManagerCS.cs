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
    public int _stage = 0;
    public int _lastChallenegeStage = 0;
    public int _score = 0;

    public bool TakeScore(int score)
    {
        if(score <= _score)
        {
            _score -= score;
            return true;
        }
        return false;
    }

    public void AddScore(int score)
    {
        _score += score;
    }

    public void LoadData()
    {
        _stage = Mathf.Max(0, SystemValue.GetInt(SystemValue.ChallengeStageKey));
        _lastChallenegeStage = Mathf.Max(0, SystemValue.GetInt(SystemValue.LastChallengeStageKey));
        _score = Mathf.Max(0, SystemValue.GetInt(SystemValue.ChallengeScoreKey));
    }
    
    public void SaveData()
    {
        SystemValue.SetInt(SystemValue.ChallengeStageKey, _stage);
        SystemValue.SetInt(SystemValue.LastChallengeStageKey, _lastChallenegeStage);
        SystemValue.SetInt(SystemValue.ChallengeScoreKey, _score);
    }
}

public class ChallengeSceneManagerCS : MonoBehaviour
{
    public GameObject MainCamera;
    public GameObject MainSceneManager;
    public GameObject GameManager;
    public GameObject SkinManager;

    public GameObject LayerPortrait;    
    public GameObject LayerVersus;
    public GameObject PortraitSelected;

    public GameObject PlayerA;
    public GameObject PlayerB;

    // challenge info
    public GameObject Text_Score;
    public GameObject PlayerA_Info;
    public GameObject PlayerB_Info;

    public GameObject[] ChallengePlayers;

    PlayerCreateInfo playerCreateInfoA = new PlayerCreateInfo();
    PlayerCreateInfo playerCreateInfoB = new PlayerCreateInfo();

    ChallengeState _challengeState = ChallengeState.None;

    PlayerCS _playerCharacter = null;
    PlayerCS _selectedChallengePlayer = null;

    ChallengeInfo _challengeInfo = new ChallengeInfo();
    int _currentStage = 0;

    float _timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    void OnEnable()
    {
        Reset();
    }

    void OnDisable()
    {        
    }

    void Reset()
    {
        LayerPortrait.SetActive(true);
        LayerVersus.SetActive(false);

        PortraitSelected.GetComponent<ChallengePortraitCS>().Reset();
        PortraitSelected.GetComponent<ChallengePortraitCS>().SetSelected(true);

        _selectedChallengePlayer = null;
        _timer = 0.0f;
        _challengeState = ChallengeState.None;

        // Load challenge info
        _challengeInfo.LoadData();
        SetChallengeInfo(_challengeInfo);

        SelectChallengePlayer(_challengeInfo._lastChallenegeStage, false);
        SetPlayerCharacter();

        // set character skin
        playerCreateInfoA._name = _playerCharacter.GetCharacterName();
        playerCreateInfoA._isNPC = false;
        playerCreateInfoA._isLeft = true;
        playerCreateInfoA._startPosition = new Vector3(-Constants.SelectDistance, Constants.GroundPosition, 0.0f);
        playerCreateInfoA._skin = _playerCharacter;

        playerCreateInfoB._name = _selectedChallengePlayer.GetCharacterName();
        playerCreateInfoB._isNPC = true;
        playerCreateInfoB._isLeft = false;
        playerCreateInfoB._startPosition = new Vector3(Constants.SelectDistance, Constants.GroundPosition, 0.0f);
        playerCreateInfoB._skin = _selectedChallengePlayer;
        
        PlayerA.GetComponent<PlayerCS>().Reset(null, null, null, playerCreateInfoA);
        PlayerB.GetComponent<PlayerCS>().Reset(null, null, null, playerCreateInfoB);

        PlayerA.GetComponent<PlayerCS>().SetSelect();
        PlayerB.GetComponent<PlayerCS>().SetSelect();
    }

    public void ClearChallengeInfo()
    {
        _challengeInfo = new ChallengeInfo();
        _challengeInfo.SaveData();

        Reset();
    }

    public void SetChallengeInfo(ChallengeInfo challengeInfo)
    {
        Text_Score.GetComponent<TextMeshProUGUI>().text = challengeInfo._score.ToString();
    }

    public void SetLastChallengeStage(int stage)
    {
        _challengeInfo._lastChallenegeStage = stage;
        _challengeInfo.SaveData();
    }

    public void AddChallengeScore(int attackPoint, int hp, bool isWin)
    {
        int score = attackPoint + hp;

        _challengeInfo._score += score;
        
        // next level
        if(isWin)
        {
            if(_challengeInfo._stage <= _challengeInfo._lastChallenegeStage)
            {
                _challengeInfo._stage = _challengeInfo._lastChallenegeStage + 1;
            }
        }

        // save data
        _challengeInfo.SaveData();
    }

    public void SetPlayerCharacter()
    {
        int skinID = SystemValue.GetInt(SystemValue.SkinIDKey, Constants.DefaultSkinID);
        _playerCharacter = SkinManager.GetComponent<SkinManagerCS>().GetSkin(skinID);
        PlayerA_Info.GetComponent<PlayerInfoCS>().SetPlayerInfo(_playerCharacter);
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

        ChallengePortraitState portraitState = ChallengePortraitState.None;
        if(PortraitSelected != challengePortrait)
        {
            if(stage < _challengeInfo._stage)
            {
                portraitState = ChallengePortraitState.Lose;
            }
            else if(_challengeInfo._stage < stage)
            {
                portraitState = ChallengePortraitState.Lock;
            }
        }
        challengePortrait.GetComponent<ChallengePortraitCS>().SetPortraitState(portraitState);
    }

    public void SelectChallengePlayer(int stage, bool playSound = true)
    {
        int stageLimit = Mathf.Min(ChallengePlayers.Length - 1, _challengeInfo._stage);
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
        
        SetLastChallengeStage(stage);

        SetChallengePlayerPortraitByStage(PortraitSelected, stage);

        _selectedChallengePlayer = ChallengePlayers[stage].GetComponent<PlayerCS>();

        PlayerB_Info.GetComponent<PlayerInfoCS>().SetPlayerInfo(_selectedChallengePlayer);
        PlayerB.GetComponent<PlayerCS>().SetSkin(_selectedChallengePlayer);
        PlayerB.GetComponent<PlayerCS>().SetSelect();

        if(playSound)
        {
            PlayerB.GetComponent<PlayerCS>().PlayCharacterName();
        }
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
        LayerVersus.GetComponent<VersusCS>().ResetVersus(
            _playerCharacter,
            _selectedChallengePlayer
        );
        
        _timer = 0.0f;
        _challengeState = ChallengeState.Versus;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(ChallengeState.Versus != _challengeState)
            {
                MainSceneManager.GetComponent<MainSceneManagerCS>().SetActivateScene(GameSceneType.MainScene);
            }
        }

        if(ChallengeState.Versus == _challengeState)
        {
            if(LayerVersus.GetComponent<VersusCS>().isEnd())
            {
                MainSceneManager.GetComponent<MainSceneManagerCS>().SetActivateScene(GameSceneType.FightScene);
                
                playerCreateInfoA._name = _playerCharacter.GetCharacterName();
                playerCreateInfoA._startPosition = new Vector3(-Constants.IdleDistance, Constants.GroundPosition, 0.0f);
                playerCreateInfoA._skin = _playerCharacter;

                playerCreateInfoB._name = _selectedChallengePlayer.GetCharacterName();
                playerCreateInfoB._startPosition = new Vector3(Constants.IdleDistance, Constants.GroundPosition, 0.0f);
                playerCreateInfoB._skin = _selectedChallengePlayer;

                GameManager.GetComponent<GameManagerCS>().ResetGameManager(playerCreateInfoA, playerCreateInfoB);
            }
        }
        _timer += Time.deltaTime;
    }
}
