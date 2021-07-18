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

public class PlayerCS : MonoBehaviour
{
    // properties
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
    GameObject Layer_HP_Bar;
    UIBarCS HP_Bar_CS;

    ShakeObject _shakeObject = new ShakeObject();

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetTexture(Texture texture)
    {
        GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
    }

    public void Reset(GameObject layer_hp_bar, bool isLeft, bool isNPC)
    {
        _isNPC = isNPC;
        _isLeft = isLeft;
        _elapsedTime = 0.0f;
        _wins = 0;

        Layer_HP_Bar = layer_hp_bar;
        HP_Bar_CS = Layer_HP_Bar.GetComponent<UIBarCS>();
        
        _startPosition = new Vector3(isLeft ? -Constants.IdleDistance : Constants.IdleDistance, Constants.GroundPosition, 0.0f);

        SetReadyToRound();
    }

    public void SetReadyToRound()
    {
        _playerState = PlayerState.None;
        _lastAttackType = AttackType.None;
        _idleMotionSpeed = 7.0f + Random.insideUnitCircle.x * 0.5f;
        _nextAttackMotionTime = Random.insideUnitCircle.x * Constants.AttackTimerTime;
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

        if(isAttackHit)
        {
            _playerState = PlayerState.AttackHit;
            Snd_AttackHit.Play();
        }
        else
        {
            _playerState = PlayerState.AttackMotion;
            _lastAttackType = attackType;
            Snd_Attack.Play();
        }
    }

    public void SetAttack(AttackType attackType)
    {
        if (false == isAlive() || PlayerState.AttackHit == _playerState)
        {
            return;
        }
        
        if(attackType == AttackType.None)
        {
            attackType = (AttackType)(Random.Range(0, 3) + 1);
        }
        SetAttackInner(attackType, false);
    }

    public void SetAttackHit()
    {
        if (false == isAlive() || PlayerState.AttackHit == _playerState)
        {
            return;
        }

        if(_lastAttackType == AttackType.None)
        {
            _lastAttackType = (AttackType)(Random.Range(0, 3) + 1);
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
        _shakeObject.setShake(Constants.AttackHitTime, 0.5f, 0.01f);
    }

    // Update is called once per frame
    void Update()
    {
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
            // Idle
            float speed = _elapsedTime * _idleMotionSpeed;
            float offsetX = Mathf.Sin(speed) * 0.4f;
            float offsetY = Mathf.Abs(Mathf.Cos(speed)) * 0.25f;
            transform.position = _startPosition + new Vector3(_isLeft ? offsetX : -offsetX, offsetY, 0.0f);

            if(_isNPC && PlayerState.Idle == _playerState)
            {
                if(_nextAttackMotionTime < 0.0f)
                {
                    SetAttack(AttackType.None);
                    _nextAttackMotionTime = Random.insideUnitCircle.x * Constants.AttackTimerTime;
                }
                _nextAttackMotionTime -= Time.deltaTime;
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
