using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanline : MonoBehaviour
{
    public float speed = 200f;
    RectTransform rect;

    void Start() => rect = GetComponent<RectTransform>();

    void Update()
    {
        rect.anchoredPosition += Vector2.down * speed * Time.deltaTime;
        if (rect.anchoredPosition.y < -Screen.height)
            rect.anchoredPosition = new Vector2(0, Screen.height);
    }
}
