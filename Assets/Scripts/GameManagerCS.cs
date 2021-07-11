using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    None,
    ReadyToRound,
    ReadyToAttack,
    AttackHit,
    End,
}

public class GameManagerCS : MonoBehaviour
{
    // Properties
    float _elapsedTime = 0.0f;
    float _attackTimerTime = 0.0f;
    float _attackHitTime = 0.0f;
    GameState _gameState = GameState.None;

    //
    public GameObject MainCamera;
    // Player
    public GameObject PlayerA;
    public GameObject PlayerB;
    PlayerCS PlayerA_CS;
    PlayerCS PlayerB_CS;
    // Sounds
    public AudioSource Snd_Fight;
    // UI
    public GameObject Btn_Fight;
    public GameObject Layer_AttackButtons;
    public GameObject Btn_Rock;
    public GameObject Btn_Scissor;
    public GameObject Btn_Paper;
    public GameObject Layer_AttackTimer;
    AttackTimerCS AttackTimer_CS;
    
    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    void Reset()
    {
        _gameState = GameState.ReadyToRound;
        _elapsedTime = 0.0f;
        _attackTimerTime = 0.0f;
        _attackHitTime = 0.0f;

        PlayerA_CS = PlayerA.GetComponent<PlayerCS>();
        PlayerB_CS = PlayerB.GetComponent<PlayerCS>();
        PlayerA_CS.Reset(true);
        PlayerB_CS.Reset(false);

        // UI        
        AttackTimer_CS = Layer_AttackTimer.GetComponent<AttackTimerCS>();
        AttackTimer_CS.Reset();

        // first update
        Layer_AttackTimer.SetActive(false);
        Layer_AttackButtons.SetActive(false);
    }

    public void Btn_Fight_OnClick() {
        Layer_AttackButtons.SetActive(true);
        Layer_AttackTimer.SetActive(true);
        Btn_Fight.SetActive(false);
        Snd_Fight.Play();
        SetReadyToAttack();
	}

    void Btn_Rock_OnClick() {
        Attack(AttackType.Rock);
	}

    void Btn_Scissor_OnClick() {
        Attack(AttackType.Scissor);
	}

    void Btn_Paper_OnClick() {
        Attack(AttackType.Paper);
	}

    void Attack(AttackType attackType)
    {
        PlayerA_CS.SetAttack(attackType);
	}

    // Set State
    void SetReadyToAttack()
    {
        _attackTimerTime = 0.0f;
        _gameState = GameState.ReadyToAttack;
    }

    void SetAttackHit()
    {
        AttackTimer_CS.setTimer(0.0f);
        _attackHitTime = 0.0f;                
        _gameState = GameState.AttackHit;
		PlayerA_CS.SetAttackHit();
        MainCamera.GetComponent<CameraCS>().setShake();
	}

     // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if(GameState.ReadyToAttack == _gameState)
        {
            float attackTimerBar = 1.0f - ((_attackTimerTime % Constants.AttackTimerTime) / Constants.AttackTimerTime);
            AttackTimer_CS.setTimer(attackTimerBar);
            if(Constants.AttackTimerTime <= _attackTimerTime)
            {
                SetAttackHit();
            }
            _attackTimerTime += Time.deltaTime;
        }
        else if(GameState.AttackHit == _gameState)
        {
            if(Constants.AttackHitTime <= _attackHitTime)
            {
                SetReadyToAttack();
            }
            _attackHitTime += Time.deltaTime;
        }
        _elapsedTime += Time.deltaTime;
    }
}
