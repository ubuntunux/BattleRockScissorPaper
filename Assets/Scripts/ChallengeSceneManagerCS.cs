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

    public GameObject LayerPortrait;    
    public GameObject LayerVersus;
    public GameObject PortraitSelected;
    public GameObject PortraitLeft;
    public GameObject PortraitRight;

    public GameObject PlayerA;
    public GameObject PlayerB;

    public GameObject Text_PlayerA_Name;
    public GameObject Text_Score;

    public GameObject[] ChallengePlayers;

    PlayerCreateInfo playerCreateInfoA = new PlayerCreateInfo();
    PlayerCreateInfo playerCreateInfoB = new PlayerCreateInfo();

    ChallengeState _challengeState = ChallengeState.None;

    ChallengePlayerCS _playerCharacter = null;
    ChallengePlayerCS _selectedChallengePlayer = null;

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

        // test
        PortraitLeft.GetComponent<ChallengePortraitCS>().SetChallengePlayer(ChallengePlayers[0].GetComponent<ChallengePlayerCS>());
        PortraitLeft.GetComponent<ChallengePortraitCS>().SetLose();
        PortraitSelected.GetComponent<ChallengePortraitCS>().SetChallengePlayer(ChallengePlayers[0].GetComponent<ChallengePlayerCS>());
        PortraitRight.GetComponent<ChallengePortraitCS>().SetChallengePlayer(ChallengePlayers[1].GetComponent<ChallengePlayerCS>());

        SelectChallengePlayer(ChallengePlayers[0].GetComponent<ChallengePlayerCS>(), false);
        SelectPlayer(ChallengePlayers[0].GetComponent<ChallengePlayerCS>());
        //

        playerCreateInfoA._name = "PlayerA";
        playerCreateInfoA._isNPC = false;
        playerCreateInfoA._isLeft = true;
        playerCreateInfoA._startPosition = new Vector3(-Constants.SelectDistance, Constants.GroundPosition, 0.0f);
        playerCreateInfoA._skin = _playerCharacter._skin.GetComponent<PlayerCS>();

        playerCreateInfoB._name = "PlayerB";
        playerCreateInfoB._isNPC = true;
        playerCreateInfoB._isLeft = false;
        playerCreateInfoB._startPosition = new Vector3(Constants.SelectDistance, Constants.GroundPosition, 0.0f);
        playerCreateInfoB._skin = _selectedChallengePlayer._skin.GetComponent<PlayerCS>();
        
        PlayerA.GetComponent<PlayerCS>().Reset(null, null, null, playerCreateInfoA);
        PlayerB.GetComponent<PlayerCS>().Reset(null, null, null, playerCreateInfoB);

        PlayerA.GetComponent<PlayerCS>().SetSelect();
        PlayerB.GetComponent<PlayerCS>().SetSelect();

        // Load Score
        int score = PlayerPrefs.GetInt("Score", 0);
        SetScore(score);
    }

    public void AddScore(int add_score)
    {
        SetScore(PlayerPrefs.GetInt("Score", 0) + add_score);
    }

    public void SetScore(int score)
    {
        Text_Score.GetComponent<TextMeshProUGUI>().text = score.ToString();
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.Save();
    }

    public void SelectPlayer(ChallengePlayerCS player)
    {
        _playerCharacter = player;
        Text_PlayerA_Name.GetComponent<TextMeshProUGUI>().text = player.GetCharacterName();
    }

    public void SelectChallengePlayer(ChallengePlayerCS player, bool playSound)
    {
        AddScore(1);

        _selectedChallengePlayer = player;

        PortraitSelected.GetComponent<ChallengePortraitCS>().SetChallengePlayer(player); 
               
        PlayerB.GetComponent<PlayerCS>().SetSkin(player._skin.GetComponent<PlayerCS>());        
        PlayerB.GetComponent<PlayerCS>().SetSelect();
        if(playSound)
        {
            PlayerB.GetComponent<PlayerCS>().PlayCharacterName();
        }
    }

    public void PortraitLeftOnClick()
    {
        ChallengePlayerCS player = PortraitLeft.GetComponent<ChallengePortraitCS>().GetChallengePlayer();
        SelectChallengePlayer(player, true);
    }

    public void PortraitRightOnClick()
    {
        ChallengePlayerCS player = PortraitRight.GetComponent<ChallengePortraitCS>().GetChallengePlayer();
        SelectChallengePlayer(player, true);
    }

    public void Btn_Fight_OnClick()
    {
        LayerPortrait.SetActive(false);
        LayerVersus.SetActive(true);
        LayerVersus.GetComponent<VersusCS>().Reset(_playerCharacter, _selectedChallengePlayer);
        
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
                
                playerCreateInfoA._startPosition = new Vector3(-Constants.IdleDistance, Constants.GroundPosition, 0.0f);
                playerCreateInfoA._skin = _playerCharacter._skin.GetComponent<PlayerCS>();

                playerCreateInfoB._startPosition = new Vector3(Constants.IdleDistance, Constants.GroundPosition, 0.0f);
                playerCreateInfoB._skin = _selectedChallengePlayer._skin.GetComponent<PlayerCS>();

                GameManager.GetComponent<GameManagerCS>().ResetGameManager(playerCreateInfoA, playerCreateInfoB);
            }
        }
        _timer += Time.deltaTime;
    }
}
