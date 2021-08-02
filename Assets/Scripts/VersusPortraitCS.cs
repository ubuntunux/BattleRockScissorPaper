using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersusPortraitCS : MonoBehaviour
{
    public GameObject Text_Name;
    public GameObject Image_Portrait;
    public AudioSource Snd_Name;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayAudioName()
    {
        Snd_Name.Play();
    }

    public void SetVersusPortrait(string name, Sprite sprite, AudioClip audioClipName)
    {
        Text_Name.GetComponent<Text>().text = name;
        Image_Portrait.GetComponent<Image>().sprite = sprite;
        Snd_Name.clip = audioClipName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
