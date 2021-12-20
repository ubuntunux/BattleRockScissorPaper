using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinCardCS : MonoBehaviour
{
    public GameObject Image_Portrait;
    public GameObject Image_BG;
    public GameObject Image_Born;
    public GameObject Image_Lock;
    public GameObject Text_Name;
    public GameObject Btn_Accounts;    
    public GameObject Text_Accounts;
    public GameObject Btn_Advertisement;
    public GameObject Text_Advertisement;

    SkinManagerCS _skinManager;
    PlayerCS _skin;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Awake()
    {
        Image_BG.GetComponent<Button>().onClick.AddListener ((UnityEngine.Events.UnityAction) this.OnClick);
    }

    public void OnClick()
    {
        if(_skin._playerStat._purchased)
        {
            _skinManager.SetPlayerSkin(_skin);
        }
    }

    public void OnClickAccounts()
    {
        if(false == _skin._playerStat._purchased)
        {
            _skinManager.PurchaseSkinByAccounts(this);
        }
    }

    public void OnClickAdvertisement()
    {
        if(false == _skin._playerStat._purchased)
        {
            _skinManager.PurchaseSkinByAdvertisement(this);
        }
    }

    public void CallbackPurchaseSkinByAdvertisement()
    {
        PlayerCS skin = GetSkin();
        if(skin._playerStat._advertisement < skin.Advertisement)
        {   
            skin._playerStat.SetAdvertisement(skin._playerStat._advertisement + 1);
            if(skin.Advertisement <= skin._playerStat._advertisement)
            {
                PurchaseSkinCard();
            }
            else
            {
                ResetSkinCard();
            }
        }
    }

    public PlayerCS GetSkin()
    {
        return _skin;
    }

    void SetAccountsText()
    {
        Text_Accounts.GetComponent<TextMeshProUGUI>().text = string.Format("{0: #,###; -#,###;0}", _skin.Accounts);
    }

    void SetAdvertisementText()
    {
        int advertisement = _skin._playerStat._advertisement;
        int maxAdvertisement = _skin.Advertisement;
        Text_Advertisement.GetComponent<TextMeshProUGUI>().text = "(" + advertisement.ToString() + "/" + maxAdvertisement.ToString() + ")";
    }

    public void PurchaseSkinCard()
    {
        _skin._playerStat.SetPurchased(true);

        ResetSkinCard();
    }

    public void ResetSkinCard()
    {
        _skin._playerStat.LoadPlayerStat();

        bool isDefaultSkin = Constants.DefaultSkinID == _skin._playerStat._skinID;
        
        bool purchased = _skin._playerStat._purchased || isDefaultSkin;
        bool locked = !purchased && _skin._playerStat._locked;
        Image_Lock.SetActive(locked);
        
        bool needPurchased = !locked && !purchased;
        Btn_Accounts.SetActive(needPurchased);
        //Btn_Advertisement.SetActive(needPurchased && 0 < _skin.Advertisement);

        Color color = (locked || needPurchased) ? new Color(0.4f, 0.4f, 0.4f, 1) : new Color(1, 1, 1, 1);
        Image_Portrait.GetComponent<Image>().color = color;
        Image_BG.GetComponent<Image>().color = color;

        SetAccountsText();
        SetAdvertisementText();
    }

    public void SetSkinCard(SkinManagerCS skinManager, PlayerCS skin)
    {
        _skinManager = skinManager;
        _skin = skin;

        Text_Name.GetComponent<TextMeshProUGUI>().text = _skin.GetCharacterName();
        Image_Portrait.GetComponent<Image>().sprite = _skin.GetImagePortrait();
        Image_Born.GetComponent<Image>().sprite = _skin.GetImageBorn();

        ResetSkinCard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
