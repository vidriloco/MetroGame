using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;


public class Configurations
{

    [System.Serializable]
    public struct Lane {
        public int width;
        public int height;
        public float cellSize;
        public Vector3 position;
        [SerializeField] public Vehicle[] vehicles;
    }

    [System.Serializable]
    public struct Vehicle
    {
        public string id;
        public bool isPrefab;
        public GameObject prefab;

        [SerializeField] public InitialPosition startingPosition;
        [SerializeField] public Movement[] movements;

        public float frequency;

        public enum Direction
        {
            LeftToRight,
            RightToLeft,
            TopToBottom,
            BottomToTop
        }

        [System.Serializable]
        public struct Movement
        {
            public float speed;
            public float duration;
        }

        [System.Serializable]
        public struct InitialPosition
        {
            public Direction direction;
            public int value;
        }

        [SerializeField] public GameObject[] childrenObjects;
    }
}


public class Testing : MonoBehaviour {

    [SerializeField] private TilemapVisual tilemapVisual;

    [SerializeField] private GameObject pivotObject;
    [SerializeField] private GameObject prefab;

    private Tilemap.TilemapObject.TilemapSprite tilemapSprite;

    [System.Serializable]
    public struct MovingObject
    {
        public GameObject prefab;
        public int startingPositionAtY;
        public float speed;
        public float frequency;
    }

    [SerializeField] private Configurations.Lane[] lanesConfigurations;

    private readonly ArrayList lanes = new ArrayList();

    private void Start() {
        Grid<Tilemap.TilemapObject> grid = new Grid<Tilemap.TilemapObject>(5, 10, 5, pivotObject.transform.position, (Grid<Tilemap.TilemapObject> g, int x, int y) => new Tilemap.TilemapObject(g, x, y));

        foreach(var laneConfig in lanesConfigurations)
        {
            var tileMap = new Tilemap(laneConfig.width, laneConfig.height, laneConfig.cellSize, laneConfig.position);
            var floatingTilemap = new FloatingTilemapVisual(grid, pivotObject, prefab);
            lanes.Add(new Lane(tileMap, laneConfig.vehicles, floatingTilemap));
        }

        //tilemap.Load();
    }

    void FixedUpdate()
    {
        foreach(var item in lanes.ToArray())
        {
            var lane = (Lane)item;
            lane.FixedUpdate();
        }
    }

    private void Update() {
        foreach (var item in lanes.ToArray())
        {
            var lane = (Lane) item;
            lane.Update();
        }

        if (Input.GetMouseButton(0)) {
            
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            foreach (var item in lanes.ToArray())
            {
                var lane = (Lane)item;
                lane.tilemap.SetTilemapSprite(mouseWorldPosition, tilemapSprite);
            }
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
            foreach (var item in lanes.ToArray())
            {
                var lane = (Lane)item;
                lane.tilemap.Save();
            }
            CMDebug.TextPopupMouse("Saved!");
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            foreach (var item in lanes.ToArray())
            {
                var lane = (Lane)item;
                lane.tilemap.Load();
            }
            CMDebug.TextPopupMouse("Loaded!");
        }

    }

}
