using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchCardCS : MonoBehaviour
{
    public GameObject Text_Rank;
    public GameObject Text_Price;
    public GameObject Image_Born;
    public GameObject Image_Portrait;
    public GameObject Text_Name;
    public GameObject Text_Record;
    public GameObject Btn_Challenge;
    public GameObject Btn_Select;
    public AudioSource Snd_Select;

    ChallengeSceneManagerCS _challengeSceneManager = null;
    int _stageIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        Btn_Challenge.GetComponent<Button>().onClick.AddListener ((UnityEngine.Events.UnityAction) this.OnClick);
        Btn_Select.GetComponent<Button>().onClick.AddListener ((UnityEngine.Events.UnityAction) this.OnClick);
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

        Text_Rank.GetComponent<TextMeshProUGUI>().text = "Rank: " + stageIndex.ToString();
        Text_Price.GetComponent<TextMeshProUGUI>().text = "Price: $10,000";
        Image_Born.GetComponent<Image>().sprite = player.GetImageBorn();
        Image_Portrait.GetComponent<Image>().sprite = player.GetImagePortrait();
        Text_Name.GetComponent<TextMeshProUGUI>().text = player.GetCharacterName();
        Text_Record.GetComponent<TextMeshProUGUI>().text = 
            player._playerStat._win.ToString() + " - " +
            player._playerStat._draw.ToString() + " - " +
            player._playerStat._lose.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
