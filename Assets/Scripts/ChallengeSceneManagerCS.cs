using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public GameObject Portrait00;
    public GameObject LayerVersus;

    public GameObject PlayerA;
    public GameObject PlayerB;
    public GameObject Btn_Fight;

    public GameObject[] ChallengePlayers;

    PlayerCreateInfo playerCreateInfoA = new PlayerCreateInfo();
    PlayerCreateInfo playerCreateInfoB = new PlayerCreateInfo();

    ChallengeState _challengeState = ChallengeState.None;

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
        playerCreateInfoA._name = "PlayerA";
        playerCreateInfoA._isNPC = false;
        playerCreateInfoA._isLeft = true;
        playerCreateInfoA._startPosition = new Vector3(-Constants.SelectDistance, Constants.GroundPosition, 0.0f);

        playerCreateInfoB._name = "PlayerB";
        playerCreateInfoB._isNPC = true;
        playerCreateInfoB._isLeft = false;
        playerCreateInfoB._startPosition = new Vector3(Constants.SelectDistance, Constants.GroundPosition, 0.0f);

        PlayerA.GetComponent<PlayerCS>().Reset(null, null, null, playerCreateInfoA);
        PlayerB.GetComponent<PlayerCS>().Reset(null, null, null, playerCreateInfoB);

        PlayerA.GetComponent<PlayerCS>().SetPause(true);
        PlayerB.GetComponent<PlayerCS>().SetPause(true);

        LayerPortrait.SetActive(true);
        LayerVersus.SetActive(false);

        _timer = 0.0f;
        _challengeState = ChallengeState.None;
    }

    public void PortraitOnClick()
    {
    }

    public void Btn_Fight_OnClick()
    {
        LayerPortrait.SetActive(false);
        LayerVersus.SetActive(true);
        
        _timer = 0.0f;
        _challengeState = ChallengeState.Versus;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MainSceneManager.GetComponent<MainSceneManagerCS>().SetActivateScene(GameSceneType.MainScene);
        }

        if(ChallengeState.Versus == _challengeState)
        {
            if(3.0f < _timer)
            {
                MainSceneManager.GetComponent<MainSceneManagerCS>().SetActivateScene(GameSceneType.FightScene);
                
                playerCreateInfoA._startPosition = new Vector3(-Constants.IdleDistance, Constants.GroundPosition, 0.0f);
                playerCreateInfoB._startPosition = new Vector3(Constants.IdleDistance, Constants.GroundPosition, 0.0f);
                GameManager.GetComponent<GameManagerCS>().ResetGameManager(playerCreateInfoA, playerCreateInfoB);
            }
        }
        _timer += Time.deltaTime;
    }
}
