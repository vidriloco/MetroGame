using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using System;

class ViewPort
{
    public static Vector2 defaultOrigin
    {
        get
        {
            return Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        }
    }

    public static Vector2 WithPadding(float x, float y)
    {
        return defaultOrigin + new Vector2(x, y);
    }

    public static Configurations.Grid GenerateGridParametersForCameraViewport(int dimension)
    {
        var camera = Camera.main;
        var aspect = Camera.main.aspect;

        var height = camera.orthographicSize * 2;
        var width = camera.orthographicSize * aspect * 2;

        return new Configurations.Grid((int) width / dimension, (int) height / dimension, dimension);
    } 
}

public class Testing : MonoBehaviour {

    private Vector2 defaultOrigin
    {
        get { return ViewPort.defaultOrigin; }
    }

    [SerializeField] private TilemapVisual tilemapVisual;
    [SerializeField] private Configurations.Lane[] lanesConfigurations;

    [SerializeField] private GameObject backgroundObject;

    private Tilemap.TilemapObject.TilemapSprite tilemapSprite;

    private readonly ArrayList lanes = new ArrayList();

    private Func<Grid<Tilemap.TilemapObject>, int, int, Tilemap.TilemapObject> gridDelegate
    {
        get { return (Grid<Tilemap.TilemapObject> g, int x, int y) => new Tilemap.TilemapObject(g, x, y); }
    }

    private void Start() {

        // Hora valle
        //Grid<Tilemap.TilemapObject> grid = new Grid<Tilemap.TilemapObject>(3, 6, 9, defaultOrigin, gridDelegate);

        // Hora pico
        //Grid<Tilemap.TilemapObject> grid = new Grid<Tilemap.TilemapObject>(5, 11, 5, defaultOrigin, gridDelegate);

        // Hora super pico
        Grid<Tilemap.TilemapObject> grid = new Grid<Tilemap.TilemapObject>(6, 13, 4, defaultOrigin, gridDelegate);
        
        var vehicleFactory = new VehicleFactory();
        var floatingTilemap = new FloatingTilemapVisual(grid, backgroundObject);
        
        foreach (var laneConfig in lanesConfigurations)
        {
            var gridConfig = ViewPort.GenerateGridParametersForCameraViewport(3);

            Grid<Tilemap.TilemapObject> gameArea = new Grid<Tilemap.TilemapObject>(gridConfig.width, gridConfig.height, gridConfig.cellSize, defaultOrigin, gridDelegate);

            var vehicleManager = new VehicleManager(laneConfig.vehicles)
            {
                VehicleWillChangeHandler = (float speed, float duration) =>
                {

                    if (speed == 0)
                    {
                        GameObject[] stations = GameObject.FindGameObjectsWithTag("station");
                        foreach (var station in stations)
                        {
                            station.tag = "dead-station";
                            LeanTween.delayedCall(station, UnityEngine.Random.Range(0, 5), () =>
                            {
                                LeanTween.alpha(station, 0f, 1f).setLoopPingPong();
                                LeanTween.move(station, new Vector3(station.transform.position.x, station.transform.position.y + 2f, 0), 1).setLoopPingPong();
                            }).setOnCompleteOnRepeat(true);

                        }
                    } else
                    {
                        GameObject[] deadStations = GameObject.FindGameObjectsWithTag("dead-station");
                        foreach (var station in deadStations)
                        {
                            LeanTween.DestroyImmediate(station);
                        }
                    }

                    return false;
                }
            };


            lanes.Add(new Lane(new Tilemap(gameArea), vehicleManager, (Configurations.Vehicle vehicle, Lane lane) =>
            {
                var position = lane.PositionForConfigurationVehicle(vehicle);
                var offsetX = lane.WorldDistanceFromOriginToPositionXAxis(vehicle.startingPosition.value);

                vehicle.prefab = floatingTilemap.GetGameObjectFilledWithAnimatableObjectsFromGroup(vehicle.animatableObjects, offsetX);
                vehicle.childrenObjects = new GameObject[] { };
                var renderer = vehicle.prefab.GetComponent<SpriteRenderer>();
                // Change the position where the metro appears
                vehicle.prefab.transform.position = new Vector3(position.x, -180);
                return vehicleFactory.LoadVehicleFromConfiguration(vehicle);
            }));
        }

        //tilemap.Load();
    }

    void FixedUpdate()
    {
        foreach(var item in lanes)
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
            Vector2 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();

            foreach (var item in lanes)
            {
                var lane = (Lane) item;
                lane.DetectTapOnPosition(mouseWorldPosition);
            }

            
            foreach (var item in lanes)
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
