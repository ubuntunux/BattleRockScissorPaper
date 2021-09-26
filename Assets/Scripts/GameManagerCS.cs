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
    GroggyAttack,
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
    float _groggyAttackTime = 0.0f;
    float _roundEndTime = 0.0f;
    float _gameResultTime = 0.0f;
    int _maxRoundCount = 3;
    int _maxWinCount = 2;
    int _round = 1;
    float _roundTimer = Constants.RoundTime;
    bool _pause = false;
    bool _isVersusScene = false;
    bool _stopRoundTimer = false;
    GameState _gameState = GameState.None;

    // Record
    int _recordTimePoint = 0;
    int _recordAttackPoint = 0;
    int _recordHP = 0;
    int _recordBonus = 0;
    int _recordTotalScore = 0;

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
    public GameObject Text_Damage_A;
    public GameObject Text_Damage_B;

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
    public GameObject Canvas;    
    public GameObject LayerResult;
    public GameObject Image_Exit;
    public GameObject Layer_AttackButtonsA;
    public GameObject Layer_AttackButtonsB;
    public GameObject Layer_AttackTimer;
    FightTimerCS AttackTimer_CS;
    public GameObject Layer_HP_Bar_A;
    public GameObject Layer_HP_Bar_B;
    public GameObject Text_Result;
    public GameObject Text_Timer;
    public GameObject Image_KO;
    public Sprite Sprite_Ko;
    public Sprite Sprite_Ko_Invert;
    float _ko_sprite_flicker_time = 0.0f;
    bool _ko_sprite_flicker = false;
    bool _ko_sprite_flicker_low_hp = false;
    public GameObject Layer_Wins;
    public GameObject Btn_Advertisement;
    
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
        MainSceneManager.GetComponent<MainSceneManagerCS>().SetActivateScene(_isVersusScene ? GameSceneType.VersusScene : GameSceneType.ChallenegeScene);
    }

    public bool CheckGameState(GameState gameState)
    {
        return _gameState == gameState;
    }

    public void ResetGameManager(bool isVersusScene, PlayerCreateInfo playerCreateInfoA, PlayerCreateInfo playerCreateInfoB)
    {
        _isVersusScene = isVersusScene;
        _gameState = GameState.ReadyToFight;
        _elapsedTime = 0.0f;
        _round = 1;
        _stopRoundTimer = false;

        // record
        _recordTimePoint = 0;
        _recordAttackPoint = 0;
        _recordHP = 0;
        _recordBonus = 0;
        _recordTotalScore = 0;

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

        Layer_AttackButtonsB.SetActive(playerCreateInfoB._isPlayer);
        LayerResult.SetActive(false);
        Btn_Advertisement.SetActive(true);

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
        // DestroyEffectAttackHit(isPlayerA);

        // float offsetY = Constants.GroundPosition;
        // if(AttackType.Rock == attackType)
        // {
        //     offsetY = 2.0f;
        // }
        // else if(AttackType.Scissor == attackType)
        // {
        //     offsetY = 1.0f;
        // }
        // else if(AttackType.Paper == attackType)
        // {
        //     offsetY = 3.0f;
        // }

        // Vector3 pos = new Vector3(isPlayerA ? -Constants.AttackDistance : Constants.AttackDistance, offsetY, 0.0f);
        // Quaternion rot = Quaternion.Euler(-90.0f, isPlayerA ? 180.0f : 0.0f, 0.0f);
		// GameObject effect_AttackHit = (GameObject)GameObject.Instantiate(Effect_AttackHit, pos, rot);

        // if(isPlayerA)
        // {
        //     _effect_AttackHitA = effect_AttackHit;
        // }
        // else
        // {   
        //     _effect_AttackHitB = effect_AttackHit;
        // }
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

    public void OnClickAdvertisement()
    {
        Btn_Advertisement.SetActive(false);
        MainSceneManager.GetComponent<MainSceneManagerCS>().ShowFightRewardedAd(_recordTotalScore);
    }

    public void Btn_Back_OnClick()
    {
        if(GameState.GameResult != _gameState)
        {
            SetPause(!_pause);
        }
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
        _stopRoundTimer = false;

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
        Text_Damage_A.SetActive(false);
        Text_Damage_B.SetActive(false);
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
        Text_Result.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, Text_Result.GetComponent<RectTransform>().anchoredPosition.y);

        _initialAttackTimerTime = Constants.AttackTimerTime;
        _roundTimer = Constants.RoundTime;

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
        LayerResult.GetComponent<ResultCS>().ResetResultScene(
            PlayerA_CS, 
            PlayerA_CS.GetWin(), 
            PlayerB_CS, 
            PlayerB_CS.GetWin(), 
            _isVersusScene,
            _recordTimePoint,
            _recordAttackPoint,
            _recordHP,
            _recordBonus,
            _recordTotalScore
        );
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
        
        float newAttackTime = 2.0f / Mathf.Max(PlayerA_CS._playerStat._speed, PlayerB_CS._playerStat._speed);
        // prevent too fast
        _initialAttackTimerTime = Mathf.Lerp(_initialAttackTimerTime, Random.Range(newAttackTime, Constants.AttackTimerTime), 0.5f);

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
        Text_Result.SetActive(true);
        Text_Result.GetComponent<TextMeshProUGUI>().text = "Finish!!";
        Layer_AttackTimer.SetActive(false);
        
        _stopRoundTimer = true;
        _groggyTime = 0.0f;
        _groggyAttackTime = 0.0f;        
        _gameState = GameState.Groggy;
    }

    float GetHitImagePositionY(AttackType attackType)
    {
        if(AttackType.Paper == attackType)
        {
            return 3.8f;
        }
        else if(AttackType.Scissor == attackType)
        {
            return 1.5f;
        }
        return 2.8f;
    }

    int SetHitPlayer(PlayerCS attacker, PlayerCS attackee, GameObject Image_Bam, GameObject Image_Critical, GameObject Text_Damage)
    {
        AttackType attackeeAttackType = attackee.getLastAttackType();
        AttackType attackerAttackType = attacker.getLastAttackType();

        if(AttackType.None != attackerAttackType && (attackerAttackType == attackeeAttackType || checkLose(attackerAttackType, attackeeAttackType)))
        {
            CreateEffectAttackHit(attackerAttackType, attackee.GetIsPlayerA());
            bool isGroggyHP = attackee.isGroggyHP();
            bool isCritical = attacker.IsCriticalAttack() && false == isGroggyHP;
            Vector3 imagePosition = Image_Bam.transform.localPosition;
            imagePosition.y = GetHitImagePositionY(attackerAttackType);
            Image_Bam.transform.localPosition = imagePosition;
            Image_Bam.SetActive(!isCritical);
            Image_Critical.transform.localPosition = imagePosition;
            Image_Critical.SetActive(isCritical);
            int damage = isGroggyHP ? attacker.GetPower() : attacker.GetPowerWithGuage();
            attackee.SetDamage(damage, attackerAttackType);

            // first you need the RectTransform component of your canvas
            Vector3 damagePosition = imagePosition;
            damagePosition.y += 1.0f;
            RectTransform CanvasRect = Canvas.GetComponent<RectTransform>();
            Vector2 ViewportPosition = MainCamera.GetComponent<Camera>().WorldToViewportPoint(damagePosition);
            Vector2 WorldObject_ScreenPosition = new Vector2(
                ((ViewportPosition.x*CanvasRect.sizeDelta.x)-(CanvasRect.sizeDelta.x*0.5f)),
                ((ViewportPosition.y*CanvasRect.sizeDelta.y)-(CanvasRect.sizeDelta.y*0.5f))
            );
            Text_Damage.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
            Text_Damage.GetComponent<TextMeshProUGUI>().text = damage.ToString();
            Text_Damage.GetComponent<TextMeshProUGUI>().color = attacker.GetPowerGuageColor();
            Text_Damage.SetActive(true);

            return damage;
        }
        return 0;
    }

    void SetHitPlayers(float shaderIntensity = 0.0f)
    {
        SetHitPlayer(PlayerB_CS, PlayerA_CS, Image_Bam_A, Image_Critical_A, Text_Damage_A);
        int damage = SetHitPlayer(PlayerA_CS, PlayerB_CS, Image_Bam_B, Image_Critical_B, Text_Damage_B);
        if(0 < damage && false == PlayerB_CS.GetIsPlayer())
        {
            _recordAttackPoint += damage;
        }
        MainCamera.GetComponent<CameraCS>().setShake(shaderIntensity);
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
                if(GameState.GameResult != _gameState)
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

        // update timer
        if(false == _stopRoundTimer)
        {
            if(GameState.ReadyToAttack == _gameState || GameState.AttackHit == _gameState)
            {
                if(0.0f < _roundTimer)
                {
                    _roundTimer -= Time.deltaTime;
                    if(_roundTimer < 0.0f)
                    {
                        _roundTimer = 0.0f;
                    }
                }
            }

            int timer = Mathf.Max(0, (int)(Mathf.Ceil(_roundTimer)));
            Text_Timer.GetComponent<TextMeshProUGUI>().text = timer.ToString();
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
            float attackTimerBar = Mathf.Max(0.0f, 1.0f - (_attackTimerTime / _initialAttackTimerTime));
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
                Text_Damage_A.SetActive(false);
                Text_Damage_B.SetActive(false);

                bool isAliveA = PlayerA_CS.isAlive();
                bool isAliveB = PlayerB_CS.isAlive();
                if (false == isAliveA || false == isAliveB || _roundTimer <= 0.0f)
                {
                    Text_Result.SetActive(true);
                    float textResultPosOffsetX = 270.0f;
                    float textResultPosX = 0.0f;

                    if(false == isAliveA)
                    {
                        PlayerA_CS.SetDead();
                    }

                    if(false == isAliveB)
                    {
                        PlayerB_CS.SetDead();
                    }

                    if(isAliveA && false == isAliveB)
                    {
                        Layer_Wins.transform.Find("WinA" + PlayerA_CS.GetWin().ToString()).gameObject.SetActive(true);
                        PlayerA_CS.SetWin();
                        Snd_Win.Play();

                        if(_isVersusScene)
                        {
                            textResultPosX = -textResultPosOffsetX;
                            Text_Result.GetComponent<TextMeshProUGUI>().text = "Win";
                        }
                        else
                        {
                            Text_Result.GetComponent<TextMeshProUGUI>().text = "You Win";
                        }
                    }
                    else if(false == isAliveA && isAliveB)
                    {
                        Layer_Wins.transform.Find("WinB" + PlayerB_CS.GetWin().ToString()).gameObject.SetActive(true);
                        PlayerB_CS.SetWin();

                        if(_isVersusScene)
                        {
                            Snd_Win.Play();
                            Text_Result.GetComponent<TextMeshProUGUI>().text = "Win";
                            textResultPosX = textResultPosOffsetX;
                        }
                        else
                        {
                            Snd_Lose.Play();
                            Text_Result.GetComponent<TextMeshProUGUI>().text = "You Lose";
                        }
                    }
                    else
                    {
                        Snd_Draw.Play();
                        Text_Result.GetComponent<TextMeshProUGUI>().text = "Draw";
                    }
                    Text_Result.GetComponent<RectTransform>().anchoredPosition = new Vector2(textResultPosX, Text_Result.GetComponent<RectTransform>().anchoredPosition.y);
                    
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
                    _recordTimePoint += Mathf.Max(0, (int)(Mathf.Ceil(_roundTimer)));

                    SetRoundEnd();
                }
                else
                {
                    // check is groggy
                    bool isGroggyHP_A = PlayerA_CS.isAlive() && PlayerA_CS.isGroggyHP();
                    bool isGroggyHP_B = PlayerB_CS.isAlive() && PlayerB_CS.isGroggyHP();
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
            if(0.0f < _groggyAttackTime)
            {
                _groggyAttackTime -= Time.deltaTime;

                if(_groggyAttackTime < 0.0f)
                {
                    if(isGroggyHP_A)
                    {
                        PlayerA_CS.SetGroggyState();
                    }
                    else
                    {
                        PlayerA_CS.SetReadyToAttack();
                    }

                    if(isGroggyHP_B)
                    {
                        PlayerB_CS.SetGroggyState();
                    }
                    else
                    {
                        PlayerB_CS.SetReadyToAttack();
                    }

                    _groggyAttackTime = 0.0f;
                }
            }

            if(0.0f == _groggyAttackTime)
            {
                float shaderIntensity = Constants.CameraShakeIntensity * 0.2f;
                PlayerCS player = null;
                if(false == isGroggyHP_A && AttackType.None != PlayerA_CS.getLastAttackType())
                {
                    player = PlayerA_CS;
                }
                else if(false == isGroggyHP_B && AttackType.None != PlayerB_CS.getLastAttackType())
                {
                    player = PlayerB_CS;
                }

                if(null != player)
                {
                    player.SetAttackHit();
                    SetHitPlayers(shaderIntensity);
                    _groggyAttackTime = Constants.AttackMotionTime;
                }
            }

            // end groggy
            if(Constants.GroggyHitTime <= _groggyTime)
            {
                _gameState = GameState.AttackHit;
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
                    if(false == _isVersusScene)
                    {
                        // record score
                        _recordTotalScore = _recordAttackPoint + _recordHP + _recordTimePoint * Constants.TimePoint;
                        _recordBonus = isPlayerA_Win ? (_recordTotalScore / 2) : 0;
                        _recordTotalScore += _recordBonus;
                        MainSceneManager.GetComponent<MainSceneManagerCS>().AddScore(_recordTotalScore);
                        
                        if(PlayerB_CS.GetWin() < PlayerA_CS.GetWin())
                        {
                            PlayerA_CS._playerStat._win += 1;
                            PlayerB_CS._playerStat._lose += 1;

                            // next stage
                            if(false == PlayerB_CS.GetIsPlayer())
                            {
                                int lastStage = SystemValue.GetInt(SystemValue.PlayerLastStageKey);
                                if(lastStage <= SystemValue.GetInt(SystemValue.PlayerSelectStageKey))
                                {
                                    int nextStage = lastStage + 1;
                                    SystemValue.SetInt(SystemValue.PlayerLastStageKey, nextStage);
                                    SystemValue.SetInt(SystemValue.PlayerSelectStageKey, nextStage);
                                }
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
                    }
                    
                    SetGameResult();
                }
            }
            _roundEndTime += Time.deltaTime;
        }
        else if(GameState.GameResult == _gameState)
        {
            if(Constants.GameResultTime <= _gameResultTime)
            {
                if(false == _isVersusScene)
                {
                    MainSceneManager.GetComponent<MainSceneManagerCS>().ShowInterstitial();
                }
                
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
