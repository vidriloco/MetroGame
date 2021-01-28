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

    private Tilemap.TilemapObject.TilemapSprite tilemapSprite;

    private readonly ArrayList lanes = new ArrayList();

    private Func<Grid<PassengerSeat>, int, int, PassengerSeat> gridDelegate
    {
        get { return (Grid<PassengerSeat> g, int x, int y) => new PassengerSeat(x, y, g); }
    }

    private void MetroWillCloseDoors()
    {
        GameObject.FindObjectOfType<SoundManager>().PlayMetroSoundClosingDoors();
        GameObject[] deadStations = GameObject.FindGameObjectsWithTag(Tags.StationSymbolMarkedToDrop);
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

        GameObject cover = GameObject.FindGameObjectWithTag(Tags.MetroCover);
        LeanTween.delayedCall(cover, 1f, () =>
        {
            LeanTween.alpha(cover, 1f, 2f).setOnComplete(() => {
                cover.tag = Tags.DiscardObject;
            });
        }).setOnCompleteOnRepeat(false);
    }

    private void MetroWillLetPassengersGoInAndOut() {
        GameObject[] stations = GameObject.FindGameObjectsWithTag(Tags.Station);
        GameObject cover = GameObject.FindGameObjectWithTag(Tags.MetroCover);

        if (cover != null)
        {
            LeanTween.alpha(cover, 0f, 1f).setEaseLinear();
        }

        foreach (var station in stations)
        {
            station.tag = Tags.StationSymbolMarkedToDrop;
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
        Grid<PassengerSeat> grid;

        int rangeIndex = UnityEngine.Random.Range(0, 2);

        switch (rangeIndex)
        {
            case 0:
                grid = new Grid<PassengerSeat>(4, UnityEngine.Random.Range(5, 8), 7, defaultOrigin, gridDelegate);
                break;
            default:
                grid = new Grid<PassengerSeat>(3, UnityEngine.Random.Range(5, 8), 9, defaultOrigin, gridDelegate);
                break;
        }

        return new FloatingTilemapVisual(grid);
    }

    private void Start() {

        var vehicleFactory = new VehicleFactory();

        foreach (var laneConfig in lanesConfigurations)
        {
            var gridConfig = ViewPort.GenerateGridParametersForCameraViewport(3);

            Grid<PassengerSeat> gameArea = new Grid<PassengerSeat>(gridConfig.width, gridConfig.height, gridConfig.cellSize, defaultOrigin, gridDelegate);

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

                vehicle.prefab = floatingTilemap.GeneratePassengerGrid(vehicle.animatableObjects);
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
      

    }

}
