using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultCS : MonoBehaviour
{
    public GameObject Result_PlayerA;
    public GameObject Result_PlayerB;
    public GameObject Text_Result;
    
    public AudioSource Snd_Win;
    public AudioSource Snd_Lose;
    public AudioSource Snd_Draw;

    float _timer = 0.0f;
    bool _isEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        _timer = 0.0f;
        _isEnd = false;
    }

    public void Reset(PlayerCS playerA, int playerA_Win, PlayerCS playerB, int playerB_Win)
    {
        _timer = 0.0f;
        _isEnd = false;

        Sprite spritePlayerA = null;
        Sprite spritePlayerB = null;

        if(playerA_Win == playerB_Win)
        {
            Snd_Draw.Play();
            spritePlayerA = playerA.GetImagePortrait();
            spritePlayerB = playerB.GetImagePortrait();
            Text_Result.GetComponent<Text>().text = "Draw";
            Text_Result.GetComponent<Text>().color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        }
        else if(playerA_Win < playerB_Win)
        {
            Snd_Lose.Play();
            spritePlayerA = playerA.GetImagePortraitLose();
            spritePlayerB = playerB.GetImagePortrait();
            Text_Result.GetComponent<Text>().text = "You Lose";
            Text_Result.GetComponent<Text>().color = new Color(0.4f, 0.4f, 1.0f, 1.0f);
        }
        else if(playerB_Win < playerA_Win)
        {
            Snd_Win.Play();
            spritePlayerA = playerA.GetImagePortrait();
            spritePlayerB = playerB.GetImagePortraitLose();
            Text_Result.GetComponent<Text>().text = "You Win";
            Text_Result.GetComponent<Text>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }

        Result_PlayerA.GetComponent<VersusPortraitCS>().SetVersusPortrait(playerA.GetComponent<PlayerCS>().GetCharacterName(), spritePlayerA, null);
        Result_PlayerB.GetComponent<VersusPortraitCS>().SetVersusPortrait(playerB.GetComponent<PlayerCS>().GetCharacterName(), spritePlayerB, null);
    }

    public bool isEnd()
    {
        return _isEnd;
    }

    // Update is called once per frame
    void Update()
    {
        if(false == _isEnd)
        {
            if(4.0f < _timer)
            {
                _isEnd = true;
            }
            _timer += Time.deltaTime;
        }
    }
}
