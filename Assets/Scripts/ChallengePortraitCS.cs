using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengePortraitCS : MonoBehaviour
{
    public GameObject _name;
    public GameObject _portrait;
    public GameObject _portraitLose;
    public GameObject _imageLose;

    public Sprite Sprite_BackGround;
    public Sprite Sprite_BackGround_Selected;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Reset()
    {
        _portrait.SetActive(true);
        _portraitLose.SetActive(false);
        _imageLose.SetActive(false);
        SetSelected(false);
    }

    public void SetLose()
    {
        _portrait.SetActive(false);
        _portraitLose.SetActive(true);
        _imageLose.SetActive(true);
    }

    public void SetSelected(bool selected)
    {
        GetComponent<Button>().enabled = !selected;
        GetComponent<Image>().sprite = selected ? Sprite_BackGround_Selected : Sprite_BackGround;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
