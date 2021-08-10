using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SkinManagerCS : MonoBehaviour
{
    public GameObject[] skins;

    Dictionary<int, PlayerCS> skinMap = new Dictionary<int, PlayerCS>();

    // Start is called before the first frame update
    void Start()
    {
        int skinCount = skins.Length;
        for(int i = 0; i < skinCount; ++i)
        {
            PlayerCS playerSkin = skins[i].GetComponent<PlayerCS>();
            playerSkin.LoadPlayerStat();
            skinMap.Add(playerSkin.SkinID, playerSkin);
        }
    }

    public PlayerCS GetSkin(int skinID)
    {
        PlayerCS skin = skinMap[skinID];
        skin.LoadPlayerStat();
        return skin;
    }

    public void ClearPlayerStats()
    {
        int skinCount = skins.Length;
        for(int i = 0; i < skinCount; ++i)
        {
            PlayerCS playerSkin = skins[i].GetComponent<PlayerCS>();
            playerSkin.InitializePlayerStat();
            playerSkin.SavePlayerStat();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
