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

    public void Awake()
    {
        passengerSpriteIdx = Random.Range(0, passengerSprites.Length);
        stationSpriteIdx = Random.Range(0, stationSprites.Length);
        shouldShowStation = Random.Range(0, 3) == 1;

        passengerGameObjectI = Instantiate(passengerGameObjectRef, Vector3.zero, Quaternion.identity);
        passengerGameObjectI.GetComponent<SpriteRenderer>().sprite = passengerSprites[passengerSpriteIdx];
        passengerGameObjectI.GetComponent<SpriteRenderer>().sortingOrder = 1;

        passengerGameObjectI.tag = "passenger";

        if (shouldShowStation)
        {
            stationGameObjectI = Instantiate(stationGameObjectRef, Vector3.zero, Quaternion.identity);

            stationGameObjectI.GetComponent<SpriteRenderer>().sprite = stationSprites[stationSpriteIdx];
            stationGameObjectI.GetComponent<SpriteRenderer>().sortingOrder = 2;
            stationGameObjectI.tag = "station";
        }
    }

    public void SetParentAndPosition(Transform transform, Vector3 position)
    {
        if(passengerGameObjectI != null)
        {
            passengerGameObjectI.transform.parent = transform;
            passengerGameObjectI.transform.position = position;

            if (stationGameObjectI != null)
            {
                stationGameObjectI.transform.parent = passengerGameObjectI.transform;
                stationGameObjectI.transform.position = position + new Vector3(0, 2, 0);
            }
        }
    }
}
