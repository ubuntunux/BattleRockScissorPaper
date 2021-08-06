using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerStat
{
    public PlayerStat(int level, int hp, int powerString, int powerWeak)
    {
        _level = level;
        _hp = hp;
        _powerString = powerString;
        _powerWeak = powerWeak;
    }

    public int GetPower(bool isWeakDamage)
    {
        return isWeakDamage ? _powerWeak : _powerString;
    }

    public int _level;
    public int _hp;
    public int _powerString;
    public int _powerWeak;
}

public class Constants : MonoBehaviour
{
    static public int[] Exps =
    {
        0,
        10,
        20,
        40,
        80,
    };

    static public PlayerStat[] PlayerStats =
    {
        // (level, hp, powerString, powerWeak)
        new PlayerStat(0, 1, 1, 1),
        new PlayerStat(1, 5, 2, 1),
        new PlayerStat(2, 5, 3, 1),
        new PlayerStat(3, 6, 4, 2),
        new PlayerStat(4, 7, 4, 2),
    };

    static public int DefaultSkinID = 1;

    static public float FadeTime = 0.25f;
    static public float AttackRandomTermMin = 0.1f;
    static public float AttackRandomTermMax = 1.0f;
    static public float AttackTimerTime = 2.0f;
    static public float AttackHitTimeDelay = 0.5f;
    static public float AttackMotionTime = 0.1f;
    static public float AttackHitTime = 0.5f;
    static public float RoundReadyTime = 3.0f;
    static public float RoundEndTime = 3.0f;
    static public float GameResultTime = 4.0f;
    static public float GroundPosition = 3.2f;
    static public float AttackDistance = 1.5f;
    static public float IdleDistance = 2.0f; 
    static public float SelectDistance = 4.0f;
    static public float CameraShakeDuration = 0.2f;
    static public float CameraShakeRandomTerm = 0.01f;
    static public float CameraShakeIntensity = 1.0f;
}
