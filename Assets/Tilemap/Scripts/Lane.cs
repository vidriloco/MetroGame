using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Lane
{
    public Tilemap tilemap;
    private Configurations.Vehicle[] vehiclesConfigs;
    private VehicleFactory vehicleFactory;
    private FloatingTilemapVisual floatingTilemap;

    private ArrayList movingVehicles = new ArrayList();

    public Dictionary<String, float> frequencies = new Dictionary<string, float>();
    public Dictionary<String, Configurations.Vehicle.Movement?> sequences = new Dictionary<string, Configurations.Vehicle.Movement?>();
    public Dictionary<String, int?> indexes = new Dictionary<string, int?>();

    public Lane(Tilemap tilemap, Configurations.Vehicle[] vehiclesConfigs, FloatingTilemapVisual floatingTilemap)
    {
        this.tilemap = tilemap;
        this.vehiclesConfigs = vehiclesConfigs;
        vehicleFactory = new VehicleFactory(tilemap.grid);
        this.floatingTilemap = floatingTilemap;
    }

    public void Update()
    {
        Spawn();
    }

    public void FixedUpdate()
    {
        for(var index = 0; index < movingVehicles.Count; index++)
        {
            var movingVehicle = (VehicleInGameObject) movingVehicles[index];
            Vector2 vectorDirection = Vector2.zero;

            switch (movingVehicle.config.startingPosition.direction)
            {
                case Configurations.Vehicle.Direction.LeftToRight:
                    vectorDirection = new Vector2(1, 0);
                    break;
                case Configurations.Vehicle.Direction.RightToLeft:
                    vectorDirection = new Vector2(-1, 0);
                    break;
                case Configurations.Vehicle.Direction.TopToBottom:
                    vectorDirection = new Vector2(0, -1);
                    break;
                case Configurations.Vehicle.Direction.BottomToTop:
                    vectorDirection = new Vector2(0, 1);
                    break;
            }

            UpdateSpeedAsync(0, movingVehicle);
            indexes[movingVehicle.config.id] = 0;
            if (!sequences.ContainsKey(movingVehicle.config.id))
            {
                Debug.Log("Hola");
                movingVehicle.gameObject.GetComponent<Rigidbody>().velocity = vectorDirection * 0;
            } else
            {
                Configurations.Vehicle.Movement sequence = sequences[movingVehicle.config.id].GetValueOrDefault();

                movingVehicle.gameObject.GetComponent<Rigidbody>().velocity = vectorDirection * sequence.speed;
            }

            TryToDestroyMovingObject(movingVehicle);
        }
    }

    private async Task UpdateSpeedAsync(int index, VehicleInGameObject vehicle)
    {
        Debug.Log("Adios");

        var movements = vehicle.config.movements;
        if (index < movements.Length && indexes[vehicle.config.id] != index)
        {
            Debug.Log(4234234);

            var movement = vehicle.config.movements[index];
            sequences[vehicle.config.id] = movement;
            await Task.Delay((int) movement.duration);
            UpdateSpeedAsync(index+1, vehicle);

            indexes[vehicle.config.id] = index+1;
        }
    }

    private void InstantiateVehicleFrom(Configurations.Vehicle vehicleConfig)
    {
        if(!vehicleConfig.isPrefab)
        {
            vehicleConfig.prefab = floatingTilemap.GetPivotWithVisualRepresentation();
        }

        var vehicleMoving = vehicleFactory.LoadVehicleFromConfiguration(vehicleConfig);
        movingVehicles.Add(vehicleMoving);
        frequencies[vehicleConfig.id] = vehicleMoving.nextSpawnTime;
    }

    private void Spawn()
    {
        foreach(var vehicleConfig in vehiclesConfigs)
        {
            if (!frequencies.ContainsKey(vehicleConfig.id) || Time.time > frequencies[vehicleConfig.id])
            {
                InstantiateVehicleFrom(vehicleConfig);
            }
        }
    }

    private void TryToDestroyMovingObject(VehicleInGameObject movingObject)
    {
        if(movingObject == null) { return; }

        var renderer = movingObject.gameObject.GetComponent<SpriteRenderer>();

        switch (movingObject.config.startingPosition.direction)
        {
            case Configurations.Vehicle.Direction.LeftToRight:
                if (renderer.bounds.min.x > tilemap.grid.GetRightEnd())
                {
                    GameObject.DestroyImmediate(movingObject.gameObject);
                    movingVehicles.Remove(movingObject);
                    sequences[movingObject.config.id] = null;
                }
                break;
            case Configurations.Vehicle.Direction.RightToLeft:
                if (renderer.bounds.max.x < tilemap.grid.GetLeftEnd())
                {
                    GameObject.DestroyImmediate(movingObject.gameObject);
                    movingVehicles.Remove(movingObject);
                    sequences[movingObject.config.id] = null;
                }
                break;
            case Configurations.Vehicle.Direction.TopToBottom:
                if (renderer.bounds.max.y < tilemap.grid.GetBottomEnd())
                {
                    GameObject.DestroyImmediate(movingObject.gameObject);
                    movingVehicles.Remove(movingObject);
                    sequences[movingObject.config.id] = null;
                }
                break;
            case Configurations.Vehicle.Direction.BottomToTop:
                if (renderer.bounds.min.y > tilemap.grid.GetTopEnd())
                {
                    GameObject.DestroyImmediate(movingObject.gameObject);
                    movingVehicles.Remove(movingObject);
                    sequences[movingObject.config.id] = null;
                }
                break;
        }
    }
}
