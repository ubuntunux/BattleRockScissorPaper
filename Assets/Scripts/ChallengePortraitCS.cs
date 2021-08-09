using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ChallengePortraitState
{
    None,
    Lose,
    Lock
}

public class ChallengePortraitCS : MonoBehaviour
{
    PlayerCS _challengePlayer;

    public GameObject _name;
    public GameObject _portrait;
    public GameObject _portraitLose;
    public GameObject _imageLose;
    public GameObject _imageLock;

    public Sprite Sprite_BackGround;
    public Sprite Sprite_BackGround_Selected;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Reset()
    {
        SetPortraitState(ChallengePortraitState.None);
        SetSelected(false);
    }

    public void SetPortraitState(ChallengePortraitState state)
    {
        bool isLose = ChallengePortraitState.Lose == state;
        _portrait.SetActive(false == isLose);
        _portraitLose.SetActive(isLose);
        _imageLose.SetActive(isLose);
        _imageLock.SetActive(ChallengePortraitState.Lock == state);
    }

    public void SetSelected(bool selected)
    {
        GetComponent<Button>().enabled = !selected;
        //GetComponent<Image>().sprite = selected ? Sprite_BackGround_Selected : Sprite_BackGround;
    }

    public PlayerCS GetChallengePlayer()
    {
        return _challengePlayer;
    }

    public void SetChallengePlayerPortrait(PlayerCS challengePlayer)
    {
        _challengePlayer = challengePlayer;
        _portrait.GetComponent<Image>().sprite = challengePlayer.GetImagePortrait();
        _portraitLose.GetComponent<Image>().sprite = challengePlayer.GetImagePortraitLose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
