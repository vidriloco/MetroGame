using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaler : MonoBehaviour
{
    void Start()
    {
        var renderer = gameObject.GetComponent<SpriteRenderer>();
        var size = renderer.bounds.size;
        Debug.Log(size.x);

        float width = ScreenSize.GetScreenToWorldWidth;
        //transform.localScale = Vector3.one * width/5;
    }
}
