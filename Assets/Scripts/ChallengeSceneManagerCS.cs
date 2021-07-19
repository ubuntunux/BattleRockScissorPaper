using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ChallengeState
{
    None,
    Versus,
}

public class ChallengeSceneManagerCS : MonoBehaviour
{
    public GameObject LayerPortrait;
    public GameObject Portrait00;
    public GameObject LayerVersus;

    ChallengeState _challengeState = ChallengeState.None;

    float _timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    void Reset()
    {
        LayerPortrait.SetActive(true);
        LayerVersus.SetActive(false);

        _timer = 0.0f;
        _challengeState = ChallengeState.None;
    }

    public void PortraitOnClick()
    {
        LayerPortrait.SetActive(false);
        LayerVersus.SetActive(true);
        
        _timer = 0.0f;
        _challengeState = ChallengeState.Versus;
    }

    // Update is called once per frame
    void Update()
    {
        if(ChallengeState.Versus == _challengeState)
        {
            if(3.0f < _timer)
            {
                SceneManager.LoadScene("FightScene");
            }
        }
        _timer += Time.deltaTime;
    }
}
