using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimationCS : MonoBehaviour
{
    public float _animationSpeed = 1.0f;
    public float _playTime = 0.0f;
    public Sprite[] _sprites;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int index = (int)_playTime;
        GetComponent<Renderer>().material.SetTexture("_MainTex", _sprites[index].texture);
        _playTime = (_playTime + Time.deltaTime * _animationSpeed) % (float)_sprites.Length;
    }
}
