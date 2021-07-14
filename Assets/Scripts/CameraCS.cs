using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeObject
{
    bool _enableShake = false;
    Vector3 _shakeOffset;
    Vector3 _prevShakeOffset;
    float _shakeDuration = 0.0f;
    float _shakeTime = 0.0f;    
    float _shakeRandomTerm = 0.01f;
    float _shakeRandomTime = 0.0f;
    float _shakeIntensity = 1.0f;

    public void reset()
    {
        _enableShake = false;
        _shakeOffset = new Vector3(0.0f, 0.0f, 0.0f);
        _prevShakeOffset = new Vector3(0.0f, 0.0f, 0.0f);
        _shakeDuration = 0.0f;
        _shakeTime = 0.0f;
        _shakeRandomTerm = 0.01f;
        _shakeRandomTime = 0.0f;
    }

    public void setShake(float shakeDuration, float shakeIntensity, float shakeRandomTerm)
    {
        _enableShake = true;
        _shakeDuration = shakeDuration;
        _shakeTime = shakeDuration;
        _shakeIntensity = shakeIntensity;
        _shakeRandomTerm = shakeRandomTerm;
        _shakeRandomTime = 0.0f;
        _shakeOffset = new Vector3(0.0f, 0.0f, 0.0f);
        _prevShakeOffset = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public void updateShakeObject(ref Vector3 cameraOffset)
    {
        if(_enableShake)
        {
            bool isLoop = _shakeDuration <= 0.0f;
            if (isLoop || 0.0f < _shakeTime)
            {
                if(_shakeRandomTime <= 0.0f)
                {
                    _prevShakeOffset = _shakeOffset;
                    _shakeOffset = (Vector3)Random.insideUnitCircle * _shakeIntensity;
                    _shakeRandomTime = _shakeRandomTerm;
                }

                float shakeIntensityByTime = isLoop ? 1.0f : (_shakeTime / _shakeDuration);
                float t = (0.0f == _shakeRandomTerm) ? 1.0f : (1.0f - _shakeRandomTime / _shakeRandomTerm);
                cameraOffset += Vector3.Lerp(_prevShakeOffset, _shakeOffset, t) * shakeIntensityByTime;
                _shakeRandomTime -= Time.deltaTime;

                if(false == isLoop)
                {
                    _shakeTime -= Time.deltaTime;
                }
            }
        }
    }
}

public class CameraCS : MonoBehaviour
{
    Vector3 _originPos;
    ShakeObject _cameraShake = new ShakeObject();
    ShakeObject _cameraHandMove = new ShakeObject();

    // Start is called before the first frame update
    void Start () {
        reset();
    }

    public void reset()
    {
        _originPos = transform.localPosition;
        _cameraShake.reset();
        _cameraHandMove.reset();
        _cameraHandMove.setShake(0.0f, 0.5f, 3.0f);
    }

    public void setShake()
    {
        _cameraShake.setShake(Constants.CameraShakeDuration, Constants.CameraShakeIntensity, Constants.CameraShakeRandomTerm);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraOffset = new Vector3(0.0f, 0.0f, 0.0f);

        _cameraShake.updateShakeObject(ref cameraOffset);
        _cameraHandMove.updateShakeObject(ref cameraOffset);

        transform.localPosition = _originPos + cameraOffset;
    }
}
