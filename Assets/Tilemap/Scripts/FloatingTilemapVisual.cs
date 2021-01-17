using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTilemapVisual
{

    private Grid<Tilemap.TilemapObject> grid;
    private GameObject locomotiveSprite;
    private GameObject carSprite;
    private GameObject carCoverSprite;

    public FloatingTilemapVisual(Grid<Tilemap.TilemapObject> grid, GameObject locomotiveSprite, GameObject carSprite, GameObject carCoverSprite)
    {
        this.grid = grid;
        this.grid.position = locomotiveSprite.transform.position;
        this.locomotiveSprite = locomotiveSprite;
        this.carSprite = carSprite;
        this.carCoverSprite = carCoverSprite;
    }

    public GameObject GetGameObjectFilledWithAnimatableObjectsFromGroup(PassengerAnimator[] animatables, float offsetX)
    {
        var passengerAnimators = GameObject.FindGameObjectsWithTag("PA");

        for(var idx = 0; idx < passengerAnimators.Length; idx++)
        {
            GameObject.DestroyImmediate(passengerAnimators[idx]);
        }

        var bgObject = (GameObject)GameObject.Instantiate(locomotiveSprite);
        var bgCoverObject = (GameObject)GameObject.Instantiate(carCoverSprite);

        var passengerPositionsObject = new GameObject();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                var index = Random.Range(0, animatables.Length);
                Vector3 position = grid.GetWorldPosition(x, y);
                var passenger = (PassengerAnimator)GameObject.Instantiate(animatables[index], position, Quaternion.Euler(0, 0, 0));
                passenger.SetParentAndPosition(passengerPositionsObject.transform, position);
            }
        }

        var renderer = bgObject.GetComponent<SpriteRenderer>();

        var passengerPositionX = renderer.bounds.size.x/2 - grid.GetWorldWidth()/2;
        var bgObjectHeight = renderer.bounds.size.y / 2;

        passengerPositionsObject.transform.parent = bgObject.transform;

        // Center the whole thing within the background object
        passengerPositionsObject.transform.position = new Vector3(passengerPositionX*bgObject.transform.localScale.x, bgObjectHeight - grid.GetWorldHeight()/2);

        // Add an additional car below the main car
        var carObject = (GameObject)GameObject.Instantiate(carSprite);
        carObject.transform.parent = bgObject.transform;
        carObject.transform.position = new Vector3(carObject.transform.position.x, carObject.transform.position.y, 0);

        // Add a cover to hide the passengers while the train moves
        bgCoverObject.transform.parent = bgObject.transform;
        bgCoverObject.transform.position = bgObject.transform.position;
        bgCoverObject.tag = "metro-cover";
        bgCoverObject.GetComponent<SpriteRenderer>().sortingOrder = 10;

        return bgObject;
    }

    public GameObject GetGameObjectFilledWithObjectsFromGroup(GameObject[] prefabs, float offsetX, float offsetY)
    {
        var bgObject = (GameObject)GameObject.Instantiate(locomotiveSprite);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                var index = Random.Range(0, prefabs.Length);

                Tilemap.TilemapObject gridObject = grid.GetGridObject(x, y);
                Tilemap.TilemapObject.TilemapSprite tilemapSprite = gridObject.GetTilemapSprite();
                Vector3 position = grid.GetWorldPosition(x, y) + new Vector3(offsetX, offsetY);
                var movingVehicle = (GameObject)GameObject.Instantiate(prefabs[index], position, Quaternion.Euler(0, 0, 0));
                movingVehicle.transform.SetParent(bgObject.transform);
            }
        }

        return bgObject;
    }

    public GameObject GetGameObjectFilledWithObjects(GameObject prefab)
    {
        var bgObject = (GameObject)GameObject.Instantiate(locomotiveSprite);
        
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {

                Tilemap.TilemapObject gridObject = grid.GetGridObject(x, y);
                Tilemap.TilemapObject.TilemapSprite tilemapSprite = gridObject.GetTilemapSprite();
                Vector3 position = new Vector3(x, y) * grid.GetCellSize();
                var movingVehicle = (GameObject)GameObject.Instantiate(prefab, position, Quaternion.Euler(0, 0, 0));

                movingVehicle.transform.SetParent(bgObject.transform);
            }
        }

        return bgObject;
    }

}

