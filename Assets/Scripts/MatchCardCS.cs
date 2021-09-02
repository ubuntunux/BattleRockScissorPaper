using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchCardCS : MonoBehaviour
{
    public GameObject Image_Portrait;

    ChallengeSceneManagerCS _challengeSceneManager = null;
    int _stageIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener ((UnityEngine.Events.UnityAction) this.OnClick);
    }

    public void OnClick()
    {
        _challengeSceneManager.SelectChallengePlayer(_stageIndex);
        _challengeSceneManager.LayerMatchCardClick();
    }

    public void SetMatchCard(ChallengeSceneManagerCS challengeSceneManager, int stageIndex, PlayerCS player)
    {
        _challengeSceneManager = challengeSceneManager;
        _stageIndex = stageIndex;
        Image_Portrait.GetComponent<Image>().sprite = player.GetImagePortrait();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
