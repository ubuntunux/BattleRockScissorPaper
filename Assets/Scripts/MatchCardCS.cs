using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchCardCS : MonoBehaviour
{
    public GameObject Image_Portrait;

    MatchCardManagerCS _matchCardManager = null;
    
    GameObject _player;
    int _stageIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        SetDisabledColor();
    }

    public int getStageIndex()
    {
        return _stageIndex;
    }

    public GameObject getPlayer()
    {
        return _player;
    }

    public void SetSelectedColor()
    {
        GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    public void SetDisabledColor()
    {
        GetComponent<Image>().color = new Color(0, 0, 0, 1);
    }

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener ((UnityEngine.Events.UnityAction) this.OnClick);
    }

    public void OnClick()
    {
        _matchCardManager.MatchCardOnClick(this);
    }

    public void SetMatchCard(MatchCardManagerCS matchCardManager, int stageIndex, PlayerCS skin, GameObject player)
    {
        _matchCardManager = matchCardManager;        
        _stageIndex = stageIndex;
        _player = player;
        Image_Portrait.GetComponent<Image>().sprite = skin.GetImagePortrait();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
