using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum VersusState
{
    None,
    SpeakPlayerA,
    SpeakVersus,
    SpeakPlayerB,
    End,
}

public class VersusCS : MonoBehaviour
{
    public GameObject PlayerA;
    public GameObject PlayerB;
    
    public AudioSource Snd_Versus;

    float _timer = 0.0f;
    VersusState _state = VersusState.None;

    // Start is called before the first frame update
    void Start()
    {
        _timer = 0.0f;
        _state = VersusState.None;
    }

    public void ResetVersus(PlayerCS playerA, PlayerCS playerB)
    {
        _timer = 0.0f;
        _state = VersusState.None;
        SetVersusPortraitPlayerA(playerA);
        SetVersusPortraitPlayerB(playerB);
    }
    
    public void SetVersusPortraitPlayerA(PlayerCS player)
    {
        string name = player.GetCharacterName();
        Sprite portrait = player.GetImagePortrait();
        AudioClip audioClip = player.GetAudioClip_CharacterName();
        PlayerA.GetComponent<VersusPortraitCS>().SetVersusPortrait(name, portrait, audioClip);
    }

    public void SetVersusPortraitPlayerB(PlayerCS player)
    {
        string name = player.GetCharacterName();
        Sprite portrait = player.GetImagePortrait();
        AudioClip audioClip = player.GetAudioClip_CharacterName();
        PlayerB.GetComponent<VersusPortraitCS>().SetVersusPortrait(name, portrait, audioClip);
    }

    public bool isEnd()
    {
        return VersusState.End == _state;
    }

    // Update is called once per frame
    void Update()
    {
        if(VersusState.End != _state)
        {
            if(VersusState.None == _state)
            {
                PlayerA.GetComponent<VersusPortraitCS>().PlayAudioName();
                _state = VersusState.SpeakPlayerA;
            }
            else if(VersusState.SpeakPlayerA == _state)
            {
                if(1.5f <=_timer)
                {
                    Snd_Versus.Play();
                    _state = VersusState.SpeakVersus;
                }
            }
            else if(VersusState.SpeakVersus == _state)
            {
                if(2.5f <=_timer)
                {
                    PlayerB.GetComponent<VersusPortraitCS>().PlayAudioName();
                    _state = VersusState.SpeakPlayerB;
                }
            }
            else if(VersusState.SpeakPlayerB == _state)
            {
                if(4.0f <=_timer)
                {
                    _state = VersusState.End;
                }
            }
            _timer += Time.deltaTime;
        }
    }
}
