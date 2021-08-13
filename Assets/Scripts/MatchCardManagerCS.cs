using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchCardManagerCS : MonoBehaviour
{
    public GameObject LayerMatchCardContents;    
    public GameObject LayerMatchCardPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Reset()
    {
        //RemoveAllMatchCards();

        AddMatchCardEntry();
        AddMatchCardEntry();
        AddMatchCardEntry();
        AddMatchCardEntry();
    }

    public void AddMatchCardEntry()
    {
        GameObject LayerMatchCardEntry = (GameObject)GameObject.Instantiate(LayerMatchCardPrefab);
        LayerMatchCardEntry.transform.SetParent(LayerMatchCardContents.transform);
        float posY = 50.0f - LayerMatchCardContents.transform.childCount * 100.0f;
        LayerMatchCardEntry.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0.0f, posY, 0.0f);
    }

    public void RemoveAllMatchCards()
    {
        foreach(Transform child in LayerMatchCardContents.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
