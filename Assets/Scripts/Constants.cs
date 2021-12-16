using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    static public bool TEST_SELECT_CARD = false;
    static public bool CALCULATE_RANK = false;
    static public bool SHOW_ME_THE_MONEY = false;

    static public int DefaultSkinID = 6;
    static public int DefaultAdvertiseNum = 3;
    static public int DefaultAccounts = 900;
    static public int DefaultHP = 30;
    static public int DefaultPower = 10;
    static public float DefaultSpeed = 1.0f;

    static public int MaxHP = 300;
    static public int MaxPower = 100;
    static public float MaxSpeed = 10.0f;

    static public float FadeTime = 0.25f;
    static public float AttackRandomTermMin = 0.1f;
    static public float AttackRandomTermMax = 1.0f;
    static public float AttackTimerTime = 2.0f;
    static public float AttackHitTimeDelay = 0.5f;
    static public float AttackMotionTime = 0.1f;
    static public float AnnoyingTime = 0.3f;
    static public float AttackHitTime = 0.5f;
    static public float RoundTime = 10.0f;
    static public float RoundReadyTime = 3.0f;
    static public float RoundEndTime = 3.0f;
    static public float GameResultTime = 5.0f;
    static public float GroundPosition = 3.2f;
    static public float GroggyHitTime = 3.0f;
    static public float AttackDistance = 1.5f;
    static public float IdleDistance = 2.0f; 
    static public float SelectDistance = 4.0f;
    static public float CameraShakeDuration = 0.2f;
    static public float CameraShakeRandomTerm = 0.01f;
    static public float CameraShakeIntensity = 1.0f;
    static public float CriticalPowerGuage = 0.95f;
    static public int CriticalDamageRatio = 2;
    static public float MinPowerGuage = 0.2f;
    static public int WarningHPDivide = 3;
    static public int GroggyHPDivide = 5;
    static public int TimePoint = 100;
    static public float TraningTime = 2.0f;
}
