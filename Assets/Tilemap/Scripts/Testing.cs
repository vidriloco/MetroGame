using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;

public class Testing : MonoBehaviour {

    //[SerializeField] private TilemapVisual tilemapVisual;

    private Tilemap tilemapLeft;
    private Tilemap tilemapRight;
    private Tilemap.TilemapObject.TilemapSprite tilemapSprite;

    private Lane laneLeft;
    private Lane laneRight;

    [System.Serializable]
    public struct MovingObject
    {
        public GameObject prefab;
        public int startingPositionAtY;
        public float speed;
        public float frequency;
    }

    [SerializeField] private MovingObject[] movingObjects;

    private void Start() {
        tilemapLeft = new Tilemap(10, 5, 5f, new Vector3(0, 10, 0));
        tilemapRight = new Tilemap(10, 5, 5f, new Vector3(0, 50, 0));

        //tilemap.SetTilemapVisual(tilemapVisual);

        //tilemap.Load();

        laneLeft = new Lane(tilemapLeft, movingObjects, Lane.Direction.Left);
        laneRight = new Lane(tilemapRight, movingObjects, Lane.Direction.Right);
    }

    void FixedUpdate()
    {
        laneLeft.FixedUpdate();
        laneRight.FixedUpdate();
    }

    private void Update() {
        laneLeft.Update();
        laneRight.Update();

        if (Input.GetMouseButton(0)) {
            
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            tilemapLeft.SetTilemapSprite(mouseWorldPosition, tilemapSprite);
        }
        
        if (Input.GetKeyDown(KeyCode.T)) {
            tilemapSprite = Tilemap.TilemapObject.TilemapSprite.None;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            tilemapSprite = Tilemap.TilemapObject.TilemapSprite.VehicleLaneManHoleA;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            tilemapSprite = Tilemap.TilemapObject.TilemapSprite.VehicleLaneA;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            tilemapSprite = Tilemap.TilemapObject.TilemapSprite.VehicleLaneManHoleB;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            tilemapSprite = Tilemap.TilemapObject.TilemapSprite.VehicleLaneB;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            tilemapSprite = Tilemap.TilemapObject.TilemapSprite.Sidewalk;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            tilemapSprite = Tilemap.TilemapObject.TilemapSprite.Path;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
        }


        if (Input.GetKeyDown(KeyCode.G))
        {
            tilemapSprite = Tilemap.TilemapObject.TilemapSprite.Pasture;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
        }


        if (Input.GetKeyDown(KeyCode.P)) {
            tilemapLeft.Save();
            CMDebug.TextPopupMouse("Saved!");
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            tilemapLeft.Load();
            CMDebug.TextPopupMouse("Loaded!");
        }

    }

}
