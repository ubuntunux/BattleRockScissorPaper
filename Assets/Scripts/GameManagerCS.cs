using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    float _attackHitTimeDelay = 0.0f;
    GameState _gameState = GameState.None;

    //
    public GameObject MainCamera;

    // Player
    public GameObject PlayerA;
    public GameObject PlayerB;
    PlayerCS PlayerA_CS;
    PlayerCS PlayerB_CS;

    // Effect
    public GameObject Effect_AttackHit;
    GameObject _effect_AttackHitA;
    GameObject _effect_AttackHitB;

    // Sounds
    public AudioSource Snd_Round1;
    public AudioSource Snd_Round2;
    public AudioSource Snd_Round3;
    public AudioSource Snd_Round4;
    public AudioSource Snd_FinalRound;
    public AudioSource Snd_Fight;
    public AudioSource Snd_Win;
    public AudioSource Snd_Loose;
    public AudioSource Snd_Draw;

    // UI
    public GameObject Btn_Fight;
    public GameObject Layer_AttackButtons;
    public GameObject Btn_Rock;
    public GameObject Btn_Scissor;
    public GameObject Btn_Paper;
    public GameObject Layer_AttackTimer;
    FightTimerCS AttackTimer_CS;
    public GameObject Layer_HP_Bar_A;
    public GameObject Layer_HP_Bar_B;
    public GameObject Text_Result;
    public GameObject Image_KO;
    public Texture Texture_Ko;
    public Texture Texture_Ko_Invert;
    
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
        _attackHitTimeDelay = 0.0f;

        PlayerA_CS = PlayerA.GetComponent<PlayerCS>();
        PlayerB_CS = PlayerB.GetComponent<PlayerCS>();
        PlayerA_CS.Reset(Layer_HP_Bar_A, true, false);
        PlayerB_CS.Reset(Layer_HP_Bar_B, false, true);

        // Effects
        DestroyEffectAttackHit(true);
        DestroyEffectAttackHit(false);
        // UI        
        AttackTimer_CS = Layer_AttackTimer.GetComponent<FightTimerCS>();
        AttackTimer_CS.Reset();
        Text_Result.SetActive(false);
    }

	public void CreateEffectAttackHit(AttackType attackType, bool isLeft)
    {
        DestroyEffectAttackHit(isLeft);

        float offsetY = Constants.GroundPosition;
        if(AttackType.Rock == attackType)
        {
            offsetY = 2.0f;
        }
        else if(AttackType.Scissor == attackType)
        {
            offsetY = 1.0f;
        }
        else if(AttackType.Paper == attackType)
        {
            offsetY = 3.0f;
        }

        Vector3 pos = new Vector3(isLeft ? -Constants.AttackDistance : Constants.AttackDistance, offsetY, 0.0f);
        Quaternion rot = Quaternion.Euler(-90, 0, 0);
		GameObject effect_AttackHit = (GameObject)GameObject.Instantiate(Effect_AttackHit, pos, rot);
        if(isLeft)
        {
            effect_AttackHit.transform.localScale = new Vector3(-effect_AttackHit.transform.localScale.x, effect_AttackHit.transform.localScale.y, effect_AttackHit.transform.localScale.z);
            _effect_AttackHitA = effect_AttackHit;
        }
        else
        {   
            _effect_AttackHitB = effect_AttackHit;
        }
	}

    public void DestroyEffectAttackHit(bool isLeft)
    {
        if(isLeft)
        {
            if(null != _effect_AttackHitA) 
            {
                Destroy(_effect_AttackHitA);
                _effect_AttackHitA = null;
            }
        }
        else
        {
            if(null != _effect_AttackHitB) 
            {
                Destroy(_effect_AttackHitB);
                _effect_AttackHitB = null;
            }
        }
    }

    public void Btn_Fight_OnClick() {        
        Reset();        
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

    bool checkLoose(AttackType lhs, AttackType rhs)
    {
        switch (rhs)
        {
            case AttackType.Rock:
                return AttackType.Paper == lhs;
            case AttackType.Scissor:
                return AttackType.Rock == lhs;
            case AttackType.Paper:
                return AttackType.Scissor == lhs;
        }
        return false;
    }

    bool checkWin(AttackType lhs, AttackType rhs)
    {
        switch (rhs)
        {
            case AttackType.Rock:
                return AttackType.Scissor == lhs;
            case AttackType.Scissor:
                return AttackType.Paper == lhs;
            case AttackType.Paper:
                return AttackType.Rock == lhs;
        }
        return false;
    }

    // Set State
    void SetReadyToAttack()
    {
        PlayerA_CS.SetReadyToAttack();
        PlayerB_CS.SetReadyToAttack();
        _attackTimerTime = 0.0f;
        _gameState = GameState.ReadyToAttack;
    }

    void SetEnd()
    {
        Btn_Fight.SetActive(true);
        _gameState = GameState.End;
    }

    void SetAttackHit()
    {
        AttackTimer_CS.setBar(0.0f);
        _attackHitTime = 0.0f;                
        _gameState = GameState.AttackHit;
		PlayerA_CS.SetAttackHit();
        PlayerB_CS.SetAttackHit();

        AttackType attackTypeA = PlayerA_CS.getLastAttackType();
        AttackType attackTypeB = PlayerB_CS.getLastAttackType();

        if(attackTypeB == attackTypeA || checkLoose(attackTypeB, attackTypeA))
        {
            CreateEffectAttackHit(attackTypeB, true);
            PlayerA_CS.SetDamage((attackTypeB == attackTypeA) ? Constants.DamageDraw : Constants.DamageLoose);
        }

        if(attackTypeA == attackTypeB || checkLoose(attackTypeA, attackTypeB))
        {
            CreateEffectAttackHit(attackTypeA, false);
            PlayerB_CS.SetDamage((attackTypeA == attackTypeB) ? Constants.DamageDraw : Constants.DamageLoose);
        }

        // delay
        _attackHitTimeDelay = (false == PlayerA_CS.isAlive() || false == PlayerB_CS.isAlive()) ? Constants.AttackHitTimeDelay : 0.0f;

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
            AttackTimer_CS.setBar(attackTimerBar);
            if(Constants.AttackTimerTime <= _attackTimerTime)
            {
                SetAttackHit();
            }
            _attackTimerTime += Time.deltaTime;
        }
        else if(GameState.AttackHit == _gameState)
        {
            if((Constants.AttackHitTime + _attackHitTimeDelay) <= _attackHitTime)
            {
                bool isAliveA = PlayerA_CS.isAlive();
                bool isAliveB = PlayerB_CS.isAlive();
                if (false == isAliveA || false == isAliveB)
                {
                    Text_Result.SetActive(true);
                    if(isAliveA && false == isAliveB)
                    {
                        PlayerA_CS.SetWin();
                        PlayerB_CS.SetDead();
                        Snd_Win.Play();
                        Text_Result.GetComponent<Text>().text = "You Win";
                    }
                    else if(false == isAliveA && isAliveB)
                    {
                        PlayerA_CS.SetDead();
                        PlayerB_CS.SetWin();
                        Snd_Loose.Play();
                        Text_Result.GetComponent<Text>().text = "You Loose";
                    }
                    else
                    {
                        PlayerA_CS.SetDead();
                        PlayerB_CS.SetDead();
                        Snd_Draw.Play();
                        Text_Result.GetComponent<Text>().text = "Draw";
                    }
                    SetEnd();
                }
                else
                {
                    SetReadyToAttack();
                }
            }
            _attackHitTime += Time.deltaTime;
        }
        if(GameState.End == _gameState)
        {
        }

        if(null != _effect_AttackHitA && false == _effect_AttackHitA.GetComponent<ParticleSystem>().isPlaying)
        {
			DestroyEffectAttackHit(true);
		}

        if(null != _effect_AttackHitB && false == _effect_AttackHitB.GetComponent<ParticleSystem>().isPlaying)
        {
			DestroyEffectAttackHit(false);
		}

        _elapsedTime += Time.deltaTime;
    }
}
