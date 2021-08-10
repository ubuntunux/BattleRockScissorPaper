using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemValue : MonoBehaviour
{
    public static string SkinIDKey = "SkinID";

    // player stat
    public static string PlayerStatPerfectKey = "PlayerStatPerfect";
    public static string PlayerStatWinKey = "PlayerStatWin";
    public static string PlayerStatLoseKey = "PlayerStatLose";
    public static string PlayerStatScoreKey = "PlayerStatScore";    
    public static string PlayerStatHPKey = "PlayerStatHP";
    public static string PlayerStatPowerKey = "PlayerStatPower";    
    public static string PlayerStatSpeedKey = "PlayerStatSpeed";

    // challenge
    public static string ChallengeScoreKey = "ChallengeScore";

    public static bool GetBool(string key, bool defaultValue = false)
    {
        return (0 != PlayerPrefs.GetInt(key, defaultValue ? 1 : 0));
    }

    public static void SetBool(string key, bool value, bool save = true)
    {
        bool prevValue = GetBool(key);
        if(prevValue != value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
            if(save)
            {
                PlayerPrefs.Save();
            }
        }
    }

    public static int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public static void SetInt(string key, int value, bool save = true)
    {
        int prevValue = GetInt(key);
        if(prevValue != value)
        {
            PlayerPrefs.SetInt(key, value);
            if(save)
            {
                PlayerPrefs.Save();
            }
        }
    }

    public static float GetFloat(string key, float defaultValue = 0.0f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public static void SetFloat(string key, float value, bool save = true)
    {
        float prevValue = GetFloat(key);
        if(prevValue != value)
        {
            PlayerPrefs.SetFloat(key, value);
            if(save)
            {
                PlayerPrefs.Save();
            }
        }
    }

    public static Vector3 GetVector3(string key, Vector3 defaultValue)
    {
        Vector3 value = Vector3.zero;
        value.x = GetFloat(key + "X", defaultValue.x);
        value.y = GetFloat(key + "Y", defaultValue.y);
        value.z = GetFloat(key + "Z", defaultValue.z);
        return value;
    }

    public static void SetVector3(string key, Vector3 value)
    {
        SetFloat(key + "X", value.x);
        SetFloat(key + "Y", value.y);
        SetFloat(key + "Z", value.z);
    }

    public static string GetString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public static void SetString(string key, string value, bool save = true)
    {
        string prevValue = GetString(key);
        if(prevValue != value)
        {
            PlayerPrefs.SetString(key, value);
            if(save)
            {
                PlayerPrefs.Save();
            }
        }
    }
}