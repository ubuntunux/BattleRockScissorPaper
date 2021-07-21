using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightTimerCS : MonoBehaviour
{
    public GameObject Bar;
    public GameObject AttackTypeA;
    public GameObject AttackTypeB;

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

    public void Reset()
    {
        SetAttackType(AttackType.None, true);
        SetAttackType(AttackType.None, false);

        setBar(1.0f);

        _shakeObjectRed.reset();
        _shakeObjectBlue.reset();
    }

    public void SetAttackType(AttackType attackType, bool isLeft)
    {
        GameObject obj = isLeft ? AttackTypeA : AttackTypeB;
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
    }

    public void SetAttackTimerShake(bool isLeft)
    {
        float shakeIntensity = 15.0f;
        ShakeObject obj = isLeft ? _shakeObjectRed : _shakeObjectBlue;
        obj.setShake(Constants.AttackHitTime, shakeIntensity, Constants.CameraShakeRandomTerm);
    }

    public void setBar(float ratio)
    {
        float prevRatio = _prevRatio;
        _prevRatio = ratio;

        Bar.transform.localScale = new Vector3(ratio, 1.0f, 1.0f);

        _positionRed = new Vector3(Mathf.Lerp(-25.0f, -125.0f, ratio), 1.0f, 1.0f);
        _positionBlue = new Vector3(Mathf.Lerp(25.0f, 125.0f, ratio), 1.0f, 1.0f);
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
