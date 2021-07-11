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
    AttackHit
}

public class PlayerCS : MonoBehaviour
{
    // properties
    bool _isLeft;
    Vector3 _startPosition;
    PlayerState _playerState = PlayerState.None;
    AttackType _lastAttackType = AttackType.None;
    float _elapsedTime = 0.0f;
    float _idleMotionSpeed = 0.0f;
    float _attackMotionTime = 0.0f;

    // textures
    public Texture Texture_Idle;
    public Texture Texture_AttackRock;
    public Texture Texture_AttackScissor;
    public Texture Texture_AttackPaper;

    // sounds
    public AudioSource Snd_Attack;
    public AudioSource Snd_AttackHit;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetTexture(Texture texture)
    {
        GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
    }

    public void Reset(bool isLeft)
    {
        _isLeft = isLeft;
        _elapsedTime = 0.0f;
        _playerState = PlayerState.Idle;
        _lastAttackType = AttackType.None;
        _idleMotionSpeed = 7.0f + Random.insideUnitCircle.x * 0.5f;
        
        float offsetX = 2.0f;
        float offsetY = 3.0f;

        if(isLeft)
        {
            offsetX = -offsetX;
        }

        _startPosition = new Vector3(offsetX, offsetY, 0.0f);
        transform.position = _startPosition;

        SetTexture(Texture_Idle);
    }

    public void SetIdle()
    {
        _playerState = PlayerState.Idle;
        SetTexture(Texture_Idle);
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
        SetAttackInner(attackType, false);
    }

    public void SetAttackHit()
    {
        if(_lastAttackType == AttackType.None)
        {
            _lastAttackType = (AttackType)(Random.Range(0, 3) + 1);
        }

        SetAttackInner(_lastAttackType, true);
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerState.Idle == _playerState)
        {
            float speed = _elapsedTime * _idleMotionSpeed;
            float offsetX = Mathf.Sin(speed) * 0.4f;
            float offsetY = Mathf.Abs(Mathf.Cos(speed)) * 0.25f;
            transform.position = _startPosition + new Vector3(_isLeft ? offsetX : -offsetX, offsetY, 0.0f);
        }
        else if(PlayerState.AttackMotion == _playerState || PlayerState.AttackHit == _playerState)
        {
            if(PlayerState.AttackMotion == _playerState)
            {
                if (Constants.AttackMotionTime <= _attackMotionTime)
                {
                    SetIdle();
                }
            }
            else if(PlayerState.AttackHit == _playerState)
            {
                if (Constants.AttackHitTime <= _attackMotionTime)
                {
                    _lastAttackType = AttackType.None;
                    SetIdle();
                }
            }
            _attackMotionTime += Time.deltaTime;
        }
        _elapsedTime += Time.deltaTime;
    }
}
