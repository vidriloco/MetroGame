using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] humanReactionClips;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayRandomHumanSound()
    {
        var sound = humanReactionClips[UnityEngine.Random.Range(0, humanReactionClips.Length)];
        gameObject.GetComponent<AudioSource>().PlayOneShot(sound);
    }
}
