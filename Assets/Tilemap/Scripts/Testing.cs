using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using System;

public struct Tags
{
    public static string PassengerInPlatform = "passenger-in-platform";
    public static string Passenger = "platform";
    public static string PassengerBubbleUsed = "dead-station";
    public static string PassengerTrapped = "passenger-trapped";
}

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

    [SerializeField] private GameObject locomotiveSprite;
    [SerializeField] private GameObject carSprite;
    [SerializeField] private GameObject carCover;

    private Tilemap.TilemapObject.TilemapSprite tilemapSprite;

    private readonly ArrayList lanes = new ArrayList();

    private Func<Grid<Tilemap.TilemapObject>, int, int, Tilemap.TilemapObject> gridDelegate
    {
        get { return (Grid<Tilemap.TilemapObject> g, int x, int y) => new Tilemap.TilemapObject(g, x, y); }
    }

    private void MetroWillCloseDoors()
    {
        GameObject.FindObjectOfType<SoundManager>().PlayMetroSoundClosingDoors();
        GameObject[] deadStations = GameObject.FindGameObjectsWithTag("dead-station");
        foreach (var station in deadStations)
        {
            var passengerInfo = station.transform.parent.gameObject;
            if(passengerInfo != null)
            {
                passengerInfo.GetComponent<SpriteRenderer>().color = Color.red;
                passengerInfo.tag = Tags.PassengerTrapped;
                LeanTween.DestroyImmediate(station);
            }
            
        }

        GameObject cover = GameObject.FindGameObjectWithTag("metro-cover");
        LeanTween.delayedCall(cover, 1f, () =>
        {
            LeanTween.alpha(cover, 1f, 2f).setOnComplete(() => {
                cover.tag = "discard-object";
            });
        }).setOnCompleteOnRepeat(false);
    }

    private void MetroWillLetPassengersGoInAndOut() {
        GameObject[] stations = GameObject.FindGameObjectsWithTag("station");
        GameObject cover = GameObject.FindGameObjectWithTag("metro-cover");

        if (cover != null)
        {
            LeanTween.alpha(cover, 0f, 1f).setEaseLinear();
        }

        foreach (var station in stations)
        {
            station.tag = "dead-station";
            LeanTween.delayedCall(station, UnityEngine.Random.Range(0, 5), () =>
            {
                LeanTween.alpha(station, 0f, 1f).setLoopPingPong();
                LeanTween.move(station, new Vector3(station.transform.position.x, station.transform.position.y + 2f, 0), 1).setLoopPingPong();
            }).setOnCompleteOnRepeat(true);

        }
    }

    private void MetroWillDepart()
    {
        GameObject.FindObjectOfType<SoundManager>().PlayMetroSoundDeparting();
    }

    private FloatingTilemapVisual BuildRandomPassengersLayout()
    {
        Grid<Tilemap.TilemapObject> grid;

        int rangeIndex = UnityEngine.Random.Range(0, 2);

        switch (rangeIndex)
        {
            case 0:
                grid = new Grid<Tilemap.TilemapObject>(4, UnityEngine.Random.Range(4, 10), 7, defaultOrigin, gridDelegate);
                break;
            default:
                grid = new Grid<Tilemap.TilemapObject>(3, UnityEngine.Random.Range(4, 10), 9, defaultOrigin, gridDelegate);
                break;
        }

        return new FloatingTilemapVisual(grid, locomotiveSprite, carSprite, carCover);
    }

    private void Start() {

        var vehicleFactory = new VehicleFactory();

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

                        if (duration == 6)
                        {
                            MetroWillCloseDoors();
                        } else {
                            MetroWillLetPassengersGoInAndOut();
                        }
                    } else
                    {
                        if (speed == 1 && duration == 1)
                        {
                            MetroWillDepart();
                        }
                    }

                    return false;
                }
            };


            lanes.Add(new Lane(new Tilemap(gameArea), vehicleManager, (Configurations.Vehicle vehicle, Lane lane) =>
            {
                GameObject.FindObjectOfType<SoundManager>().PlayMetroSoundArriving();

                var position = lane.PositionForConfigurationVehicle(vehicle);
                var offsetX = lane.WorldDistanceFromOriginToPositionXAxis(vehicle.startingPosition.value);

                var floatingTilemap = BuildRandomPassengersLayout();

                vehicle.prefab = floatingTilemap.GetGameObjectFilledWithAnimatableObjectsFromGroup(vehicle.animatableObjects, 0);
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

        if (Input.GetMouseButtonUp(0)) {
            Vector2 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();

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
