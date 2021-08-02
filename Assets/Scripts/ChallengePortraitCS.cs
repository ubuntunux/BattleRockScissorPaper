using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengePortraitCS : MonoBehaviour
{
    ChallengePlayerCS _challengePlayer;

    public GameObject _name;
    public GameObject _portrait;
    public GameObject _portraitLose;
    public GameObject _imageLose;

    public Sprite Sprite_BackGround;
    public Sprite Sprite_BackGround_Selected;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Reset()
    {
        _portrait.SetActive(true);
        _portraitLose.SetActive(false);
        _imageLose.SetActive(false);
        SetSelected(false);
    }

    public void SetLose()
    {
        _portrait.SetActive(false);
        _portraitLose.SetActive(true);
        _imageLose.SetActive(true);
    }

    public void SetSelected(bool selected)
    {
        GetComponent<Button>().enabled = !selected;
        GetComponent<Image>().sprite = selected ? Sprite_BackGround_Selected : Sprite_BackGround;
    }

    public ChallengePlayerCS GetChallengePlayer()
    {
        return _challengePlayer;
    }

    public void SetChallengePlayer(ChallengePlayerCS challengePlayer)
    {
        _challengePlayer = challengePlayer;
        _name.GetComponent<Text>().text = challengePlayer.GetCharacterName();
        _portrait.GetComponent<Image>().sprite = challengePlayer.GetImagePortrait();
        _portraitLose.GetComponent<Image>().sprite = challengePlayer.GetImagePortraitLose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
