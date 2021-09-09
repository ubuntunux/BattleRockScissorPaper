using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightTimerCS : MonoBehaviour
{
    public GameObject Bar;
    public GameObject AttackTypeA;
    public GameObject AttackTypeB;

    public GameObject LayerPowerGuageA;
    public GameObject LayerPowerGuageB;

    public Sprite Sprite_AttackNone;
    public Sprite Sprite_Rock;
    public Sprite Sprite_Scissor;
    public Sprite Sprite_Paper;

    float _prevRatio = 0.0f;
    ShakeObject _shakeObjectRed = new ShakeObject();
    ShakeObject _shakeObjectBlue = new ShakeObject();

    Vector3 _positionRed;
    Vector3 _positionBlue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ResetFightTimer()
    {
        SetAttackType(AttackType.None, true);
        SetAttackType(AttackType.None, false);

        setBar(1.0f);

        _shakeObjectRed.reset();
        _shakeObjectBlue.reset();

        LayerPowerGuageA.GetComponent<PowerGaugeCS>().ResetPowerGauge();
        LayerPowerGuageB.GetComponent<PowerGaugeCS>().ResetPowerGauge();
    }

    public void SetAttackType(AttackType attackType, bool isPlayerA)
    {
        GameObject obj = isPlayerA ? AttackTypeA : AttackTypeB;
        GameObject layerPowerGuage = isPlayerA ? LayerPowerGuageA : LayerPowerGuageB;

        if(AttackType.Rock == attackType)
        {
            obj.GetComponent<Image>().sprite = Sprite_Rock;
        }
        else if(AttackType.Scissor == attackType)
        {
            obj.GetComponent<Image>().sprite = Sprite_Scissor;
        }
        else if(AttackType.Paper == attackType)
        {
            obj.GetComponent<Image>().sprite = Sprite_Paper;
        }
        else
        {
            obj.GetComponent<Image>().sprite = Sprite_AttackNone;
        }

        if(false == isPlayerA)
        {
            layerPowerGuage.GetComponent<PowerGaugeCS>().SetPowerGauage(Random.value);
        }
        layerPowerGuage.GetComponent<PowerGaugeCS>().SetPause();
    }

    public void SetAttackTimerShake(bool isPlayerA)
    {
        float shakeIntensity = 15.0f;
        ShakeObject obj = isPlayerA ? _shakeObjectRed : _shakeObjectBlue;
        obj.setShake(Constants.AttackHitTime, shakeIntensity, Constants.CameraShakeRandomTerm);
    }

    public float GetPowerGuage(bool isPlayerA)
    {
        return (isPlayerA ? LayerPowerGuageA : LayerPowerGuageB).GetComponent<PowerGaugeCS>().GetPowerGuage();
    }

    public void setBar(float ratio)
    {
        float prevRatio = _prevRatio;
        _prevRatio = ratio;

        Bar.transform.localScale = new Vector3(ratio, 1.0f, 1.0f);

        _positionRed = new Vector3(Mathf.Lerp(-25.0f, -125.0f, ratio), 1.0f, 1.0f);
        _positionBlue = new Vector3(Mathf.Lerp(25.0f, 125.0f, ratio), 1.0f, 1.0f);

        float gaugeRatio = (ratio * 2.0f) % 1.0f;
        LayerPowerGuageA.GetComponent<PowerGaugeCS>().SetPowerGauage(gaugeRatio);
        LayerPowerGuageB.GetComponent<PowerGaugeCS>().SetPowerGauage(gaugeRatio);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 shakeOffsetRed = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 shakeOffsetBlue = new Vector3(0.0f, 0.0f, 0.0f);

        _shakeObjectRed.updateShakeObject(ref shakeOffsetRed);
        _shakeObjectBlue.updateShakeObject(ref shakeOffsetBlue);

        AttackTypeA.transform.localPosition = _positionRed + shakeOffsetRed;
        AttackTypeB.transform.localPosition = _positionBlue + shakeOffsetBlue;
    }
}
