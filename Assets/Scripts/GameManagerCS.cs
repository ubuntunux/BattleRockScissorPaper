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
    RoundEnd,
    GameResult,
}

public class GameManagerCS : MonoBehaviour
{
    // Properties
    float _elapsedTime = 0.0f;
    float _attackTimerTime = 0.0f;
    float _attackHitTime = 0.0f;
    float _attackHitTimeDelay = 0.0f;    
    float _readyToRoundTime = 0.0f;
    float _roundEndTime = 0.0f;
    float _gameResultTime = 0.0f;
    int _maxRoundCount = 3;
    int _winCount = 2;
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

    // UI
    public GameObject LayerResult;
    public GameObject Btn_Exit;
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
        MainSceneManager.GetComponent<MainSceneManagerCS>().SetActivateScenePrev();     
    }

    public void ResetGameManager(PlayerCreateInfo playerCreateInfoA, PlayerCreateInfo playerCreateInfoB)
    {
        _pause = false;
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
        Btn_Exit.SetActive(false);

        PlayerA_CS.Reset(GetComponent<GameManagerCS>(), Layer_HP_Bar_A, Layer_AttackTimer, playerCreateInfoA);
        PlayerB_CS.Reset(GetComponent<GameManagerCS>(), Layer_HP_Bar_B, Layer_AttackTimer, playerCreateInfoB);

        LayerResult.SetActive(false);
        SetReadyToRound();
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
        Quaternion rot = Quaternion.Euler(-90.0f, isLeft ? 180.0f : 0.0f, 0.0f);
		GameObject effect_AttackHit = (GameObject)GameObject.Instantiate(Effect_AttackHit, pos, rot);

        if(isLeft)
        {
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
        _attackHitTimeDelay = 0.0f;
        _readyToRoundTime = 0.0f;
        _roundEndTime = 0.0f;
        _gameResultTime = 0.0f;

        PlayerA_CS.SetReadyToRound();
        PlayerB_CS.SetReadyToRound();

        DestroyEffectAttackHit(true);
        DestroyEffectAttackHit(false);

        Layer_AttackTimer.SetActive(false);
        AttackTimer_CS.Reset();
        Text_Result.SetActive(false);
        Image_Bam_A.SetActive(false);
        Image_Bam_B.SetActive(false);
    }

    void SetReadyToRound()
    {
        ResetRound();
        
        if(_maxRoundCount == _round) { Snd_FinalRound.Play(); }
        else if (1 == _round) {Snd_Round1.Play();}
        else if (2 == _round) {Snd_Round2.Play();}
        else if (3 == _round) {Snd_Round3.Play();}
        else if (4 == _round) {Snd_Round4.Play();}

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
        AttackTimer_CS.Reset();
        PlayerA_CS.SetReadyToAttack();
        PlayerB_CS.SetReadyToAttack();
        _attackTimerTime = 0.0f;
        _gameState = GameState.ReadyToAttack;
    }

    void SetAttackHit()
    {
        _gameState = GameState.AttackHit;
        AttackTimer_CS.setBar(0.0f);
        _attackHitTime = 0.0f;
        int prevHP = PlayerA_CS.GetHP();
		PlayerA_CS.SetAttackHit();
        PlayerB_CS.SetAttackHit();

        AttackType attackTypeA = PlayerA_CS.getLastAttackType();
        AttackType attackTypeB = PlayerB_CS.getLastAttackType();

        if(attackTypeB == attackTypeA || checkLose(attackTypeB, attackTypeA))
        {
            CreateEffectAttackHit(attackTypeB, true);
            Image_Bam_A.SetActive(true);        
            PlayerA_CS.SetDamage(PlayerB_CS.GetStat().GetPower(attackTypeB == attackTypeA));
        }

        if(attackTypeA == attackTypeB || checkLose(attackTypeA, attackTypeB))
        {
            CreateEffectAttackHit(attackTypeA, false);
            Image_Bam_B.SetActive(true);
            int damage = PlayerA_CS.GetStat().GetPower(attackTypeA == attackTypeB);
            PlayerB_CS.SetDamage(damage);
            
            _recordAttackPoint += damage;
        }

        // set flicker
        if(prevHP != PlayerA_CS.GetHP())
        {
            _ko_sprite_flicker = true;

            const int flickerHP = 2;

            if(PlayerA_CS.GetHP() <= flickerHP)
            {
                _ko_sprite_flicker_low_hp = true;
            }
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
            if(GameState.GameResult == _gameState)
            {
                Exit();
                return;
            }
            else
            {
                _pause = !_pause;
                PlayerA_CS.SetPause(_pause);
                PlayerB_CS.SetPause(_pause);
                Btn_Exit.SetActive(_pause);
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
                ResetRound();
                SetReadyToAttack();
            }
            _readyToRoundTime += Time.deltaTime;
        }
        else if(GameState.ReadyToAttack == _gameState)
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
                Image_Bam_A.SetActive(false);
                Image_Bam_B.SetActive(false);

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

                    // record
                    _recordHP += PlayerA_CS.GetHP();

                    SetRoundEnd();
                }
                else
                {
                    SetReadyToAttack();
                }
            }
            _attackHitTime += Time.deltaTime;
        }
        else if(GameState.RoundEnd == _gameState)
        {
            if(Constants.RoundEndTime <= _roundEndTime)
            {
                bool isPlayerA_Win = PlayerB_CS.GetWin() < PlayerA_CS.GetWin();
                // Result
                ChallengeSceneManagerCS challenegeSceneManager = ChallengeSceneManager.GetComponent<ChallengeSceneManagerCS>();
                bool levelUp = challenegeSceneManager.AddChallengeScore(PlayerB_CS.GetStat()._level, _recordAttackPoint, _recordHP, isPlayerA_Win);

                int maxWinCount = isPlayerA_Win ? PlayerA_CS.GetWin() : PlayerB_CS.GetWin();
                if (_round < _maxRoundCount && maxWinCount < _winCount)
                {
                    _round += 1;
                    SetReadyToRound();
                }
                else
                {
                    SetGameResult();
                }
            }
            _roundEndTime += Time.deltaTime;
        }
        else if(GameState.GameResult == _gameState)
        {
            if(Constants.GameResultTime <= _gameResultTime)
            {
                MainSceneManager.GetComponent<MainSceneManagerCS>().SetActivateScenePrev();
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
