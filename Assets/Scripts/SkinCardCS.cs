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

        bool purchased =_skin._playerStat._purchased;
        if(purchased)
        {
            Btn_Accounts.SetActive(false);
            Btn_Advertisement.SetActive(false);
            Image_Lock.SetActive(false);
        }
        else
        {
            Btn_Accounts.SetActive(true);
            Btn_Advertisement.SetActive(true);
            Image_Lock.SetActive(true);

            Text_Accounts.GetComponent<TextMeshProUGUI>().text = string.Format("{0: #,###; -#,###;0}",_skin.Accounts);
        
            int advertisement = _skin._playerStat._advertisement;
            int maxAdvertisement = _skin.Advertisement;
            Text_Advertisement.GetComponent<TextMeshProUGUI>().text = "(" + advertisement.ToString() + "/" + maxAdvertisement.ToString() + ")";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
