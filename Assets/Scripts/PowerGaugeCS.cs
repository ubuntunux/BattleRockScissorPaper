using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerGaugeCS : MonoBehaviour
{
    public GameObject PowerGauage;

    float _powerRatio = 0.0f;
    float _speed = 1.0f;
    bool _pause = false;

    Color HSVtoRGB(float Hue, float Saturate,float Value, float alpha)
    {
        float hueAngle = Hue * 360.0f;
        float C = Saturate * Value;
        float X = C * (1.0f - Mathf.Abs(((hueAngle / 60.0f) % 2.0f) - 1.0f));
        float m = Value - C;
        
        float r = m;
        float g = m;
        float b = m;

        if(0.0f <= hueAngle && hueAngle < 60.0f)
        {
            r += C;
            g += X;
        }
        else if(60.0f <= hueAngle && hueAngle < 120.0f)
        {
            r += X;
            g += C;
        }
        else if(120.0f <= hueAngle && hueAngle < 180.0f)
        {
            g += C;
            b += X;
        }
        else if(180.0f <= hueAngle && hueAngle < 240.0f)
        {
            g += X;
            b += C;
        }
        else if(240.0f <= hueAngle && hueAngle < 300.0f)
        {
            r += X;
            b += C;
        }
        else
        {
            r += C;
            b += X;
        }

        return new Color(r, g, b, alpha);
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetPowerGauge();
    }

    public void ResetPowerGauge()
    {
        _pause = false;
    }

    public float GetPowerGuage()
    {
        return _powerRatio;
    }

    public void SetPause()
    {
        _pause = true;
    }

    public void SetPowerGauage(float ratio)
    {
        if(null != PowerGauage && false == _pause)
        {
            _powerRatio = ratio;

            PowerGauage.transform.localScale = new Vector3(_powerRatio, _powerRatio, 1.0f);
            float hue = Mathf.Lerp(0.6666666f, 0.1666666f, _powerRatio);
            float saturate = Mathf.Lerp(0.3f, 1.0f, _powerRatio);
            float alpha = 1.0f;
            PowerGauage.GetComponent<Image>().color = HSVtoRGB(hue, saturate, 1.0f, alpha);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
