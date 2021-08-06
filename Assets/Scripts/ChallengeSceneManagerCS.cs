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
    public int _lastStage = 0;
    public int _score = 0;
    public int _level = 1;    
    public int _exp = 0;

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

    public bool AddExp(int exp)
    {
        _exp += exp;
        return calcLevel();
    }

    public bool calcLevel()
    {
        bool levelUp = false;
        while(_level < Constants.Exps.Length)
        {
            int exp_index = Mathf.Max(0, _level);
            if(Constants.Exps[exp_index] <= _exp)
            {
                _exp -= Constants.Exps[exp_index];
                _level += 1;
                levelUp = true;
            }
            else
            {
                break;
            }
        }
        return levelUp;
    }

    public void LoadData()
    {
        _stage = Mathf.Max(0, SystemValue.GetInt(SystemValue.ChallengeStageKey));
        _lastStage = Mathf.Max(0, SystemValue.GetInt(SystemValue.LastChallengeStageKey));
        _score = Mathf.Max(0, SystemValue.GetInt(SystemValue.ChallengeScoreKey));
        _level = Mathf.Max(1, SystemValue.GetInt(SystemValue.ChallengeLevelKey));
        _exp = Mathf.Max(0, SystemValue.GetInt(SystemValue.ChallengeExpKey));
    }
    
    public void SaveData()
    {
        SystemValue.SetInt(SystemValue.ChallengeStageKey, _stage);
        SystemValue.SetInt(SystemValue.LastChallengeStageKey, _lastStage);
        SystemValue.SetInt(SystemValue.ChallengeScoreKey, _score);
        SystemValue.SetInt(SystemValue.ChallengeLevelKey, _level);
        SystemValue.SetInt(SystemValue.ChallengeExpKey, _exp);
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
    public GameObject PortraitLeft;
    public GameObject PortraitRight;

    public GameObject PlayerA;
    public GameObject PlayerB;

    // challenge info
    public GameObject Text_PlayerA_Name;
    public GameObject Text_PlayerA_Level;
    public GameObject Text_Score;
    public GameObject ExpBar;
    public GameObject Text_PlayerB_Name;
    public GameObject Text_PlayerB_Level;

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
        PortraitLeft.GetComponent<ChallengePortraitCS>().Reset();
        PortraitRight.GetComponent<ChallengePortraitCS>().Reset();

        _selectedChallengePlayer = null;
        _timer = 0.0f;
        _challengeState = ChallengeState.None;

        // Load challenge info
        _challengeInfo.LoadData();
        SetChallengeInfo(_challengeInfo);

        SelectChallengePlayer(_challengeInfo._lastStage, false);
        SetPlayerCharacter();

        // set character skin
        playerCreateInfoA._name = _playerCharacter.GetCharacterName();
        playerCreateInfoA._level = _challengeInfo._level;
        playerCreateInfoA._isNPC = false;
        playerCreateInfoA._isLeft = true;
        playerCreateInfoA._startPosition = new Vector3(-Constants.SelectDistance, Constants.GroundPosition, 0.0f);
        playerCreateInfoA._skin = _playerCharacter;

        playerCreateInfoB._name = _selectedChallengePlayer.GetCharacterName();
        playerCreateInfoB._level = _currentStage;
        playerCreateInfoB._isNPC = true;
        playerCreateInfoB._isLeft = false;
        playerCreateInfoB._startPosition = new Vector3(Constants.SelectDistance, Constants.GroundPosition, 0.0f);
        playerCreateInfoB._skin = _selectedChallengePlayer;
        
        PlayerA.GetComponent<PlayerCS>().Reset(null, null, null, playerCreateInfoA);
        PlayerB.GetComponent<PlayerCS>().Reset(null, null, null, playerCreateInfoB);

        PlayerA.GetComponent<PlayerCS>().SetSelect();
        PlayerB.GetComponent<PlayerCS>().SetSelect();
    }

    public void SetChallengeInfo(ChallengeInfo challengeInfo)
    {
        Text_Score.GetComponent<TextMeshProUGUI>().text = challengeInfo._score.ToString();
        Text_PlayerA_Level.GetComponent<TextMeshProUGUI>().text = "Level: " + challengeInfo._level.ToString();
        int exp_index = Mathf.Max(0, Mathf.Min(Constants.Exps.Length - 1, challengeInfo._level));
        float expRatio = Mathf.Min(1.0f, (float)challengeInfo._exp / (float)Constants.Exps[exp_index]);
        ExpBar.transform.localScale = new Vector3(expRatio, 1.0f, 1.0f);
    }

    public void SetLastChallengeStage(int stage)
    {
        _challengeInfo._lastStage = stage;
        _challengeInfo.SaveData();
    }

    public bool AddChallengeScore(int challengeLevel, int attackPoint, int hp)
    {
        bool levelUp = false;
        int score = attackPoint + hp;
        int exp = score + Constants.Exps[_challengeInfo._level] / 10;

        _challengeInfo._score += score;
        _challengeInfo._exp += exp;

        while(_challengeInfo._level < Constants.Exps.Length)
        {
            int nextExp = Constants.Exps[_challengeInfo._level + 1];
            if(nextExp <= _challengeInfo._exp)
            {
                _challengeInfo._exp -= nextExp;
                _challengeInfo._level += 1;
                levelUp = true;
            }
            else
            {
                break;
            }
        }
        _challengeInfo.SaveData();
        return levelUp;
    }

    public void SetPlayerCharacter()
    {
        int skinID = SystemValue.GetInt(SystemValue.SkinIDKey, Constants.DefaultSkinID);
        _playerCharacter = SkinManager.GetComponent<SkinManagerCS>().GetSkin(skinID);
        Text_PlayerA_Name.GetComponent<TextMeshProUGUI>().text = _playerCharacter.GetCharacterName();
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

        SetChallengePlayerPortraitByStage(PortraitLeft, stage - 1);
        SetChallengePlayerPortraitByStage(PortraitSelected, stage);
        SetChallengePlayerPortraitByStage(PortraitRight, stage + 1);

        _selectedChallengePlayer = ChallengePlayers[stage].GetComponent<PlayerCS>();

        Text_PlayerB_Name.GetComponent<TextMeshProUGUI>().text = _selectedChallengePlayer.GetCharacterName();
        int level = stage;
        Text_PlayerB_Level.GetComponent<TextMeshProUGUI>().text = "Level: " + level.ToString();

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
                playerCreateInfoA._level = _challengeInfo._level;
                playerCreateInfoA._startPosition = new Vector3(-Constants.IdleDistance, Constants.GroundPosition, 0.0f);
                playerCreateInfoA._skin = _playerCharacter;

                playerCreateInfoB._name = _selectedChallengePlayer.GetCharacterName();
                playerCreateInfoB._level = _currentStage;
                playerCreateInfoB._startPosition = new Vector3(Constants.IdleDistance, Constants.GroundPosition, 0.0f);
                playerCreateInfoB._skin = _selectedChallengePlayer;

                GameManager.GetComponent<GameManagerCS>().ResetGameManager(playerCreateInfoA, playerCreateInfoB);
            }
        }
        _timer += Time.deltaTime;
    }
}
