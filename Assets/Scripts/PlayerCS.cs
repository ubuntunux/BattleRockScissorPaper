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
    Select,
    Idle,
    AttackMotion,
    AttackHit,
    Win,
    Dead
}

public class PlayerCreateInfo
{
    public string _name = "";
    public bool _isPlayer = false;
    public bool _isLeft = true;
    public Vector3 _startPosition = new Vector3(0.0f, 0.0f, 0.0f);
    public PlayerCS _skin = null;
}

public class PlayerStat
{
    public bool _isPlayer = false;    
    public int _skinID = Constants.DefaultSkinID;
    public int _perfect = 0;
    public int _win = 0;
    public int _lose = 0;
    public int _score = 0;
    public int _hp = Constants.DefaultHP;
    public int _power = Constants.DefaultPower;
    public float _speed = Constants.AttackTimerTime;

    public void PrintPlayerStat(string title)
    {
        Debug.Log("=============================");
        Debug.Log(title);
        Debug.Log("_isPlayer: " + _isPlayer.ToString() 
            + ", _skinID: " + _skinID.ToString()
            + ", _perfect: " + _perfect.ToString()
            + ", _win: " + _win.ToString()
            + ", _lose: " + _lose.ToString()
            + ", _score: " + _score.ToString()
            + ", _hp: " + _hp.ToString()
            + ", _power: " + _power.ToString()
            + ", _speed: " + _speed.ToString()
        );
    }

    public void InitializePlayerStat(PlayerCS player, bool isPlayer)
    {
        _isPlayer = isPlayer;
        if(null != player)
        {
            _skinID = player.SkinID;
            _perfect = isPlayer ? 0 : player.Perfect;
            _win = isPlayer ? 0 : player.Win;
            _lose = isPlayer ? 0 : player.Lose;
            _score = isPlayer ? 0 : player.Score;
            _hp = player.HP;
            _power = player.Power;
            _speed = player.Speed;
        }
        else
        {
            _skinID = Constants.DefaultSkinID;
            _perfect = 0;
            _win = 0;
            _lose = 0;
            _score = 0;
            _hp = Constants.DefaultHP;
            _power = Constants.DefaultPower;
            _speed = Constants.AttackTimerTime;
        }
    }

    public void LoadPlayerStat()
    {
        string skinID = _isPlayer ? "" : _skinID.ToString();

        _perfect = SystemValue.GetInt(skinID + SystemValue.PlayerStatPerfectKey, _perfect);
        _win = SystemValue.GetInt(skinID + SystemValue.PlayerStatWinKey, _win);
        _lose = SystemValue.GetInt(skinID + SystemValue.PlayerStatLoseKey, _lose);
        _score = SystemValue.GetInt(skinID + SystemValue.PlayerStatScoreKey, _score);
        _hp = SystemValue.GetInt(skinID + SystemValue.PlayerStatHPKey, _hp);
        _power = SystemValue.GetInt(skinID + SystemValue.PlayerStatPowerKey, _power);
        _speed = SystemValue.GetFloat(skinID + SystemValue.PlayerStatSpeedKey, _speed);
    }

    public void SavePlayerStat()
    {
        string skinID = _isPlayer ? "" : _skinID.ToString();

        SystemValue.SetInt(skinID + SystemValue.PlayerStatPerfectKey, _perfect);
        SystemValue.SetInt(skinID + SystemValue.PlayerStatWinKey, _win);
        SystemValue.SetInt(skinID + SystemValue.PlayerStatLoseKey, _lose);
        SystemValue.SetInt(skinID + SystemValue.PlayerStatScoreKey, _score);
        SystemValue.SetInt(skinID + SystemValue.PlayerStatHPKey, _hp);
        SystemValue.SetInt(skinID + SystemValue.PlayerStatPowerKey, _power);
        SystemValue.SetFloat(skinID + SystemValue.PlayerStatSpeedKey, _speed);
    }
}

public class PlayerCS : MonoBehaviour
{
    // properties
    public string _characterName;
    string _name;
    bool _pause = false;
    bool _isPlayer = false;
    bool _isLeft = false;
    Vector3 _startPosition;
    PlayerState _playerState = PlayerState.None;
    AttackType _lastAttackType = AttackType.None;
    float _elapsedTime = 0.0f;
    float _idleMotionSpeed = 0.0f;
    float _attackMotionTime = 0.0f;
    float _nextAttackMotionTime = 0.0f;
    int _hp = 0;
    int _wins = 0;

    // Stat
    public int Age = 25;
    public int Perfect = 0;
    public int Win = 0;
    public int Lose = 0;
    public int Score = 0;
    public int HP = 3;
    public int Power = 1;
    public float Speed = Constants.AttackTimerTime;
    public PlayerStat _playerStat = new PlayerStat();

    // Skin
    public int SkinID;
    public Sprite Sprite_Born;
    public Sprite Sprite_Portrait;
    public Sprite Sprite_PortraitLose;
    public Sprite Sprite_Idle;
    public Sprite Sprite_AttackRock;
    public Sprite Sprite_AttackScissor;
    public Sprite Sprite_AttackPaper;
    public Sprite Sprite_Win;
    public Sprite Sprite_Dead;

    // sounds
    public AudioSource Snd_Attack;
    public AudioSource Snd_AttackHit;
    public AudioSource Snd_AttackVoice;
    public AudioSource Snd_AttackHitVoice;
    public AudioSource Snd_AttackDeadVoice;
    public AudioSource Snd_Name = null;

    // ui
    GameManagerCS GameManager;
    GameObject Layer_HP_Bar;
    GameObject Layer_AttackTimer;

    ShakeObject _shakeObject = new ShakeObject();

    // Start is called before the first frame update
    void Start()
    {
    }

    public AudioClip GetAudioClip_CharacterName()
    {
        return Snd_Name.clip;
    }

    public void PlayCharacterName()
    {
        Snd_Name.Play();
    }

    public string GetName()
    {
        return _name;
    }

    public string GetCharacterName()
    {
        return _characterName;
    }

     public int GetPower(bool isCounter)
    {
        return Power;
    }

    public Sprite GetImagePortrait()
    {
        return Sprite_Portrait;
    }

    public Sprite GetImagePortraitLose()
    {
        return Sprite_PortraitLose;
    }

    public void SetTexture(Sprite sprite)
    {
        GetComponent<Renderer>().material.SetTexture("_MainTex", sprite.texture);
    }

    public void ResetPlayer(GameManagerCS gameManager, GameObject layer_hp_bar, GameObject layer_attack_timer, PlayerCreateInfo playerCreateInfo)
    {
        _name = playerCreateInfo._name;
        _isPlayer = playerCreateInfo._isPlayer;
        _startPosition = playerCreateInfo._startPosition;
        _isLeft = playerCreateInfo._isLeft;        
        _elapsedTime = 0.0f;        
        _pause = false;

        GameManager = gameManager;
        Layer_AttackTimer = layer_attack_timer;
        Layer_HP_Bar = layer_hp_bar;
        
        if(null != playerCreateInfo._skin)
        {
            SetSkin(playerCreateInfo._skin);
        }
        else
        {
            LoadPlayerStat();
        }

        _wins = 0;
        _hp = _playerStat._hp;

        SetReadyToRound();
    }

    public void InitializePlayerStat()
    {
        _playerStat.InitializePlayerStat(this, _isPlayer);
    }

    public void LoadPlayerStat()
    {
        InitializePlayerStat();
        _playerStat.LoadPlayerStat();
    }

    public void SavePlayerStat()
    {
        _playerStat.SavePlayerStat();
    }

    public void SetPause(bool pause)
    {
        _pause = pause;
    }

    public void SetSkin(PlayerCS skin)
    {
        SkinID = skin.SkinID; 

        // player stat
        Age = skin.Age;
        Win = skin.Win;
        Lose = skin.Lose;
        Score = skin.Score;
        Perfect = skin.Perfect;
        HP = skin.HP;
        Power = skin.Power;
        Speed = skin.Speed;
        
        _characterName = skin._characterName;
        Sprite_Born = skin.Sprite_Born;
        Sprite_Portrait = skin.Sprite_Portrait;
        Sprite_PortraitLose = skin.Sprite_PortraitLose;
        Sprite_Idle = skin.Sprite_Idle;
        Sprite_AttackRock = skin.Sprite_AttackRock;
        Sprite_AttackScissor = skin.Sprite_AttackScissor;
        Sprite_AttackPaper = skin.Sprite_AttackPaper;
        Sprite_Win = skin.Sprite_Win;
        Sprite_Dead = skin.Sprite_Dead;
        Snd_Name.clip = skin.Snd_Name.clip;

        LoadPlayerStat();
    }

    public void SetReadyToRound()
    {
        _hp = _playerStat._hp;
        _playerState = PlayerState.None;
        _lastAttackType = AttackType.None;
        _idleMotionSpeed = 7.0f + Random.insideUnitCircle.x * 0.5f;
        _nextAttackMotionTime = Mathf.Lerp(Constants.AttackRandomTermMin, Constants.AttackRandomTermMax, Random.insideUnitCircle.x);        
        transform.position = _startPosition;
        
        _shakeObject.reset();

        if(null != Layer_HP_Bar)
        {
            Layer_HP_Bar.GetComponent<UIBarCS>().Reset(_name);
        }
        SetTexture(Sprite_Idle);
    }

    public void SetSelect()
    {
        SetTexture(Sprite_Idle);        
        _playerState = PlayerState.Select;
    }

    public void SetIdle()
    {
        SetTexture(Sprite_Idle);        
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

    public int GetHP()
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
        SetTexture(Sprite_Win);
        _playerState = PlayerState.Win;
    }

    public void SetDead()
    {
        SetTexture(Sprite_Dead);
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
                SetTexture(Sprite_AttackRock);
                break;
            case AttackType.Scissor:
                SetTexture(Sprite_AttackScissor);
                break;
            case AttackType.Paper:
                SetTexture(Sprite_AttackPaper);
                break;
            default:
                SetIdle();
                break;
        }

        Layer_AttackTimer.GetComponent<FightTimerCS>().SetAttackType(attackType, _isLeft);

        if(isAttackHit)
        {
            Snd_AttackVoice.Play();
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
        _hp -= damage;

        if(_hp <= 0)
        {
            _hp = 0;
            Snd_AttackDeadVoice.Play();
        }
        else
        {
            Snd_AttackHitVoice.Play();
        }

        Layer_HP_Bar.GetComponent<UIBarCS>().setBar((float)_hp / _playerStat._hp);
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
    public void Update()
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
        else if(PlayerState.Select == _playerState)
        {
            float speed = _elapsedTime * _idleMotionSpeed;
            float offsetY = Mathf.Abs(Mathf.Cos(speed)) * 0.25f;
            transform.position = _startPosition + new Vector3(_isLeft ? 1.0f : -1.0f, offsetY, -1.0f);
        }
        else if(PlayerState.None == _playerState || PlayerState.Idle == _playerState)
        {
            // update Idle
            float speed = _elapsedTime * _idleMotionSpeed;
            float offsetX = Mathf.Sin(speed) * 0.4f;
            float offsetY = Mathf.Abs(Mathf.Cos(speed)) * 0.25f;
            transform.position = _startPosition + new Vector3(_isLeft ? offsetX : -offsetX, offsetY, 0.0f);

            // Set NPC random attack
            if(false == _isPlayer && PlayerState.Idle == _playerState)
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
