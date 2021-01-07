using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerAnimator : MonoBehaviour
{

    [SerializeField] public GameObject passengerGameObjectRef;
    [SerializeField] public GameObject stationGameObjectRef;

    private GameObject passengerGameObjectI;
    private GameObject stationGameObjectI;

    private int passengerSpriteIdx = 0;
    private int stationSpriteIdx = 0;

    [SerializeField] private Sprite[] passengerSprites;
    [SerializeField] private Sprite[] stationSprites;

    private bool shouldShowStation = false;

    private float framerate;

    private float measuredTime;
    private float nextTime
    {
        get { return framerate + measuredTime; }
    }

    private Sprite currentStationSprite = null;

    public void Awake()
    {
        passengerSpriteIdx = Random.Range(0, passengerSprites.Length);
        stationSpriteIdx = Random.Range(0, stationSprites.Length);
        framerate = Random.Range(5,10);
        measuredTime = Time.time;
        shouldShowStation = Random.Range(0, 3) == 1;
    }

    public void SetParentAndPosition(Transform transform, Vector3 position)
    {
        passengerGameObjectI = Instantiate(passengerGameObjectRef, position, Quaternion.identity);
        passengerGameObjectI.transform.parent = transform;

        if(shouldShowStation)
        {
            var stationPosition = position + new Vector3(0, 3);
            stationGameObjectI = Instantiate(stationGameObjectRef, stationPosition, Quaternion.identity);
            stationGameObjectI.transform.parent = transform;
            stationGameObjectI.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }

    }

    private void Update()
    {

        if (Time.time <= nextTime)
        {
            if(currentStationSprite == null)
            {
                currentStationSprite = stationSprites[stationSpriteIdx];
            } else
            {
                currentStationSprite = null;
            }
        } else
        {
            measuredTime = Time.time;
        }

        //Debug.Log("On: " + Time.time + " Next time: " + nextTime);


        if (stationGameObjectI != null)
        {
            stationGameObjectI.GetComponent<SpriteRenderer>().sprite = currentStationSprite;
        }

        if (passengerGameObjectI != null)
        {
            passengerGameObjectI.GetComponent<SpriteRenderer>().sprite = passengerSprites[passengerSpriteIdx];
        }
    }
}
