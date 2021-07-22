using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    None,
    Rock,
    Scissor,
    Paper
}

public enum PlayerState
{
    None,
    Idle,
    AttackMotion,
    AttackHit,
    Win,
    Dead
}

public class PlayerCreateInfo
{
    public string _name = "";
    public bool _isNPC = false;
}

public class PlayerCS : MonoBehaviour
{
    // properties
    string _name;
    bool _pause = false;
    bool _isNPC = false;
    bool _isLeft = false;
    Vector3 _startPosition;
    PlayerState _playerState = PlayerState.None;
    AttackType _lastAttackType = AttackType.None;
    float _elapsedTime = 0.0f;
    float _idleMotionSpeed = 0.0f;
    float _attackMotionTime = 0.0f;
    float _nextAttackMotionTime = 0.0f;
    int _hp = Constants.InitialHP;
    int _wins = 0;

    // textures
    public Texture Texture_Portrait;
    public Texture Texture_Idle;
    public Texture Texture_AttackRock;
    public Texture Texture_AttackScissor;
    public Texture Texture_AttackPaper;
    public Texture Texture_Win;
    public Texture Texture_Dead;

    // sounds
    public AudioSource Snd_Attack;
    public AudioSource Snd_AttackHit;

    // ui
    GameManagerCS GameManager;
    GameObject Layer_HP_Bar;
    UIBarCS HP_Bar_CS;
    GameObject Layer_AttackTimer;

    ShakeObject _shakeObject = new ShakeObject();

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetTexture(Texture texture)
    {
        GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
    }

    public void Reset(GameManagerCS gameManager, GameObject layer_hp_bar, GameObject layer_attack_timer, bool isLeft, PlayerCreateInfo playerCreateInfo)
    {
        _name = playerCreateInfo._name;
        _isNPC = playerCreateInfo._isNPC;
        _isLeft = isLeft;
        _elapsedTime = 0.0f;
        _wins = 0;
        _pause = false;

        GameManager = gameManager;
        Layer_AttackTimer = layer_attack_timer;
        Layer_HP_Bar = layer_hp_bar;
        HP_Bar_CS = Layer_HP_Bar.GetComponent<UIBarCS>();
        
        _startPosition = new Vector3(isLeft ? -Constants.IdleDistance : Constants.IdleDistance, Constants.GroundPosition, 0.0f);

        SetReadyToRound();
    }

    public void SetPause(bool pause)
    {
        _pause = pause;
    }

    public void SetReadyToRound()
    {
        _playerState = PlayerState.None;
        _lastAttackType = AttackType.None;
        _idleMotionSpeed = 7.0f + Random.insideUnitCircle.x * 0.5f;
        _nextAttackMotionTime = Mathf.Lerp(Constants.AttackRandomTermMin, Constants.AttackRandomTermMax, Random.insideUnitCircle.x);
        _hp = Constants.InitialHP;
        transform.position = _startPosition;
        _shakeObject.reset();
        HP_Bar_CS.Reset();
        SetTexture(Texture_Idle);
    }

    public void SetIdle()
    {
        SetTexture(Texture_Idle);        
        _playerState = PlayerState.Idle;
    }

    public void SetReadyToAttack()
    {
        _lastAttackType = AttackType.None;
        SetIdle();
    }

    public bool isAlive()
    {
        return (0 < _hp);
    }

    public int getHP()
    {
        return _hp;
    }

    public int GetWin()
    {
        return _wins;
    }

    public void SetWin()
    {
        _wins += 1;
        SetTexture(Texture_Win);
        _playerState = PlayerState.Win;
    }

    public void SetDead()
    {
        SetTexture(Texture_Dead);
        _playerState = PlayerState.Dead;
    }

    public AttackType getLastAttackType()
    {
        return _lastAttackType;
    }

    public void SetAttackInner(AttackType attackType, bool isAttackHit)
    {
        _attackMotionTime = 0.0f;

        switch(attackType)
        {
            case AttackType.Rock:
                SetTexture(Texture_AttackRock);
                break;
            case AttackType.Scissor:
                SetTexture(Texture_AttackScissor);
                break;
            case AttackType.Paper:
                SetTexture(Texture_AttackPaper);
                break;
            default:
                SetIdle();
                break;
        }

        Layer_AttackTimer.GetComponent<FightTimerCS>().SetAttackType(attackType, _isLeft);

        if(isAttackHit)
        {
            Snd_AttackHit.Play();
            _playerState = PlayerState.AttackHit;
        }
        else
        {
            Snd_Attack.Play();
            _lastAttackType = attackType;
            _playerState = PlayerState.AttackMotion;
        }
    }

    public void SetAttack(AttackType attackType)
    {
        if (false == isAlive() || PlayerState.AttackHit == _playerState)
        {
            return;
        }

        SetAttackInner(attackType, false);
    }

    public void SetAttackHit()
    {
        if (false == isAlive() || PlayerState.AttackHit == _playerState)
        {
            return;
        }

        SetAttackInner(_lastAttackType, true);
    }

    public void SetDamage(int damage)
    {
        // damage = 50;

        _hp -= damage;
        if(_hp <= 0)
        {
            _hp = 0;
        }

        HP_Bar_CS.setBar((float)_hp / Constants.InitialHP);
        Layer_AttackTimer.GetComponent<FightTimerCS>().SetAttackTimerShake(_isLeft);
        _shakeObject.setShake(Constants.AttackHitTime, 0.5f, 0.01f);        
    }

    void updateNPC()
    {
        float attackTimerTime = GameManager.GetAttackTimerTime();
        float RANDOM_TIME_LIMIT = 0.0f;
        bool isOverRandomTime = (attackTimerTime <= RANDOM_TIME_LIMIT && RANDOM_TIME_LIMIT <= (attackTimerTime + Time.deltaTime));
        if(_nextAttackMotionTime < 0.0f || isOverRandomTime)
        {
            AttackType attackType = (attackTimerTime <= RANDOM_TIME_LIMIT) ? AttackType.None : _lastAttackType;
            if(AttackType.None == attackType)
            {
                attackType = (AttackType)(Random.Range(0, 3) + 1);
            }
            SetAttack(attackType);
            _nextAttackMotionTime = Mathf.Lerp(Constants.AttackRandomTermMin, Constants.AttackRandomTermMax, Random.insideUnitCircle.x);
        }
        _nextAttackMotionTime -= Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(_pause)
        {
            return;
        }

        if(PlayerState.Dead == _playerState)
        {
            // Nothing
        }
        else if(PlayerState.Win == _playerState)
        {
            // Nothing
        }
        else if(PlayerState.None == _playerState || PlayerState.Idle == _playerState)
        {
            // update Idle
            float speed = _elapsedTime * _idleMotionSpeed;
            float offsetX = Mathf.Sin(speed) * 0.4f;
            float offsetY = Mathf.Abs(Mathf.Cos(speed)) * 0.25f;
            transform.position = _startPosition + new Vector3(_isLeft ? offsetX : -offsetX, offsetY, 0.0f);

            // Set NPC random attack
            if(_isNPC && PlayerState.Idle == _playerState)
            {
                updateNPC();
            }
        }
        else if(PlayerState.AttackMotion == _playerState || PlayerState.AttackHit == _playerState)
        {
            // Attack
            if(PlayerState.AttackMotion == _playerState)
            {
                if (Constants.AttackMotionTime <= _attackMotionTime)
                {
                    SetIdle();
                }
            }
            else if(PlayerState.AttackHit == _playerState)
            {
                Vector3 shakeOffset = new Vector3(0.0f, 0.0f, 0.0f);
                _shakeObject.updateShakeObject(ref shakeOffset);

                transform.position = new Vector3(_isLeft ? -Constants.AttackDistance : Constants.AttackDistance, Constants.GroundPosition, 0.0f);
                transform.position += shakeOffset;
            }
            _attackMotionTime += Time.deltaTime;
        }
        _elapsedTime += Time.deltaTime;
    }
}
