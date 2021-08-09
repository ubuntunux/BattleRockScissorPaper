using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInfoCS : MonoBehaviour
{
    public GameObject Image_Born;
    public GameObject Text_Age;
    public GameObject Text_HP;
    public GameObject Text_Power;
    public GameObject Text_Name;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetPlayerInfo(PlayerCS playerSkin)
    {
        Text_Name.GetComponent<TextMeshProUGUI>().text = playerSkin.GetCharacterName();
        Text_Age.GetComponent<TextMeshProUGUI>().text = playerSkin.Age.ToString();
        Text_HP.GetComponent<TextMeshProUGUI>().text = playerSkin.HP.ToString();
        Text_Power.GetComponent<TextMeshProUGUI>().text = playerSkin.Power.ToString();
        Image_Born.GetComponent<Image>().sprite = playerSkin.Sprite_Born;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
