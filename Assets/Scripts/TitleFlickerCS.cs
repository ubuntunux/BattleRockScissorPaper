using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleFlickerCS : MonoBehaviour
{
    float _flickerTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _flickerTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        float opacity = Mathf.Abs(Mathf.Cos(_flickerTime * 3.141592f));
        GetComponent<TextMeshProUGUI>().color = new Color(1.0f, 1.0f, 1.0f, opacity);

        _flickerTime = (_flickerTime + Time.deltaTime * 0.5f) % 1.0f;
    }
}
