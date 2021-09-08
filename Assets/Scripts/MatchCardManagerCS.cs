using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchCardManagerCS : MonoBehaviour
{
    public GameObject LayerMatchCardContents;    
    public GameObject LayerMatchCardPrefab;

    ChallengeSceneManagerCS _challengeSceneManager = null;
    GameObject[] _playerList;

    List<GameObject> _matchCards = new List<GameObject>();
    MatchCardCS _lastSelectedMatchCard = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ResetMatchCardManager(ChallengeSceneManagerCS challengeSceneManager, GameObject[] playerList, GameObject player)
    {
        _challengeSceneManager = challengeSceneManager;
        _playerList = playerList;
        
        RemoveAllMatchCards();

        int count = _playerList.Length;
        for(int i = 0; i < count; ++i)
        {
            AddMatchCardEntry(count, i, _playerList[i].GetComponent<PlayerCS>(), player);
        }
    }

    public void AddMatchCardEntry(int totalCount, int stageIndex, PlayerCS skin, GameObject player)
    {
        float width = 100.0f;
        float layerWidth = (_matchCards.Count + 1) * width;
        float posX = -width * 0.5f * (totalCount - 1) + _matchCards.Count * width;

        LayerMatchCardContents.GetComponent<RectTransform>().sizeDelta = new Vector2(layerWidth, 80.0f);

        GameObject LayerMatchCardEntry = (GameObject)GameObject.Instantiate(LayerMatchCardPrefab);
        LayerMatchCardEntry.transform.SetParent(LayerMatchCardContents.transform);
        LayerMatchCardEntry.transform.localScale = new Vector3(1, 1, 1);        
        LayerMatchCardEntry.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(posX, 0.0f, 0.0f);
        LayerMatchCardEntry.GetComponent<MatchCardCS>().SetMatchCard(this, stageIndex, skin, player);

        _matchCards.Add(LayerMatchCardEntry);
    }

    public void RemoveAllMatchCards()
    {
        foreach(GameObject child in _matchCards)
        {
            Destroy(child);
        }
        _matchCards.Clear();
    }

    public void SelectMatchCardBySkinID(int skinID, bool playSound)
    {
        int count = _matchCards.Count;
        for(int i = 0; i < count; ++i)
        {
            if(skinID == _matchCards[i].GetComponent<MatchCardCS>().GetSkinID())
            {
                SelectMatchCard(_matchCards[i].GetComponent<MatchCardCS>(), playSound);
                return;
            }
        }
    }

    public void SelectMatchCardByIndex(int index, bool playSound)
    {
        index = Mathf.Max(0, Mathf.Min(_matchCards.Count - 1, index));
        SelectMatchCard(_matchCards[index].GetComponent<MatchCardCS>(), playSound);
    }

    public void SelectMatchCard(MatchCardCS matchCard, bool playSound = true)
    {
        if(matchCard == _lastSelectedMatchCard)
        {
            return;
        }

        if(null != _lastSelectedMatchCard)
        {
            _lastSelectedMatchCard.SetSelected(false);
        }

        _lastSelectedMatchCard = matchCard;
        matchCard.SetSelected(true);

        _challengeSceneManager.SelectChallengePlayer(matchCard.GetPlayer(), matchCard.GetSkin(), matchCard.GetStageIndex(), playSound);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
