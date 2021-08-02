using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengePlayerCS : MonoBehaviour
{
    public int _hp = 5;
    public int _power = 2;
    public GameObject _skin;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetCharacterName()
    {
        return _skin.GetComponent<PlayerCS>().GetCharacterName();
    }

    public Sprite GetImagePortrait()
    {
        return _skin.GetComponent<PlayerCS>().GetImagePortrait();
    }

    public Sprite GetImagePortraitLose()
    {
        return _skin.GetComponent<PlayerCS>().GetImagePortraitLose();
    }
}
