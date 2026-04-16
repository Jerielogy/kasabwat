using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleJitter : MonoBehaviour
{
    Vector3 originalPos;
    void Start() => originalPos = transform.localPosition;

    void Update()
    {
        if (Random.value > 0.98f)
        { // 2% chance per frame to glitch
            transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * 10f;
        }
        else
        {
            transform.localPosition = originalPos;
        }
    }
}
