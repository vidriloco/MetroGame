using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PassengerSeat
{
    private readonly int x;
    private readonly int y;
    private readonly Grid<PassengerSeat> grid;
    private VisualPassenger visualPassenger;

    public PassengerSeat(int x, int y, Grid<PassengerSeat> grid) : this()
    {
        this.x = x;
        this.y = y;
        this.grid = grid;

        //this.visualPassenger = visualPassenger;
    }

}

public class FloatingTilemapVisual
{

    private Grid<PassengerSeat> grid;
    private readonly GameObject locomotiveSprite;
    private readonly GameObject carSprite;
    private readonly GameObject carCoverSprite;

    public FloatingTilemapVisual(Grid<PassengerSeat> grid, GameObject locomotiveSprite, GameObject carSprite, GameObject carCoverSprite)
    {
        this.grid = grid;
        this.grid.position = locomotiveSprite.transform.position;
        this.locomotiveSprite = locomotiveSprite;
        this.carSprite = carSprite;
        this.carCoverSprite = carCoverSprite;
    }

    public GameObject GeneratePassengerGrid(VisualPassenger[] visualPassengers)
    {
        var bgObject = GameObject.Instantiate(locomotiveSprite);

        ClearActivePassengers();

        GeneratePassengerLayoutUsing(bgObject, visualPassengers);

        AddSecondCarWith(bgObject);

        EnableCarCoverWith(bgObject);

        return bgObject;
    }

    private void ClearActivePassengers()
    {
        var visualPassengers = GameObject.FindGameObjectsWithTag(Tags.PassengerAnimator);

        for (var idx = 0; idx < visualPassengers.Length; idx++)
        {
            GameObject.DestroyImmediate(visualPassengers[idx]);
        }

    }

    private void GeneratePassengerLayoutUsing(GameObject parentObject, VisualPassenger[] animatables)
    {
        var passengerHolder = new GameObject();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                var index = Random.Range(0, animatables.Length);
                Vector3 position = grid.GetWorldPosition(x, y);
                var visualPassenger = (VisualPassenger) GameObject.Instantiate(animatables[index], position, Quaternion.Euler(0, 0, 0));
                visualPassenger.SetParentAndPosition(passengerHolder.transform, position);
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
        var carObject = GameObject.Instantiate(carSprite);
        carObject.transform.parent = parentObject.transform;
        carObject.transform.position = new Vector3(carObject.transform.position.x, carObject.transform.position.y, 0);
    }

    private void EnableCarCoverWith(GameObject parentObject)
    {
        var bgCoverObject = GameObject.Instantiate(carCoverSprite);

        bgCoverObject.transform.parent = parentObject.transform;
        bgCoverObject.transform.position = parentObject.transform.position;
        bgCoverObject.tag = Tags.MetroCover;
        bgCoverObject.GetComponent<SpriteRenderer>().sortingOrder = 10;

        // Let it be hidden initially
        Color bgCoverObjectSpriteRenderer = bgCoverObject.GetComponent<SpriteRenderer>().color;
        bgCoverObjectSpriteRenderer.a = 0;
        bgCoverObject.GetComponent<SpriteRenderer>().color = bgCoverObjectSpriteRenderer;
    }

}

