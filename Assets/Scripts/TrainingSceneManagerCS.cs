using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingSceneManagerCS : MonoBehaviour
{
    public GameObject MainSceneManager;
    public GameObject PlayerA;
    public GameObject PlayerB;

    public GameObject Text_Health;
    public GameObject Text_Power;
    public GameObject Text_Speed;

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
        MainSceneManager.GetComponent<MainSceneManagerCS>().SetActivateScene(GameSceneType.SkinScene);
    }

    public void Btn_OnClick_PowerTraning()
    {
        int power = PlayerA.GetComponent<PlayerCS>()._playerStat._power + 1;
        PlayerA.GetComponent<PlayerCS>()._playerStat._power = power;
        PlayerA.GetComponent<PlayerCS>()._playerStat.SaveInt(SystemValue.PlayerStatPowerKey, power);
        MainSceneManager.GetComponent<MainSceneManagerCS>().AddScore(-1000);
        Refresh();
    }

    public void Btn_OnClick_HealthTraning()
    {
        int hp = PlayerA.GetComponent<PlayerCS>()._playerStat._hp + 1;
        PlayerA.GetComponent<PlayerCS>()._playerStat._hp = hp;
        PlayerA.GetComponent<PlayerCS>()._playerStat.SaveInt(SystemValue.PlayerStatHPKey, hp);
        MainSceneManager.GetComponent<MainSceneManagerCS>().AddScore(-1000);
        Refresh();
    }

    public void Btn_OnClick_SpeedTraning()
    {
        float speed = PlayerA.GetComponent<PlayerCS>()._playerStat._speed + 1.0f;
        PlayerA.GetComponent<PlayerCS>()._playerStat._speed = speed;
        PlayerA.GetComponent<PlayerCS>()._playerStat.SaveFloat(SystemValue.PlayerStatSpeedKey, speed);
        MainSceneManager.GetComponent<MainSceneManagerCS>().AddScore(-1000);
        Refresh();
    }

    public void Refresh()
    {
        Text_Health.GetComponent<Text>().text = "Health: " + PlayerA.GetComponent<PlayerCS>()._playerStat._hp.ToString();
        Text_Power.GetComponent<Text>().text = "Power: " + PlayerA.GetComponent<PlayerCS>()._playerStat._power.ToString();
        Text_Speed.GetComponent<Text>().text = "Speed: " + PlayerA.GetComponent<PlayerCS>()._playerStat._speed.ToString();
    }

    void ResetTrainingScene()
    {
        PlayerA.SetActive(true);
        PlayerA.GetComponent<PlayerCS>().SetStateIdle();
        
        PlayerB.SetActive(false);
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        if(MainSceneManager.GetComponent<MainSceneManagerCS>().GetActivateSceneType() == GameSceneType.TrainingScene)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Exit();
            }
        }
    }
}
