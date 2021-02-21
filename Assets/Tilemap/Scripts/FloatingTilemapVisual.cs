using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct PassengerSeat
{
    private readonly int x;
    private readonly int y;
    private readonly Grid<PassengerSeat> grid;

    public PassengerSeat(int x, int y, Grid<PassengerSeat> grid) : this()
    {
        this.x = x;
        this.y = y;
        this.grid = grid;
    }

}

public class FloatingTilemapVisual
{

    private Grid<PassengerSeat> grid;
    private readonly GameObject frontCarSprite;
    private readonly GameObject closedCarSprite;
    private readonly GameObject carCoverSprite;
    private readonly GameObject boundObject;
    private readonly GameObject stationNameObject;

    private readonly ResourceManager resourceManager = GameObject.FindObjectOfType<ResourceManager>();

    public FloatingTilemapVisual(Grid<PassengerSeat> grid)
    {
        this.grid = grid;

        this.frontCarSprite = resourceManager.knownImages.frontCar;
        this.closedCarSprite = resourceManager.knownImages.closedCar;
        this.carCoverSprite = resourceManager.knownImages.carCover;
        this.boundObject = resourceManager.knownImages.boundText;
        this.stationNameObject = resourceManager.knownImages.stationNameText;

        this.grid.position = frontCarSprite.transform.position;
    }

    public GameObject GeneratePassengerGrid()
    {
        var bgObject = GameObject.Instantiate(frontCarSprite);

        ClearActivePassengers();

        GeneratePassengerLayoutUsing(bgObject);

        AddSecondCarWith(bgObject);

        EnableCarCoverWith(bgObject);

        return bgObject;
    }

    private void ClearActivePassengers()
    {
        var visualPassengers = GameObject.FindGameObjectsWithTag(Tags.VisualPassenger);

        for (var idx = 0; idx < visualPassengers.Length; idx++)
        {
            GameObject.DestroyImmediate(visualPassengers[idx]);
        }

    }

    private void GeneratePassengerLayoutUsing(GameObject parentObject)
    {
        var passengerHolder = new GameObject();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                Vector3 position = grid.GetWorldPosition(x, y);
                var passengerData = resourceManager.knownImages.GetRandomPassenger();
                var stationData = resourceManager.knownImages.GetRandomStation();

                PassengerGenerator.Instance.GenerateWith(position, passengerData, passengerHolder, Area.Train, stationData);
            }
        }

        var renderer = parentObject.GetComponent<SpriteRenderer>();

        var passengerPositionX = renderer.bounds.size.x / 2 - grid.GetWorldWidth() / 2;
        var bgObjectHeight = renderer.bounds.size.y / 2;

        passengerHolder.transform.parent = parentObject.transform;

        // Center the whole thing within the background object
        passengerHolder.transform.position = new Vector3(passengerPositionX * parentObject.transform.localScale.x, bgObjectHeight - grid.GetWorldHeight() / 2);
    }

    private void AddSecondCarWith(GameObject parentObject)
    {
        var carObject = GameObject.Instantiate(closedCarSprite);
        carObject.transform.parent = parentObject.transform;
        carObject.transform.position = new Vector3(carObject.transform.position.x, carObject.transform.position.y, 0);
    }

    private void EnableCarCoverWith(GameObject parentObject)
    {
        var bgCoverObject = GameObject.Instantiate(carCoverSprite);

        bgCoverObject.transform.parent = parentObject.transform;
        bgCoverObject.transform.position = parentObject.transform.position;
        bgCoverObject.tag = Tags.MetroCover;
        bgCoverObject.GetComponent<SpriteRenderer>().sortingOrder = 3;

        // Let it be hidden initially
        Color bgCoverObjectSpriteRenderer = bgCoverObject.GetComponent<SpriteRenderer>().color;
        //bgCoverObjectSpriteRenderer.a = 0;
        bgCoverObject.GetComponent<SpriteRenderer>().color = bgCoverObjectSpriteRenderer;
    }

    private void GenerateTextSignalWithText(string boundToText,  GameObject trainObject)
    {
        var station = resourceManager.knownImages.GetStationAt(0);

        var boundObjectClone = GameObject.Instantiate(boundObject);

        var stationNameObjectClone = GameObject.Instantiate(stationNameObject);

        var boundObjectrenderer = boundObjectClone.GetComponent<MeshRenderer>();
        boundObjectrenderer.sortingOrder = 4;

        var stationNameObjectRenderer = stationNameObjectClone.GetComponent<MeshRenderer>();
        stationNameObjectRenderer.sortingOrder = 4;

        var containerCenter = trainObject.transform.GetComponent<SpriteRenderer>().bounds.center;
        var verticalCenter = containerCenter.y + boundObjectrenderer.bounds.size.y;

        var signalObject = new GameObject();
        signalObject.tag = Tags.MetroBoundSignal;

        var boundToLabel = boundObjectClone.GetComponent<TMPro.TextMeshPro>();
        boundToLabel.text = boundToText;
        boundToLabel.transform.SetParent(signalObject.transform);

        var destinationToLabel = stationNameObjectClone.GetComponent<TMPro.TextMeshPro>();
        destinationToLabel.text = station.name;
        destinationToLabel.transform.SetParent(signalObject.transform);

        var image = GameObject.Instantiate(GameObject.FindGameObjectWithTag("debug-object-2"));
        
        image.GetComponent<SpriteRenderer>().sprite = station.bigIcon;

        image.transform.SetParent(signalObject.transform);
        image.GetComponent<SpriteRenderer>().sortingOrder = 4;

        image.transform.position = new Vector3(containerCenter.x / 2 + image.GetComponent<SpriteRenderer>().bounds.size.x / 2, verticalCenter + 13);
        boundToLabel.transform.position = new Vector3(image.transform.position.x, verticalCenter);
        destinationToLabel.transform.position = new Vector3(image.transform.position.x, verticalCenter - 5);

        signalObject.transform.SetParent(trainObject.transform);
    }

}

