using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInfoCS : MonoBehaviour
{
    public GameObject Image_Born;    
    public GameObject Text_Win;
    public GameObject Text_Draw;
    public GameObject Text_Lose;
    public GameObject Text_HP;
    public GameObject Text_Power;
    public GameObject Text_Speed;
    public GameObject Text_Name;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetPlayerInfo(PlayerCS playerSkin)
    {
        Text_Name.GetComponent<TextMeshProUGUI>().text = playerSkin.GetCharacterName();        
        Text_Win.GetComponent<TextMeshProUGUI>().text = playerSkin._playerStat._win.ToString();
        Text_Draw.GetComponent<TextMeshProUGUI>().text = playerSkin._playerStat._draw.ToString();        
        Text_Lose.GetComponent<TextMeshProUGUI>().text = playerSkin._playerStat._lose.ToString();
        Text_HP.GetComponent<TextMeshProUGUI>().text = playerSkin._playerStat._hp.ToString();
        Text_Power.GetComponent<TextMeshProUGUI>().text = playerSkin._playerStat._power.ToString();
        Text_Speed.GetComponent<TextMeshProUGUI>().text = playerSkin._playerStat._speed.ToString();
        Image_Born.GetComponent<Image>().sprite = playerSkin.GetImageBorn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
