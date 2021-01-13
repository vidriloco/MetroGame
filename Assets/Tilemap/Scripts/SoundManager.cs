using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] humanReactionClips;
    [SerializeField] private AudioClip arrivingMetro;
    [SerializeField] private AudioClip closingDoors;
    [SerializeField] private AudioClip departingMetro;


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

    public void PlayMetroSoundArriving()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(arrivingMetro);
    }

    public void PlayMetroSoundClosingDoors()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(closingDoors);
    }

    public void PlayMetroSoundDeparting()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(departingMetro);
    }
}
