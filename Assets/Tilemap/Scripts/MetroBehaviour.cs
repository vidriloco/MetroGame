using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using System;

public class MetroBehaviour : MonoBehaviour {

    private Vector2 defaultOrigin
    {
        get { return ViewPort.defaultOrigin; }
    }

    [SerializeField] private TilemapVisual tilemapVisual;
    [SerializeField] private Configurations.Lane[] lanesConfigurations;
    [SerializeField] public VisualPassenger visualPassenger;

    private Lane metroLane;

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


        return new FloatingTilemapVisual(grid, visualPassenger);
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


            metroLane = new Lane(new Tilemap(gameArea), vehicleManager);
            metroLane.vehicleInitialiser = (Configurations.Vehicle vehicle, Lane lane) =>
            {
                GameObject.FindObjectOfType<SoundManager>().PlayMetroSoundArriving();

                var position = lane.PositionForConfigurationVehicle(vehicle);
                var offsetX = lane.WorldDistanceFromOriginToPositionXAxis(vehicle.startingPosition.value);

                var floatingTilemap = BuildRandomPassengersLayout();

                vehicle.prefab = floatingTilemap.GeneratePassengerGrid();
                vehicle.childrenObjects = new GameObject[] { };
                var renderer = vehicle.prefab.GetComponent<SpriteRenderer>();
                // Change the position where the metro appears
                vehicle.prefab.transform.position = new Vector3(position.x, -180);
                return vehicleFactory.LoadVehicleFromConfiguration(vehicle);
            };
        }
    }

    void FixedUpdate()
    {
        metroLane.FixedUpdate();
    }

    private void Update() {
        metroLane.Update();
    }

}
