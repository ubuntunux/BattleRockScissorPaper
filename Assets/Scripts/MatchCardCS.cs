using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchCardCS : MonoBehaviour
{
    public GameObject Image_Portrait;
    public GameObject Image_Lock;

    MatchCardManagerCS _matchCardManager = null;
    
    GameObject _player;
    PlayerCS _skin;

    static float BlendTime = 1.0f;
    int _stageIndex = 0;
    bool _unlocked = false;
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
        return selected ? new Color(1, 1, 1, 1) : new Color(0.4f, 0.4f, 0.4f, 1.0f);
    }

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener ((UnityEngine.Events.UnityAction) this.OnClick);
    }

    public void OnClick()
    {
        if(Constants.TEST_SELECT_CARD)
        {
            _matchCardManager.SelectMatchCard(this);
        }
        else
        {
            if(_unlocked)
            {
                _matchCardManager.SelectMatchCard(this);
            }
            else
            {
                _matchCardManager.UnlockMatchCard(this);
            }
        }
    }

    public void SetMatchCard(MatchCardManagerCS matchCardManager, int stageIndex, bool unlocked, PlayerCS skin, GameObject player)
    {
        _matchCardManager = matchCardManager;
        _stageIndex = stageIndex;
        _player = player;
        _skin = skin;
        _unlocked = unlocked;
        _colorTime = 0.0f;
        Image_Portrait.GetComponent<Image>().sprite = skin.GetImagePortrait();
        Image_Lock.SetActive(!unlocked);
    }

    // Update is called once per frame
    void Update()
    {
        if(0.0f <= _colorTime)
        {
            _colorTime -= Time.deltaTime;
            float ratio = Mathf.Max(0.0f, _colorTime / BlendTime);

            Color goalColor = GetColor(_selected);
            Color color = GetComponent<Image>().color;
            
            color.r = Mathf.Lerp(goalColor.r, color.r, ratio);
            color.g = Mathf.Lerp(goalColor.g, color.g, ratio);
            color.b = Mathf.Lerp(goalColor.b, color.b, ratio);
            color.a = Mathf.Lerp(goalColor.a, color.a, ratio);
            GetComponent<Image>().color = color;
            Image_Portrait.GetComponent<Image>().color = color;
        }
    }
}
