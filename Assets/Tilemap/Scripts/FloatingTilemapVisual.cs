using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTilemapVisual
{

    private Grid<Tilemap.TilemapObject> grid;
    private GameObject pivotObject;
    private GameObject backgroundObject;

    public FloatingTilemapVisual(Grid<Tilemap.TilemapObject> grid, GameObject pivotObject, GameObject backgroundObject)
    {
        this.grid = grid;
        this.pivotObject = pivotObject;
        this.backgroundObject = backgroundObject;
    }

    public GameObject GetGameObjectFilledWithObjectsFromGroup(GameObject[] prefabs)
    {
        var pivot = (GameObject)GameObject.Instantiate(pivotObject);

        var bgObject = (GameObject)GameObject.Instantiate(backgroundObject);
        bgObject.transform.SetParent(pivot.transform);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                var index = Random.Range(0, prefabs.Length);

                Tilemap.TilemapObject gridObject = grid.GetGridObject(x, y);
                Tilemap.TilemapObject.TilemapSprite tilemapSprite = gridObject.GetTilemapSprite();
                Vector3 position = new Vector3(x, y) * grid.GetCellSize();
                var movingVehicle = (GameObject)GameObject.Instantiate(prefabs[index], position, Quaternion.Euler(0, 0, 0));
                movingVehicle.transform.SetParent(pivot.transform);
            }
        }

        return pivot;
    }

    public GameObject GetGameObjectFilledWithObjects(GameObject prefab)
    {
        var pivot = (GameObject)GameObject.Instantiate(pivotObject);
        
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {

                Tilemap.TilemapObject gridObject = grid.GetGridObject(x, y);
                Tilemap.TilemapObject.TilemapSprite tilemapSprite = gridObject.GetTilemapSprite();
                Vector3 position = new Vector3(x, y) * grid.GetCellSize();
                var movingVehicle = (GameObject)GameObject.Instantiate(prefab, position, Quaternion.Euler(0, 0, 0));

                movingVehicle.transform.SetParent(pivot.transform);
            }
        }

        return pivot;
    }

}

