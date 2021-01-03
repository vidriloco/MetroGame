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

