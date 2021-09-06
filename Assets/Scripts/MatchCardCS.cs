using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchCardCS : MonoBehaviour
{
    public GameObject Image_Portrait;
    static float BlendTime = 1.0f;

    MatchCardManagerCS _matchCardManager = null;
    
    GameObject _player;
    PlayerCS _skin;
    int _stageIndex = 0;
    bool _selected = false;
    float _colorTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().color = GetColor(false);
    }

    public int GetStageIndex()
    {
        return _stageIndex;
    }

    public GameObject GetPlayer()
    {
        return _player;
    }

    public PlayerCS GetSkin()
    {
        return _skin;
    }

    public int GetSkinID()
    {
        return _skin._playerStat._skinID;
    }

    public void SetSelected(bool selected)
    {
        _colorTime = BlendTime;
        _selected = selected;
    }

    Color GetColor(bool selected)
    {
        return selected ? new Color(1, 1, 1, 1) : new Color(0.0f, 0.0f, 0.0f, 0.5f);
    }

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener ((UnityEngine.Events.UnityAction) this.OnClick);
    }

    public void OnClick()
    {
        _matchCardManager.SelectMatchCard(this);
    }

    public void SetMatchCard(MatchCardManagerCS matchCardManager, int stageIndex, PlayerCS skin, GameObject player)
    {
        _matchCardManager = matchCardManager;        
        _stageIndex = stageIndex;
        _player = player;
        _skin = skin;
        Image_Portrait.GetComponent<Image>().sprite = skin.GetImagePortrait();
    }

    // Update is called once per frame
    void Update()
    {
        if(0.0f < _colorTime)
        {
            _colorTime -= Time.deltaTime;
            float ratio = Mathf.Max(0.0f, _colorTime / BlendTime);

            Color golaColor = GetColor(_selected);
            Color color = GetComponent<Image>().color;
            
            color.r = Mathf.Lerp(golaColor.r, color.r, ratio);
            color.g = Mathf.Lerp(golaColor.g, color.g, ratio);
            color.b = Mathf.Lerp(golaColor.b, color.b, ratio);
            color.a = Mathf.Lerp(golaColor.a, color.a, ratio);

            GetComponent<Image>().color = color;
        }
    }
}
