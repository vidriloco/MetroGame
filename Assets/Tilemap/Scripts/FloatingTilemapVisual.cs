using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTilemapVisual
{

    private Grid<Tilemap.TilemapObject> grid;
    private GameObject pivotObject;

    public FloatingTilemapVisual(Grid<Tilemap.TilemapObject> grid, GameObject pivotObject)
    {
        this.grid = grid;
        this.pivotObject = pivotObject;
    }

    public GameObject GetPivotWithVisualRepresentation(GameObject prefab)
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

