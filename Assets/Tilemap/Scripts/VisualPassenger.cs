using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Area
{
    Platform, Train
}

public struct Passenger
{
    public string identifier;
    public Sprite image;
}

public class VisualPassenger : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    private Passenger associatedPassenger;

    public static VisualPassenger SpawnWith(VisualPassenger prefab, GameObject parentObject, Vector3 position, Passenger passenger, Area area)
    {
        var visualPassenger = (VisualPassenger)GameObject.Instantiate(prefab, position, Quaternion.Euler(0, 0, 0));
        visualPassenger.transform.parent = parentObject.transform;
        visualPassenger.transform.position = position;
        visualPassenger.SetArea(area);
        visualPassenger.SetPassenger(passenger);
        return visualPassenger;
    }

    public void SetArea(Area area)
    {
        switch(area)
        {
            case Area.Train:
                this.tag = Tags.Passenger;
                this.spriteRenderer.sortingOrder = 1;
                break;
            case Area.Platform:
                this.tag = Tags.PassengerInPlatform;
                this.spriteRenderer.sortingOrder = 4;
                break;
        }
    }

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetPassenger(Passenger passenger)
    {
        this.associatedPassenger = passenger;
        spriteRenderer.sprite = passenger.image;
    }
}