using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTilemapVisual : MonoBehaviour
{

    private Grid<Tilemap.TilemapObject> grid;
    [SerializeField] public GameObject pivotObject;
    [SerializeField] private GameObject prefab;

    public void SetGrid(Tilemap tilemap, Grid<Tilemap.TilemapObject> grid)
    {
        this.grid = grid;
        UpdateVisualRepresentation();

        grid.OnGridObjectChanged += Grid_OnGridValueChanged;
        tilemap.OnLoaded += Tilemap_OnLoaded;
    }

    private void Tilemap_OnLoaded(object sender, System.EventArgs e) { }

    private void Grid_OnGridValueChanged(object sender, Grid<Tilemap.TilemapObject>.OnGridObjectChangedEventArgs e) { }
  
    private void UpdateVisualRepresentation()
    {

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {

                Tilemap.TilemapObject gridObject = grid.GetGridObject(x, y);
                Tilemap.TilemapObject.TilemapSprite tilemapSprite = gridObject.GetTilemapSprite();
                Vector3 position = new Vector3(x, y) * grid.GetCellSize();

                var movingVehicle = (GameObject)GameObject.Instantiate(prefab, position, Quaternion.Euler(0, 0, 0));
                movingVehicle.transform.SetParent(pivotObject.transform);
            }
        }
    }

}

