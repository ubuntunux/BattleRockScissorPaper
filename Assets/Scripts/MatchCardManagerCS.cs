using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchCardManagerCS : MonoBehaviour
{
    public GameObject LayerMatchCardContents;    
    public GameObject LayerMatchCardPrefab;

    List<GameObject> _matchCards = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ResetMatchCardManager(ChallengeSceneManagerCS challengeSceneManager, GameObject[] challengePlayers)
    {
        RemoveAllMatchCards();

        for(int i = 0; i < challengePlayers.Length; ++i)
        {
            AddMatchCardEntry(challengeSceneManager, i, challengePlayers[i].GetComponent<PlayerCS>());
        }
    }

    public void AddMatchCardEntry(ChallengeSceneManagerCS challengeSceneManager, int stageIndex, PlayerCS player)
    {
        float posY = -(50.0f + _matchCards.Count * 100.0f);

        LayerMatchCardContents.GetComponent<RectTransform>().offsetMin = new Vector2(0.0f, 350.0f + posY);

        GameObject LayerMatchCardEntry = (GameObject)GameObject.Instantiate(LayerMatchCardPrefab);
        LayerMatchCardEntry.transform.SetParent(LayerMatchCardContents.transform);
        LayerMatchCardEntry.transform.localScale = new Vector3(1, 1, 1);        
        LayerMatchCardEntry.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0.0f, posY, 0.0f);
        LayerMatchCardEntry.GetComponent<MatchCardCS>().SetMatchCard(challengeSceneManager, stageIndex, player);

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
