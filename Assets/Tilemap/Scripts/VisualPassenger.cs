using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualPassenger : MonoBehaviour
{

    [SerializeField] public GameObject genericSpriteReference;

    private GameObject passengerGameObject;
    private GameObject stationGameObject;

    private bool shouldShowStation;

    private ResourceManager resourceManager;

    public void ConfigureAsPassengerInTrain(bool inTrain)
    {
        passengerGameObject.tag = inTrain ? Tags.Passenger : Tags.PassengerInPlatform;

        if(!inTrain)
        {
            passengerGameObject.GetComponent<SpriteRenderer>().sortingOrder = 4;
        }

        if (shouldShowStation)
        {
            stationGameObject.tag = inTrain ? Tags.Station : Tags.StationInPlatform;

            if (!inTrain)
            {
                stationGameObject.GetComponent<SpriteRenderer>().sortingOrder = 5;
            }
        }
    }

    public void Awake()
    {
        shouldShowStation = UnityEngine.Random.Range(0, 3) == 1;
        resourceManager = GameObject.FindObjectOfType<ResourceManager>();

        passengerGameObject = Instantiate(genericSpriteReference, transform.position, Quaternion.identity);
        passengerGameObject.GetComponent<SpriteRenderer>().sprite = resourceManager.knownImages.randomPassenger();
        passengerGameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;

        if (shouldShowStation)
        {
            stationGameObject = Instantiate(genericSpriteReference, transform.position, Quaternion.identity);

            stationGameObject.GetComponent<SpriteRenderer>().sprite = resourceManager.knownImages.randomStation();
            stationGameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
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
