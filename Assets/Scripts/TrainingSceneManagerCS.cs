using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

enum TraningState
{
    None,
    UpgradeHP,
    UpgradePower,
    UpgradeSpeed,
};

public class TrainingSceneManagerCS : MonoBehaviour
{
    static int UpgradeStepHP = 2;
    static int UpgradeStepPower = 1;
    static float UpgradeStepSpeed = 0.2f;

    public GameObject MainSceneManager;
    public GameObject PlayerA;
    public GameObject PlayerB;
    public GameObject SkinScene;

    public GameObject Text_Health;
    public GameObject Text_Power;
    public GameObject Text_Speed;

    public GameObject Text_Acoount_Health;
    public GameObject Text_Acoount_Power;
    public GameObject Text_Acoount_Speed;

    public GameObject Btn_Health;
    public GameObject Btn_Power;
    public GameObject Btn_Speed;

    public GameObject Image_Upgrade;
    public GameObject Text_Upgrade;

    public AudioSource Snd_Upgrade;

    TraningState _traningState = TraningState.None;
    float _upgradeTime = 0.0f;
    float _attackMotionTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    void OnEnable()
    {
        ResetTrainingScene();
    }

    void OnDisable()
    {
    }

    public void Exit()
    {
        MainSceneManager.GetComponent<MainSceneManagerCS>().SetActivateScene(GameSceneType.MainScene);
    }

    public void Btn_Skin_OnClick()
    {
        SkinScene.GetComponent<SkinManagerCS>().SetTargetPlayer(PlayerA);
        MainSceneManager.GetComponent<MainSceneManagerCS>().SetActivateScene(GameSceneType.SkinScene);
    }

    public int GetNextUpdateHPAccounts()
    {
        int upgradeHPAccounts = (PlayerA.GetComponent<PlayerCS>()._playerStat._hp - Constants.DefaultHP + UpgradeStepHP) / UpgradeStepHP;
        upgradeHPAccounts = 900 + upgradeHPAccounts * 100;
        return upgradeHPAccounts;
    }

    public int GetNextUpdatePowerAccounts()
    {
        int upgradePowerAccounts = (PlayerA.GetComponent<PlayerCS>()._playerStat._power - Constants.DefaultPower + UpgradeStepPower) / UpgradeStepPower;
        upgradePowerAccounts = 900 + upgradePowerAccounts * 100;
        return upgradePowerAccounts;
    }


    public int GetNextUpdateSpeedAccounts()
    {
        int upgradeSpeedAccounts = (int)((PlayerA.GetComponent<PlayerCS>()._playerStat._speed - Constants.DefaultSpeed + UpgradeStepSpeed) / UpgradeStepSpeed);
        upgradeSpeedAccounts = 900 + upgradeSpeedAccounts * 100;
        return upgradeSpeedAccounts;
    }


    public void Btn_OnClick_HealthTraning()
    {
        int upgradeHPAccounts = GetNextUpdateHPAccounts();
        int score = MainSceneManager.GetComponent<MainSceneManagerCS>().GetScore();
        if(Constants.SHOW_ME_THE_MONEY || upgradeHPAccounts <= score)
        {
            SetTraning(TraningState.UpgradeHP);
        }
    }

    public void Btn_OnClick_PowerTraning()
    {
        int upgradePowerAccounts = GetNextUpdatePowerAccounts();
        int score = MainSceneManager.GetComponent<MainSceneManagerCS>().GetScore();
        if(Constants.SHOW_ME_THE_MONEY || upgradePowerAccounts <= score)
        {
            SetTraning(TraningState.UpgradePower);
        }
    }

    public void Btn_OnClick_SpeedTraning()
    {
        int upgradeSpeedAccounts = GetNextUpdateSpeedAccounts();
        int score = MainSceneManager.GetComponent<MainSceneManagerCS>().GetScore();
        if(Constants.SHOW_ME_THE_MONEY || upgradeSpeedAccounts <= score)
        {
            SetTraning(TraningState.UpgradeSpeed);
        }
    }

    public void Refresh()
    {
        int hp = PlayerA.GetComponent<PlayerCS>()._playerStat._hp;
        int power = PlayerA.GetComponent<PlayerCS>()._playerStat._power;
        float speed = PlayerA.GetComponent<PlayerCS>()._playerStat._speed;

        Text_Health.GetComponent<TextMeshProUGUI>().text = string.Format("HP: {0} -> {1}", hp, hp + UpgradeStepHP);
        Text_Power.GetComponent<TextMeshProUGUI>().text = string.Format("Power: {0} -> {1}", power, power + UpgradeStepPower);
        Text_Speed.GetComponent<TextMeshProUGUI>().text = string.Format("Speed: {0:F1} -> {1:F1}", speed, speed + UpgradeStepSpeed);

        Text_Acoount_Health.GetComponent<TextMeshProUGUI>().text = GetNextUpdateHPAccounts().ToString();
        Text_Acoount_Power.GetComponent<TextMeshProUGUI>().text = GetNextUpdatePowerAccounts().ToString();
        Text_Acoount_Speed.GetComponent<TextMeshProUGUI>().text = GetNextUpdateSpeedAccounts().ToString();
    }

    void ResetTrainingScene()
    {
        PlayerA.SetActive(true);
        PlayerA.GetComponent<PlayerCS>().SetPlayerInfo(true, true);
        PlayerA.GetComponent<PlayerCS>().SetAttackIdle();
        
        PlayerB.SetActive(false);
        SetTraning(TraningState.None);
        Refresh();
    }

    void SetTraning(TraningState traningState)
    {
        bool isTraning = TraningState.None != traningState;
        Btn_Health.SetActive(false == isTraning);
        Btn_Power.SetActive(false == isTraning);
        Btn_Speed.SetActive(false == isTraning);
        Image_Upgrade.SetActive(isTraning);
        Text_Upgrade.SetActive(isTraning);
        if(isTraning)
        {
            Vector3 scale = Image_Upgrade.transform.localScale;
            scale.x = 0.0f;
            Image_Upgrade.transform.localScale = scale;

            Vector3 imagePosition = Image_Upgrade.transform.localPosition;
            Vector3 textPosition = Text_Upgrade.transform.localPosition;
            if(TraningState.UpgradeHP == traningState)
            {
                imagePosition.y = Btn_Health.transform.localPosition.y;
                textPosition.y = Btn_Health.transform.localPosition.y;
            }
            else if(TraningState.UpgradePower == traningState)
            {
                imagePosition.y = Btn_Power.transform.localPosition.y;
                textPosition.y = Btn_Power.transform.localPosition.y;
            }
            else if(TraningState.UpgradeSpeed == traningState)
            {
                imagePosition.y = Btn_Speed.transform.localPosition.y;
                textPosition.y = Btn_Speed.transform.localPosition.y;
            }
            Image_Upgrade.transform.localPosition = imagePosition;
            
            Text_Upgrade.GetComponent<TextMeshProUGUI>().text = "Traning";
            Text_Upgrade.transform.localPosition = textPosition;
        }

        _attackMotionTime = 0.0f;
        _upgradeTime = 0.0f;
        _traningState = traningState;
    }

    // Update is called once per frame
    void Update()
    {
        if(MainSceneManager.GetComponent<MainSceneManagerCS>().GetActivateSceneType() == GameSceneType.TrainingScene)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(TraningState.None == _traningState)
                {
                    Exit();
                }
                else
                {
                    SetTraning(TraningState.None);
                }
            }
        }

        if(TraningState.None != _traningState)
        {
            if(_attackMotionTime <= 0.0f)
            {
                PlayerA.GetComponent<PlayerCS>().SetAttack((AttackType)(Random.Range(0, 3) + 1));
                _attackMotionTime = Random.Range(Constants.AttackRandomTermMin, Constants.AttackRandomTermMax * 0.5f);
            }

            // upgrade image
            Vector3 scale = Image_Upgrade.transform.localScale;
            scale.x = Mathf.Min(1.0f, _upgradeTime / Constants.TraningTime);
            Image_Upgrade.transform.localScale = scale;

            // upgrade text
            int dotCount = ((int)(_upgradeTime / 0.1f) % 4);
            string upgradeString = "Traning";
            for(int i = 0; i < dotCount; ++i)
            {
                upgradeString += ".";
            }
            Text_Upgrade.GetComponent<TextMeshProUGUI>().text = upgradeString;

            if(Constants.TraningTime <= _upgradeTime)
            {
                int score = MainSceneManager.GetComponent<MainSceneManagerCS>().GetScore();

                // update hp
                if(TraningState.UpgradeHP == _traningState)
                {
                    int upgradeHPAccounts = GetNextUpdateHPAccounts();
                    if(Constants.SHOW_ME_THE_MONEY || upgradeHPAccounts <= score)
                    {
                        int hp = PlayerA.GetComponent<PlayerCS>()._playerStat._hp + UpgradeStepHP;
                        PlayerA.GetComponent<PlayerCS>()._playerStat._hp = hp;
                        PlayerA.GetComponent<PlayerCS>()._playerStat.SaveInt(SystemValue.PlayerStatHPKey, hp);
                        MainSceneManager.GetComponent<MainSceneManagerCS>().AddScore(-upgradeHPAccounts);
                        Refresh();
                    }
                }
                else if(TraningState.UpgradePower == _traningState)
                {
                    // upgrage power
                    int upgradePowerAccounts = GetNextUpdatePowerAccounts();
                    if(Constants.SHOW_ME_THE_MONEY || upgradePowerAccounts <= score)
                    {
                        SetTraning(TraningState.UpgradePower);
                        
                        int power = PlayerA.GetComponent<PlayerCS>()._playerStat._power + UpgradeStepPower;
                        PlayerA.GetComponent<PlayerCS>()._playerStat._power = power;
                        PlayerA.GetComponent<PlayerCS>()._playerStat.SaveInt(SystemValue.PlayerStatPowerKey, power);
                        MainSceneManager.GetComponent<MainSceneManagerCS>().AddScore(-upgradePowerAccounts);
                        Refresh();
                    }
                }
                else if(TraningState.UpgradeSpeed == _traningState)
                {
                    // upgrage speed
                    int upgradeSpeedAccounts = GetNextUpdateSpeedAccounts();
                    if(Constants.SHOW_ME_THE_MONEY || upgradeSpeedAccounts <= score)
                    {
                        SetTraning(TraningState.UpgradeSpeed);
                        
                        float speed = PlayerA.GetComponent<PlayerCS>()._playerStat._speed + UpgradeStepSpeed;
                        PlayerA.GetComponent<PlayerCS>()._playerStat._speed = speed;
                        PlayerA.GetComponent<PlayerCS>()._playerStat.SaveFloat(SystemValue.PlayerStatSpeedKey, speed);
                        MainSceneManager.GetComponent<MainSceneManagerCS>().AddScore(-upgradeSpeedAccounts);
                        Refresh();
                    }
                }

                Snd_Upgrade.Play();

                SetTraning(TraningState.None);
            }
            _upgradeTime += Time.deltaTime;
            _attackMotionTime -= Time.deltaTime;
        }
    }
}
