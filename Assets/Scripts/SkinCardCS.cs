using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinCardCS : MonoBehaviour
{
    public GameObject Image_Portrait;
    public GameObject Image_Born;
    public GameObject Text_Name;

    SkinManagerCS _skinManager;
    PlayerCS _skin;

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
        _skinManager.SetPlayerSkin(_skin);
    }

    public PlayerCS GetSkin()
    {
        return _skin;
    }

    public void SetSkinCard(SkinManagerCS skinManager, PlayerCS skin)
    {
        _skinManager = skinManager;
        _skin = skin;

        Text_Name.GetComponent<TextMeshProUGUI>().text = _skin.GetCharacterName();
        Image_Portrait.GetComponent<Image>().sprite = _skin.GetImagePortrait();
        Image_Born.GetComponent<Image>().sprite = _skin.GetImageBorn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
