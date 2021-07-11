using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    None,
    ReadyToRound,
    ReadyToAttack,
    Attack,
    End,
}

public enum AttackType
{
    Rock,
    Scissor,
    Paper
}

public class GameManagerCS : MonoBehaviour
{
    // constants
    public float kAttackTimerTime = 3.0f;
    public float kAttackTime = 1.0f;

    // Properties
    float _elapsedTime = 0.0f;
    float _attackTimerTime = 0.0f;
    float _attackTime = 0.0f;
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
        _attackTime = 0.0f;

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
        _attackTimerTime = 0.0f;
        _attackTime = 0.0f;
        _gameState = GameState.ReadyToAttack;
		Snd_Fight.Play();
        Layer_AttackButtons.SetActive(true);
        Layer_AttackTimer.SetActive(true);
        Btn_Fight.SetActive(false);
	}

    public void Btn_Rock_OnClick() {
        Attack(AttackType.Rock);
	}

    public void Btn_Scissor_OnClick() {
        Attack(AttackType.Scissor);
	}

    public void Btn_Paper_OnClick() {
        Attack(AttackType.Paper);
	}

    public void Attack(AttackType attackType)
    {
        PlayerA_CS.SetAttack(attackType);
	}

    public void AttackHit(AttackType attackType)
    {
		PlayerA_CS.SetAttackHit(attackType);
        MainCamera.GetComponent<CameraCS>().setShake();
	}

     // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if(GameState.ReadyToAttack == _gameState || GameState.Attack == _gameState)
        {
            bool doAttack = false;

            if(GameState.ReadyToAttack == _gameState)
            {
                float attackTimerBar = 1.0f - ((_attackTimerTime % kAttackTimerTime) / kAttackTimerTime);
                AttackTimer_CS.setTimer(attackTimerBar);

                if(kAttackTimerTime <= _attackTimerTime)
                {
                    doAttack = true;
                    _attackTime = 0.0f;
                    _attackTimerTime = 0.0f;                    
                    AttackTimer_CS.setTimer(0.0f);
                    _gameState = GameState.Attack;
                }
                else
                {
                    _attackTimerTime += Time.deltaTime;
                }
            }

            if(GameState.Attack == _gameState)
            {
                if(doAttack)
                {
                    Attack(AttackType.Paper);
                }
                
                if(kAttackTime <= _attackTime)
                {
                    _attackTime = 0.0f;
                    _attackTimerTime = 0.0f;
                    _gameState = GameState.ReadyToAttack;
                }
                else
                {
                    _attackTime += Time.deltaTime;
                }
            }
        }
        _elapsedTime += Time.deltaTime;
    }
}
