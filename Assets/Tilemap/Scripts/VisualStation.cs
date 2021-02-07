using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Station
{
    public string identifier;
    public string name;

    public Sprite floatingIcon;
    public Sprite bigIcon;
}

public class VisualStation : MonoBehaviour
{

    private Station associatedStation;
    private SpriteRenderer spriteRenderer;

    private ResourceManager resourceManager;

    public static VisualStation SpawnWith(VisualStation prefab, GameObject parentObject, Vector3 position, Station station, Area area, Vector3 offset)
    {
        var visualStation = (VisualStation)GameObject.Instantiate(prefab, position, Quaternion.Euler(0, 0, 0));
        visualStation.transform.parent = parentObject.transform;
        visualStation.transform.position = position + offset;
        visualStation.SetArea(area);
        visualStation.SetStation(station);
        return visualStation;
    }

    public void SetArea(Area area)
    {
        switch (area)
        {
            case Area.Platform:
                this.tag = Tags.PassengerInPlatform;
                this.spriteRenderer.sortingOrder = 5;
                break;
            case Area.Train:
                this.tag = Tags.Passenger;
                this.spriteRenderer.sortingOrder = 2;
                break;
        }
    }


    public void SetStation(Station station)
    {
        this.associatedStation = station;
        spriteRenderer.sprite = station.floatingIcon;

    }

    public void Awake()
    {
        resourceManager = GameObject.FindObjectOfType<ResourceManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public Station GetStation()
    {
        return associatedStation;
    }
}