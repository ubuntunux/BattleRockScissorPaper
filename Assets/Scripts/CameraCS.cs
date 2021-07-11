using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCS : MonoBehaviour
{
    Vector3 originPos;
    Vector3 shakeOffset;
    Vector3 prevShakeOffset;
    float shakeDuration = 0.2f;
    float shakeTime = 0.0f;
    float shakeRandomTerm = 0.01f;
    float shakeRandomTime = 0.0f;

    // Start is called before the first frame update
    void Start () {
        originPos = transform.localPosition;
    }
    public void setShake()
    {
        shakeTime = shakeDuration;
        shakeRandomTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < shakeTime)
        {
            if(shakeRandomTime <= 0.0f)
            {
                prevShakeOffset = shakeOffset;
                shakeOffset = (Vector3)Random.insideUnitCircle;
                shakeRandomTime = shakeRandomTerm;
            }

            float shakeIntensity = shakeTime / shakeDuration;
            float t = 1.0f - shakeRandomTime / shakeRandomTerm;
            transform.localPosition = originPos + Vector3.Lerp(shakeOffset, prevShakeOffset, t) * shakeIntensity;
            shakeRandomTime -= Time.deltaTime;
            shakeTime -= Time.deltaTime;

            if(shakeTime <= 0.0f)
            {
                transform.localPosition = originPos;
            }
        }
    }
}
