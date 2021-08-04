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
        for(int i=0; i<skinCount; ++i)
        {
            PlayerCS skin = skins[i].GetComponent<PlayerCS>();
            skinMap.Add(skin.SkinID, skin);
        }
    }

    public PlayerCS GetSkin(int skinID)
    {
        return skinMap[skinID];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
