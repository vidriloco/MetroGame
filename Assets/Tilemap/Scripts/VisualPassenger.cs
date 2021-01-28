using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualPassenger : MonoBehaviour
{

    [SerializeField] public GameObject genericSpriteReference;

    private GameObject passengerGameObject;
    private GameObject stationGameObject;

    private bool shouldShowStation = false;

    public void Awake()
    {
        var resourceManager = GameObject.FindObjectOfType<ResourceManager>();

        shouldShowStation = UnityEngine.Random.Range(0, 3) == 1;

        passengerGameObject = Instantiate(genericSpriteReference, Vector3.zero, Quaternion.identity);
        passengerGameObject.GetComponent<SpriteRenderer>().sprite = resourceManager.knownImages.randomPassenger();
        passengerGameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;

        passengerGameObject.tag = Tags.Passenger;

        if (shouldShowStation)
        {
            stationGameObject = Instantiate(genericSpriteReference, Vector3.zero, Quaternion.identity);

            stationGameObject.GetComponent<SpriteRenderer>().sprite = resourceManager.knownImages.randomStation();
            stationGameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
            stationGameObject.tag = Tags.Station;
        }
    }

    public void SetParentAndPosition(Transform transform, Vector3 position)
    {
        if(passengerGameObject != null)
        {
            passengerGameObject.transform.parent = transform;
            passengerGameObject.transform.position = position;

            if (stationGameObject != null)
            {
                stationGameObject.transform.parent = passengerGameObject.transform;
                stationGameObject.transform.position = position + new Vector3(0, 2, 0);
            }
        }
    }
}
