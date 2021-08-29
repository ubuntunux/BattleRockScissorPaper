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

    public void ResetMatchCardManager()
    {
        RemoveAllMatchCards();

        AddMatchCardEntry();
        AddMatchCardEntry();
        AddMatchCardEntry();
        AddMatchCardEntry();
    }

    public void AddMatchCardEntry()
    {
        GameObject LayerMatchCardEntry = (GameObject)GameObject.Instantiate(LayerMatchCardPrefab);
        LayerMatchCardEntry.transform.SetParent(LayerMatchCardContents.transform);
        LayerMatchCardEntry.transform.localScale = new Vector3(1, 1, 1);
        float posY = -50.0f - _matchCards.Count * 100.0f;
        LayerMatchCardEntry.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0.0f, posY, 0.0f);

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
