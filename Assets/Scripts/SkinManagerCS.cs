using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SkinManagerCS : MonoBehaviour
{
    public GameObject MainSceneManager;
    public GameObject PlayerA;
    public GameObject PlayerB;

    public GameObject SkinCardPrefab;
    public GameObject LayerSkinCard;
    public GameObject LayerSkinCardContents;

    GameObject _targetPlayer = null;

    // Start is called before the first frame update
    void Start()
    {
        AddSkinCards();
    }

    void OnEnable()
    {
        ResetSkinScene();
    }

    void OnDisable()
    {
    }

    public void Exit()
    {
        MainSceneManager.GetComponent<MainSceneManagerCS>().SetActivateScenePrev();
    }

    public void ResetSkinScene()
    {
        bool showPlayerA = PlayerA == _targetPlayer;

        LayerSkinCard.transform.localPosition = new Vector3(showPlayerA ? 165.0f : -165.0f, 150.0f, 0.0f);

        PlayerA.SetActive(showPlayerA);
        PlayerA.GetComponent<PlayerCS>().SetStateIdle();
        
        PlayerB.SetActive(false == showPlayerA);
        PlayerB.GetComponent<PlayerCS>().SetStateIdle();
    }

    public void SetTargetPlayer(GameObject targetPlayer)
    {
        _targetPlayer = targetPlayer;
    }

    public void AddSkinCards()
    {
        int skinIndex = 0;
        int skinCount = MainSceneManager.GetComponent<MainSceneManagerCS>().GetSkinCount();
        int heightCount = 4;
        int widthCount = 4;
        float cardSizeX = 160.0f;
        float cardSizeY = 190.0f;
        float offsetX = -240;
        float offsetY = 320;
        for(int y = 0; y < heightCount; ++y)
        {
            for(int x = 0; x < widthCount; ++x)
            {
                PlayerCS skin = MainSceneManager.GetComponent<MainSceneManagerCS>().GetSkinByIndex(skinIndex++);
                GameObject SkinCardEntry = (GameObject)GameObject.Instantiate(SkinCardPrefab);
                SkinCardEntry.transform.SetParent(LayerSkinCardContents.transform);
                SkinCardEntry.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(offsetX + x * cardSizeX, offsetY + y * -cardSizeY, 0.0f);
                SkinCardEntry.transform.localScale = new Vector3(1, 1, 1);
                SkinCardEntry.GetComponent<SkinCardCS>().SetSkinCard(GetComponent<SkinManagerCS>(), skin);

                if(skinCount <= skinIndex)
                {
                    break;
                }
            }
            
            if(skinCount <= skinIndex)
            {
                break;
            }
        }
    }

    public void SetPlayerSkin(PlayerCS skin)
    {
        _targetPlayer.GetComponent<PlayerCS>().SetSkin(skin);
        _targetPlayer.GetComponent<PlayerCS>().PlayCharacterName();
        SystemValue.SetInt((PlayerA == _targetPlayer) ? SystemValue.PlayerSkinIDKey : SystemValue.PlayerBSkinIDKey, skin._playerStat._skinID);
    }

    public void PurchaseSkinByAccounts(SkinCardCS skinCard)
    {
        PlayerCS skin = skinCard.GetSkin();
        int score = MainSceneManager.GetComponent<MainSceneManagerCS>().GetScore() - skin.Accounts;

        if(0 <= score)
        {
            MainSceneManager.GetComponent<MainSceneManagerCS>().SetScore(score);
            skinCard.PurchaseSkinCard();
        }
    }

    public void PurchaseSkinByAdvertisement(SkinCardCS skinCard)
    {
        PlayerCS skin = skinCard.GetSkin();
        if(skin._playerStat._advertisement < skin.Advertisement)
        {
            // will callback - CallbackPurchaseSkinByAdvertisement
            MainSceneManager.GetComponent<MainSceneManagerCS>().ShowRewardedAd(skinCard);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(MainSceneManager.GetComponent<MainSceneManagerCS>().GetActivateSceneType() == GameSceneType.SkinScene)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Exit();
            }
        }
    }
}
