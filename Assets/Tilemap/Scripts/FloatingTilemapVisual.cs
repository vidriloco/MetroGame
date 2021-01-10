using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTilemapVisual
{

    private Grid<Tilemap.TilemapObject> grid;
    private GameObject backgroundObject;

    public FloatingTilemapVisual(Grid<Tilemap.TilemapObject> grid, GameObject backgroundObject)
    {
        this.grid = grid;
        this.grid.position = backgroundObject.transform.position;
        this.backgroundObject = backgroundObject;
    }

    public GameObject GetGameObjectFilledWithAnimatableObjectsFromGroup(PassengerAnimator[] animatables, float offsetX)
    {
        var passengerAnimators = GameObject.FindGameObjectsWithTag("PA");

        for(var idx = 0; idx < passengerAnimators.Length; idx++)
        {
            GameObject.DestroyImmediate(passengerAnimators[idx]);
        }

        var bgObject = (GameObject)GameObject.Instantiate(backgroundObject);

        var gameObject = new GameObject();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                var index = Random.Range(0, animatables.Length);
                Vector3 position = grid.GetWorldPosition(x, y);
                var passenger = (PassengerAnimator)GameObject.Instantiate(animatables[index], position, Quaternion.Euler(0, 0, 0));
                passenger.SetParentAndPosition(gameObject.transform, position);
            }
        }

        var renderer = bgObject.GetComponent<SpriteRenderer>();

        var bgObjectWidth = renderer.bounds.size.x / 2;
        var bgObjectHeight = renderer.bounds.size.y / 2;

        gameObject.transform.parent = bgObject.transform;

        // Calculate the width of the built animatables set of sprites
        var bottomLeftPoint = Camera.main.WorldToScreenPoint(gameObject.transform.TransformPoint(0, 0, 0)).x;
        var topRightPoint = Camera.main.WorldToScreenPoint(gameObject.transform.TransformPoint(1, 1, 0)).x;
        var width = topRightPoint - bottomLeftPoint;

        // Center the whole thing within the background object
        gameObject.transform.position = new Vector3(bgObjectWidth - width/2 - offsetX, bgObjectHeight - grid.GetWorldHeight()/2);

        return bgObject;
    }

    public GameObject GetGameObjectFilledWithObjectsFromGroup(GameObject[] prefabs, float offsetX, float offsetY)
    {
        var bgObject = (GameObject)GameObject.Instantiate(backgroundObject);

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
        var bgObject = (GameObject)GameObject.Instantiate(backgroundObject);
        
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

