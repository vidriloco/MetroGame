using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using System;

public delegate void MetroStatusChangedHandler(VehicleStatus status);

public class MetroBehaviour : MonoBehaviour {

    private Vector2 defaultOrigin
    {
        get { return ViewPort.defaultOrigin; }
    }

    public GameObject metro
    {
        get {
            return GameObject.FindGameObjectWithTag(Tags.Metro);
        }
    }

    [SerializeField] private TilemapVisual tilemapVisual;
    [SerializeField] private Configurations.Lane[] lanesConfigurations;
    [SerializeField] public VisualPassenger visualPassenger;

    public event MetroStatusChangedHandler MetroStatusChanged;

    private Lane metroLane;

    private Func<Grid<PassengerSeat>, int, int, PassengerSeat> gridDelegate
    {
        get { return (Grid<PassengerSeat> g, int x, int y) => new PassengerSeat(x, y, g); }
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

            var vehicleManager = new VehicleManager(laneConfig.vehicles);
            vehicleManager.VehicleStatusChanged += VehicleManager_VehicleStatusChanged;

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

    private void VehicleManager_VehicleStatusChanged(VehicleStatus status)
    {
        MetroStatusChanged?.Invoke(status);

        switch (status) {
            case VehicleStatus.CloseDoors:
                MetroWillCloseDoors();
                break;
            case VehicleStatus.OpenDoors:
                MetroWillOpenDoors();
                break;
            case VehicleStatus.WillDepart:
                MetroWillDepart();
                break;
        }
    }

    private void MarkPassengersWithColor(Color color)
    {
        GameObject[] passengers = GameObject.FindGameObjectsWithTag(Tags.Passenger);
        foreach (var passenger in passengers)
        {
            passenger.GetComponent<SpriteRenderer>().color = color;
        }
    }

    private void MetroWillCloseDoors()
    {
        GameObject.FindObjectOfType<SoundManager>().PlayMetroSoundClosingDoors();

        MarkPassengersWithColor(Color.red);

        GameObject cover = GameObject.FindGameObjectWithTag(Tags.MetroCover);
        LeanTween.delayedCall(cover, 1f, () =>
        {
            LeanTween.alpha(cover, 1f, 2f).setOnComplete(() => {
                cover.tag = Tags.DiscardObject;
            });
        }).setOnCompleteOnRepeat(false);
    }

    private void MetroWillOpenDoors()
    {
        GameObject cover = GameObject.FindGameObjectWithTag(Tags.MetroCover);

        if (cover != null)
        {
            LeanTween.alpha(cover, 0f, 1f).setEaseLinear();
        }
    }

    private void MetroWillDepart()
    {
        GameObject.FindObjectOfType<SoundManager>().PlayMetroSoundDeparting();
        MarkPassengersWithColor(Color.white);
    }

    void FixedUpdate()
    {
        metroLane.FixedUpdate();
    }

    private void Update() {
        metroLane.Update();
    }

}
