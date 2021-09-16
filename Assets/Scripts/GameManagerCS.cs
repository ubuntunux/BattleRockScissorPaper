using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum GameState
{
    None,
    ReadyToFight,
    ReadyToRound,
    ReadyToAttack,
    AttackHit,
    Groggy,
    RoundEnd,
    GameResult,
}

public class GameManagerCS : MonoBehaviour
{
    // Properties    
    float _elapsedTime = 0.0f;
    float _initialAttackTimerTime = Constants.AttackTimerTime;
    float _attackTimerTime = 0.0f;
    float _attackHitTime = 0.0f;
    float _readyToRoundTime = 0.0f;
    float _groggyTime = 0.0f;
    float _roundEndTime = 0.0f;
    float _gameResultTime = 0.0f;
    int _maxRoundCount = 3;
    int _maxWinCount = 2;
    int _round = 1;
    bool _pause = false;
    GameState _gameState = GameState.None;

    // Record
    int _recordAttackPoint = 0;
    int _recordHP = 0;

    //
    public GameObject MainCamera;
    public GameObject MainSceneManager;
    public GameObject ChallengeSceneManager;
    
    // Player
    public GameObject PlayerA;
    public GameObject PlayerB;
    PlayerCS PlayerA_CS;
    PlayerCS PlayerB_CS;

    // Effect
    public GameObject Effect_AttackHit;
    GameObject _effect_AttackHitA;
    GameObject _effect_AttackHitB;
    public GameObject Image_Bam_A;
    public GameObject Image_Bam_B;
    public GameObject Image_Critical_A;
    public GameObject Image_Critical_B;

    // Sounds
    public AudioSource Snd_Round1;
    public AudioSource Snd_Round2;
    public AudioSource Snd_Round3;
    public AudioSource Snd_Round4;
    public AudioSource Snd_FinalRound;
    public AudioSource Snd_Fight;
    public AudioSource Snd_Win;
    public AudioSource Snd_Lose;
    public AudioSource Snd_Draw;
    public AudioSource Snd_Finish;
    public AudioSource Snd_Bell_1;
    public AudioSource Snd_Bell_2;
    public AudioSource Snd_Bell_3;

    // UI
    public GameObject LayerResult;
    public GameObject Image_Exit;
    public GameObject Layer_AttackButtonsA;
    public GameObject Layer_AttackButtonsB;
    public GameObject Layer_AttackTimer;
    FightTimerCS AttackTimer_CS;
    public GameObject Layer_HP_Bar_A;
    public GameObject Layer_HP_Bar_B;
    public GameObject Text_Result;
    public GameObject Image_KO;    
    public Sprite Sprite_Ko;
    public Sprite Sprite_Ko_Invert;
    float _ko_sprite_flicker_time = 0.0f;
    bool _ko_sprite_flicker = false;
    bool _ko_sprite_flicker_low_hp = false;
    public GameObject Layer_Wins;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
    }

    public void Exit()
    {   
        SetPause(false);
        MainSceneManager.GetComponent<MainSceneManagerCS>().SetActivateScene(GameSceneType.ChallenegeScene);
    }

    public bool CheckGameState(GameState gameState)
    {
        return _gameState == gameState;
    }

    public void ResetGameManager(PlayerCreateInfo playerCreateInfoA, PlayerCreateInfo playerCreateInfoB)
    {
        _gameState = GameState.ReadyToFight;
        _elapsedTime = 0.0f;
        _round = 1;

        // record
        _recordAttackPoint = 0;
        _recordHP = 0;

        for(int i=0; i < 3; ++i)
        {
            Layer_Wins.transform.Find("WinA" + i.ToString()).gameObject.SetActive(false);
            Layer_Wins.transform.Find("WinB" + i.ToString()).gameObject.SetActive(false);
        }

        PlayerA_CS = PlayerA.GetComponent<PlayerCS>();
        PlayerB_CS = PlayerB.GetComponent<PlayerCS>();
        AttackTimer_CS = Layer_AttackTimer.GetComponent<FightTimerCS>();
        Image_Exit.SetActive(false);

        PlayerA_CS.ResetPlayer(GetComponent<GameManagerCS>(), Layer_HP_Bar_A, Layer_AttackTimer, playerCreateInfoA);
        PlayerB_CS.ResetPlayer(GetComponent<GameManagerCS>(), Layer_HP_Bar_B, Layer_AttackTimer, playerCreateInfoB);

        _initialAttackTimerTime = Mathf.Min(PlayerA_CS._playerStat._speed, PlayerB_CS._playerStat._speed);

        Layer_AttackButtonsB.SetActive(playerCreateInfoB._isPlayer);
        LayerResult.SetActive(false);

        SetReadyToRound();
        SetPause(false);
    }

    public float GetAttackTimerTime()
    {
        return _attackTimerTime;
    }

    public Texture GetTexture(GameObject obj)
    {
        return obj.GetComponent<Renderer>().material.GetTexture("_MainTex");
    }

    public void SetTexture(GameObject obj, Texture texture)
    {
        obj.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
    }

    public Sprite GetSprite(GameObject obj)
    {
        return obj.GetComponent<Image>().sprite;
    }

    public void SetSprite(GameObject obj, Sprite sprite)
    {
        obj.GetComponent<Image>().sprite = sprite;
    }

	public void CreateEffectAttackHit(AttackType attackType, bool isPlayerA)
    {
        DestroyEffectAttackHit(isPlayerA);

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

        Vector3 pos = new Vector3(isPlayerA ? -Constants.AttackDistance : Constants.AttackDistance, offsetY, 0.0f);
        Quaternion rot = Quaternion.Euler(-90.0f, isPlayerA ? 180.0f : 0.0f, 0.0f);
		GameObject effect_AttackHit = (GameObject)GameObject.Instantiate(Effect_AttackHit, pos, rot);

        if(isPlayerA)
        {
            _effect_AttackHitA = effect_AttackHit;
        }
        else
        {   
            _effect_AttackHitB = effect_AttackHit;
        }
	}

    public void DestroyEffectAttackHit(bool isPlayerA)
    {
        if(isPlayerA)
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

    public void Btn_Back_OnClick()
    {
        SetPause(!_pause);
    }

    public void Btn_PlayerA_Rock_OnClick() {
        Attack(PlayerA_CS, AttackType.Rock);
	}

    public void Btn_PlayerA_Scissor_OnClick() {
        Attack(PlayerA_CS, AttackType.Scissor);
	}

    public void Btn_PlayerA_Paper_OnClick() {
        Attack(PlayerA_CS, AttackType.Paper);
	}

    public void Btn_PlayerB_Rock_OnClick() {
        Attack(PlayerB_CS, AttackType.Rock);
	}

    public void Btn_PlayerB_Scissor_OnClick() {
        Attack(PlayerB_CS, AttackType.Scissor);
	}

    public void Btn_PlayerB_Paper_OnClick() {
        Attack(PlayerB_CS, AttackType.Paper);
	}

    void Attack(PlayerCS Player, AttackType attackType)
    {
        if(GameState.Groggy == _gameState)
        {
            Player.SetAttackHit(attackType);
        }
        else
        {
            Player.SetAttack(attackType);
        }
	}

    bool checkLose(AttackType lhs, AttackType rhs)
    {
        switch (rhs)
        {
            case AttackType.None:
                return true;
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
            case AttackType.None:
                return true;
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
    void ResetRound()
    {
        _attackTimerTime = 0.0f;
        _attackHitTime = 0.0f;
        _readyToRoundTime = 0.0f;
        _roundEndTime = 0.0f;
        _gameResultTime = 0.0f;

        PlayerA_CS.SetReadyToRound();
        PlayerB_CS.SetReadyToRound();

        DestroyEffectAttackHit(true);
        DestroyEffectAttackHit(false);

        Layer_AttackTimer.SetActive(false);
        AttackTimer_CS.ResetFightTimer();
        Text_Result.SetActive(false);
        Image_Bam_A.SetActive(false);
        Image_Bam_B.SetActive(false);
        Image_Critical_A.SetActive(false);
        Image_Critical_B.SetActive(false);
    }

    void SetReadyToRound()
    {
        ResetRound();
        
        if(_maxRoundCount == _round) { Snd_FinalRound.Play(); }
        else if (1 == _round) { Snd_Round1.Play(); }
        else if (2 == _round) { Snd_Round2.Play(); }
        else if (3 == _round) { Snd_Round3.Play(); }
        else if (4 == _round) { Snd_Round4.Play(); }

        Text_Result.SetActive(true);
        Text_Result.GetComponent<TextMeshProUGUI>().text = (_maxRoundCount == _round) ? "Final Round" : ("Round " + _round.ToString());

        _gameState = GameState.ReadyToRound;
    }

    void RoundEnd()
    {
        Layer_AttackTimer.SetActive(false);        
        _ko_sprite_flicker = false;
        _ko_sprite_flicker_low_hp = false;
        SetSprite(Image_KO, Sprite_Ko);
    }

    void SetRoundEnd()
    {
        RoundEnd();
        _roundEndTime = 0.0f;
        _gameState = GameState.RoundEnd;
    }

    void SetGameResult()
    {
        LayerResult.SetActive(true);
        LayerResult.GetComponent<ResultCS>().Reset(PlayerA_CS, PlayerA_CS.GetWin(), PlayerB_CS, PlayerB_CS.GetWin());
        _gameResultTime = 0.0f;
        _gameState = GameState.GameResult;
    }

    void SetReadyToAttack()
    {
        _ko_sprite_flicker = false;
        SetSprite(Image_KO, Sprite_Ko);
        Layer_AttackTimer.SetActive(true);
        AttackTimer_CS.ResetFightTimer();
        PlayerA_CS.SetReadyToAttack();
        PlayerB_CS.SetReadyToAttack();
        _attackTimerTime = 0.0f;
        _gameState = GameState.ReadyToAttack;
    }

    void SetGroggyState()
    {
        if(PlayerA_CS.isGroggyHP())
        {
            PlayerA_CS.SetGroggyState();
        }
        else
        {
            PlayerA_CS.SetReadyToAttack();
        }

        if(PlayerB_CS.isGroggyHP())
        {
            PlayerB_CS.SetGroggyState();
        }
        else
        {
            PlayerB_CS.SetReadyToAttack();
        }

        Snd_Finish.Play();
        Layer_AttackTimer.SetActive(false);
        
        _groggyTime = 0.0f;
         _gameState = GameState.Groggy;
    }

    void SetHitPlayers()
    {
        AttackType attackTypeA = PlayerA_CS.getLastAttackType();
        AttackType attackTypeB = PlayerB_CS.getLastAttackType();

        if(AttackType.None != attackTypeB && (attackTypeB == attackTypeA || checkLose(attackTypeB, attackTypeA)))
        {
            CreateEffectAttackHit(attackTypeB, true);
            bool isCritical = PlayerB_CS.IsCriticalAttack();
            Image_Bam_A.SetActive(!isCritical);
            Image_Critical_A.SetActive(isCritical);
            int damage = PlayerB_CS.GetPowerWithGuage();
            PlayerA_CS.SetDamage(damage, attackTypeB);
        }

        if(AttackType.None != attackTypeA && (attackTypeA == attackTypeB || checkLose(attackTypeA, attackTypeB)))
        {
            CreateEffectAttackHit(attackTypeA, false);
            bool isCritical = PlayerA_CS.IsCriticalAttack();
            Image_Bam_B.SetActive(!isCritical);
            Image_Critical_B.SetActive(isCritical);
            int damage = PlayerA_CS.GetPowerWithGuage();
            PlayerB_CS.SetDamage(damage, attackTypeA);

            if(false == PlayerB_CS.GetIsPlayer())
            {
                _recordAttackPoint += damage;
            }
        }

        MainCamera.GetComponent<CameraCS>().setShake();
    }

    void SetAttackHitState()
    {
        _gameState = GameState.AttackHit;
        _attackHitTime = 0.0f;
        AttackTimer_CS.setBar(0.0f);

        int prevHP = PlayerA_CS.GetHP();

        PlayerA_CS.SetAttackHit();
        PlayerB_CS.SetAttackHit();

        SetHitPlayers();

        // set flicker
        if(prevHP != PlayerA_CS.GetHP())
        {
            _ko_sprite_flicker = true;
            int flickerHP = PlayerA_CS.GetMaxHP() / Constants.WarningHPDivide;
            if(PlayerA_CS.GetHP() <= flickerHP)
            {
                _ko_sprite_flicker_low_hp = true;
            }
        }
	}

    void SetPause(bool pause)
    {
        if(GameState.RoundEnd == _gameState && false == _pause)
        {
            return;
        }

        _pause = pause;
        PlayerA_CS.SetPause(pause);
        PlayerB_CS.SetPause(pause);
        Image_Exit.SetActive(pause);
    }

     // Update is called once per frame
    void Update()
    {
        if(MainSceneManager.GetComponent<MainSceneManagerCS>().GetActivateSceneType() == GameSceneType.FightScene)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(GameState.GameResult == _gameState)
                {
                    Exit();
                    return;
                }
                else
                {
                    SetPause(!_pause);
                }
            }
        }

        if(_pause)
        {
            return;
        }

        // update flicker
        if(_ko_sprite_flicker || _ko_sprite_flicker_low_hp)
        {
            if(_ko_sprite_flicker_time < 0.0f)
            {
                SetSprite(Image_KO, (Sprite_Ko == GetSprite(Image_KO)) ? Sprite_Ko_Invert : Sprite_Ko);
                _ko_sprite_flicker_time = 0.1f;
            }
            _ko_sprite_flicker_time -= Time.deltaTime;
        }

        // update state
        if(GameState.ReadyToFight == _gameState)
        {
        }
        else if(GameState.ReadyToRound == _gameState)
        {
            float fightSoundTime = Constants.RoundReadyTime - 1.0f;
            if(_readyToRoundTime <= fightSoundTime && fightSoundTime < (_readyToRoundTime + Time.deltaTime))
            {
                Snd_Fight.Play();
                Text_Result.GetComponent<TextMeshProUGUI>().text = "Fight!";
            }

            if(Constants.RoundReadyTime <= _readyToRoundTime)
            {
                Snd_Bell_1.Play();
                ResetRound();
                SetReadyToAttack();
            }
            _readyToRoundTime += Time.deltaTime;
        }
        else if(GameState.ReadyToAttack == _gameState)
        {
            float attackTimerBar = 1.0f - ((_attackTimerTime % _initialAttackTimerTime) / _initialAttackTimerTime);
            AttackTimer_CS.setBar(attackTimerBar);
            if(_initialAttackTimerTime <= _attackTimerTime)
            {
                SetAttackHitState();
            }
            _attackTimerTime += Time.deltaTime;
        }
        else if(GameState.AttackHit == _gameState)
        {
            // delay
            float attackHitTimeDelay = (false == PlayerA_CS.isAlive() || false == PlayerB_CS.isAlive()) ? Constants.AttackHitTimeDelay : 0.0f;

            if((Constants.AttackHitTime + attackHitTimeDelay) <= _attackHitTime)
            {
                Image_Bam_A.SetActive(false);
                Image_Bam_B.SetActive(false);
                Image_Critical_A.SetActive(false);
                Image_Critical_B.SetActive(false);

                bool isAliveA = PlayerA_CS.isAlive();
                bool isAliveB = PlayerB_CS.isAlive();
                if (false == isAliveA || false == isAliveB)
                {
                    Text_Result.SetActive(true);

                    if(isAliveA && false == isAliveB)
                    {
                        Layer_Wins.transform.Find("WinA" + PlayerA_CS.GetWin().ToString()).gameObject.SetActive(true);
                        PlayerA_CS.SetWin();
                        PlayerB_CS.SetDead();
                        Snd_Win.Play();
                        Text_Result.GetComponent<TextMeshProUGUI>().text = "You Win";
                    }
                    else if(false == isAliveA && isAliveB)
                    {
                        Layer_Wins.transform.Find("WinB" + PlayerB_CS.GetWin().ToString()).gameObject.SetActive(true);
                        PlayerA_CS.SetDead();
                        PlayerB_CS.SetWin();
                        Snd_Lose.Play();
                        Text_Result.GetComponent<TextMeshProUGUI>().text = "You Lose";
                    }
                    else
                    {
                        PlayerA_CS.SetDead();
                        PlayerB_CS.SetDead();
                        Snd_Draw.Play();
                        Text_Result.GetComponent<TextMeshProUGUI>().text = "Draw";
                    }
                    
                    int maxWinCount = Mathf.Max(PlayerA_CS.GetWin(), PlayerB_CS.GetWin());
                    if (_round < _maxRoundCount && maxWinCount < _maxWinCount)
                    {
                        Snd_Bell_2.Play();
                    }
                    else
                    {
                        Snd_Bell_3.Play();
                    }

                    // record
                    _recordHP += PlayerA_CS.GetHP();

                    SetRoundEnd();
                }
                else
                {
                    // check is groggy
                    bool isGroggyHP_A = PlayerA_CS.isGroggyHP();
                    bool isGroggyHP_B = PlayerB_CS.isGroggyHP();
                    int winCountA = PlayerA_CS.GetWin() + ((false == isGroggyHP_A && isGroggyHP_B) ? 1 : 0);
                    int winCountB = PlayerB_CS.GetWin() + ((false == isGroggyHP_B && isGroggyHP_A) ? 1 : 0);
                    int maxWinCount = Mathf.Max(winCountA, winCountB);
                    if ((_maxRoundCount <= _round || _maxWinCount <= maxWinCount) && (isGroggyHP_A || isGroggyHP_B))
                    {
                        SetGroggyState();
                    }
                    else
                    {
                        SetReadyToAttack();
                    }
                }
            }
            _attackHitTime += Time.deltaTime;
        }
        else if(GameState.Groggy == _gameState)
        {
            bool isGroggyHP_A = PlayerA_CS.isGroggyHP();
            bool isGroggyHP_B = PlayerB_CS.isGroggyHP();

            // groggy attack
            if(false == isGroggyHP_A && AttackType.None != PlayerA_CS.getLastAttackType())
            {
                SetHitPlayers();
            }
            else if(false == isGroggyHP_B && AttackType.None != PlayerB_CS.getLastAttackType())
            {
                SetHitPlayers();
            }

            // end groggy
            if(Constants.GroggyHitTime <= _groggyTime)
            {
                if(isGroggyHP_A)
                {
                    PlayerA_CS.SetDead();
                }

                if(isGroggyHP_B)
                {
                    PlayerB_CS.SetDead();
                }

                SetAttackHitState();
            }
            _groggyTime += Time.deltaTime;
        }
        else if(GameState.RoundEnd == _gameState)
        {
            if(Constants.RoundEndTime <= _roundEndTime)
            {
                bool isPlayerA_Win = PlayerB_CS.GetWin() < PlayerA_CS.GetWin();

                int maxWinCount = isPlayerA_Win ? PlayerA_CS.GetWin() : PlayerB_CS.GetWin();
                if (_round < _maxRoundCount && maxWinCount < _maxWinCount)
                {
                    _round += 1;
                    SetReadyToRound();
                }
                else
                {
                    // Save
                    ChallengeSceneManagerCS challenegeSceneManager = ChallengeSceneManager.GetComponent<ChallengeSceneManagerCS>();
                    challenegeSceneManager.AddChallengeScore(_recordAttackPoint, _recordHP, isPlayerA_Win);
                    if(PlayerB_CS.GetWin() < PlayerA_CS.GetWin())
                    {
                        PlayerA_CS._playerStat._win += 1;
                        PlayerB_CS._playerStat._lose += 1;

                        // next stage
                        if(false == PlayerB_CS.GetIsPlayer())
                        {
                            int nextStage = PlayerA_CS._playerStat._stage + 1;
                            PlayerA_CS._playerStat._stage = nextStage;
                            SystemValue.SetInt(SystemValue.PlayerLastStageKey, nextStage);
                        }
                    }
                    else if(PlayerA_CS.GetWin() < PlayerB_CS.GetWin())
                    {
                        PlayerB_CS._playerStat._win += 1;
                        PlayerA_CS._playerStat._lose += 1;
                    }
                    else
                    {
                        PlayerA_CS._playerStat._draw += 1;
                        PlayerB_CS._playerStat._draw += 1;
                    }

                    PlayerA_CS.SavePlayerStat();
                    PlayerB_CS.SavePlayerStat();

                    SetGameResult();
                }
            }
            _roundEndTime += Time.deltaTime;
        }
        else if(GameState.GameResult == _gameState)
        {
            if(Constants.GameResultTime <= _gameResultTime)
            {
                MainSceneManager.GetComponent<MainSceneManagerCS>().ShowInterstitial();
                Exit();
            }
            _gameResultTime += Time.deltaTime;
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
