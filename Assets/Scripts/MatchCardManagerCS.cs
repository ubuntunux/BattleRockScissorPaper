using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchCardManagerCS : MonoBehaviour
{
    public GameObject LayerMatchCardContents;    
    public GameObject LayerMatchCardPrefab;

    ChallengeSceneManagerCS _challengeSceneManager = null;

    List<GameObject> _matchCards = new List<GameObject>();
    MatchCardCS _lastSelectedMatchCard = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ResetMatchCardManager(ChallengeSceneManagerCS challengeSceneManager, GameObject[] challengePlayers, GameObject player)
    {
        _challengeSceneManager = challengeSceneManager;
        
        RemoveAllMatchCards();

        int count = challengePlayers.Length;
        for(int i = 0; i < count; ++i)
        {
            AddMatchCardEntry(count, i % 3, challengePlayers[i % 3].GetComponent<PlayerCS>(), player);
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

    public void MatchCardOnClick(MatchCardCS matchCard)
    {
        if(matchCard == _lastSelectedMatchCard)
        {
            return;
        }

        _lastSelectedMatchCard.SetDisabledColor();
        _lastSelectedMatchCard = matchCard;
        matchCard.SetSelectedColor();
        
        _challengeSceneManager.SelectChallengePlayer(matchCard.getStageIndex(), matchCard.getPlayer());
        _challengeSceneManager.LayerMatchCardClick();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
