using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightTimerCS : MonoBehaviour
{
    public GameObject Bar;
    public GameObject GloveRed;
    public GameObject GloveBlue;

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
        setBar(1.0f);

        _shakeObjectRed.reset();
        _shakeObjectBlue.reset();
    }

    public void setBar(float ratio)
    {
        float prevRatio = _prevRatio;
        _prevRatio = ratio;

        Bar.transform.localScale = new Vector3(ratio, 1.0f, 1.0f);

        if(0.0f == ratio && prevRatio != ratio)
        {
            float shakeIntensity = 15.0f;
            _shakeObjectRed.setShake(Constants.AttackHitTime, shakeIntensity, Constants.CameraShakeRandomTerm);
            _shakeObjectBlue.setShake(Constants.AttackHitTime, shakeIntensity, Constants.CameraShakeRandomTerm);
        }

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

        GloveRed.transform.localPosition = _positionRed + shakeOffsetRed;
        GloveBlue.transform.localPosition = _positionBlue + shakeOffsetBlue;
    }
}
