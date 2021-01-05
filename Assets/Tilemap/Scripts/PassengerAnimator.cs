using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] frameArray;
    private float timer;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        var framerate = Random.Range(.9f, 2.5f);

        if (timer >= framerate)
        {
            timer -= framerate;
            var currentFrame = Random.Range(0, frameArray.Length);
            spriteRenderer.sprite = frameArray[currentFrame];
        }
    }
}
