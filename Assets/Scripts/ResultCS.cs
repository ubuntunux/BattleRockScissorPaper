using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultCS : MonoBehaviour
{
    public GameObject MainSceneManager;
    public GameObject Result_PlayerA;
    public GameObject Result_PlayerB;
    public GameObject Text_Result;
    public GameObject LayerScore;
    public GameObject Text_Point;
    public GameObject Text_Vital;
    public GameObject Text_Bonus;
    public GameObject Text_Total;
    public GameObject Text_Score;
    
    public AudioSource Snd_Win;
    public AudioSource Snd_Lose;
    public AudioSource Snd_Draw;
    public AudioSource Snd_CoinLoop;

    float _timer = 0.0f;
    int _initialScore = 0;
    int _goalScore = 0;
    bool _isVersusScene = false;

    // Start is called before the first frame update
    void Start()
    {
        _timer = 0.0f;
    }

    void OnDisable()
    {
        Snd_CoinLoop.Stop();
    }

    public void ResetResultScene(
        PlayerCS playerA, 
        int playerA_Win, 
        PlayerCS playerB, 
        int playerB_Win, 
        bool isVersusScene, 
        int recordAttackPoint, 
        int recordHP,
        int bonus,        
        int totalScore)
    {
        _timer = 0.0f;
        _isVersusScene = isVersusScene;

        if(isVersusScene)
        {
            LayerScore.SetActive(false);
        }
        else
        {
            int score = MainSceneManager.GetComponent<MainSceneManagerCS>().GetScore();
            _initialScore = score - totalScore;
            _goalScore = score;

            if(0 < totalScore)
            {
                Snd_CoinLoop.Play();
            }

            Text_Point.GetComponent<TextMeshProUGUI>().text = recordAttackPoint.ToString();
            Text_Vital.GetComponent<TextMeshProUGUI>().text = recordHP.ToString();
            Text_Bonus.GetComponent<TextMeshProUGUI>().text = bonus.ToString();
            Text_Total.GetComponent<TextMeshProUGUI>().text = totalScore.ToString();
            Text_Score.GetComponent<TextMeshProUGUI>().text = _initialScore.ToString();
            LayerScore.SetActive(true);
        }

        Sprite spritePlayerA = null;
        Sprite spritePlayerB = null;

        Color DrawColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        Color WinColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        Color LoseColor = new Color(0.4f, 0.4f, 1.0f, 1.0f);

        float resultTextOffsetX = 200.0f;
        float resultTextPosX = -resultTextOffsetX;

        if(playerA_Win == playerB_Win)
        {
            Snd_Draw.Play();
            spritePlayerA = playerA.GetImagePortrait();
            spritePlayerB = playerB.GetImagePortrait();
            resultTextPosX = 0.0f;
            Text_Result.GetComponent<Text>().text = "Draw";
            Text_Result.GetComponent<Text>().color = DrawColor;
        }
        else if(playerA_Win < playerB_Win)
        {
            spritePlayerA = playerA.GetImagePortraitLose();
            spritePlayerB = playerB.GetImagePortrait();
            
            if(isVersusScene)
            {
                Snd_Win.Play();
                resultTextPosX = resultTextOffsetX;
                Text_Result.GetComponent<Text>().text = "Win";
                Text_Result.GetComponent<Text>().color = WinColor;
            }
            else
            {
                Snd_Lose.Play();
                Text_Result.GetComponent<Text>().text = "You Lose";
                Text_Result.GetComponent<Text>().color = LoseColor;
            }
        }
        else if(playerB_Win < playerA_Win)
        {
            Snd_Win.Play();
            spritePlayerA = playerA.GetImagePortrait();
            spritePlayerB = playerB.GetImagePortraitLose();
            if(isVersusScene)
            {
                Text_Result.GetComponent<Text>().text = "Win";
            }
            else
            {
                Text_Result.GetComponent<Text>().text = "You Win";
            }
            Text_Result.GetComponent<Text>().color = WinColor;
        }
        Text_Result.GetComponent<RectTransform>().anchoredPosition = new Vector2(resultTextPosX, Text_Result.GetComponent<RectTransform>().anchoredPosition.y);

        Result_PlayerA.GetComponent<VersusPortraitCS>().SetVersusPortrait(playerA.GetComponent<PlayerCS>().GetCharacterName(), spritePlayerA, null);
        Result_PlayerB.GetComponent<VersusPortraitCS>().SetVersusPortrait(playerB.GetComponent<PlayerCS>().GetCharacterName(), spritePlayerB, null);
    }

    // Update is called once per frame
    void Update()
    {
        if(false == _isVersusScene)
        {
            float scoreTime = Constants.GameResultTime * 0.5f;
            float ratio = Mathf.Min(1.0f, _timer / scoreTime);
            int score = (int)Mathf.Lerp((float)_initialScore, (float)_goalScore, ratio);
            Text_Score.GetComponent<TextMeshProUGUI>().text = score.ToString();
            if(scoreTime <= _timer && Snd_CoinLoop.isPlaying)
            {
                Snd_CoinLoop.Stop();
            }
        }

        if(_timer < Constants.GameResultTime)
        {
            _timer += Time.deltaTime;
        }
    }
}
