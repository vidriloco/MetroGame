using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane
{
    private Tilemap tilemap;
    private Configurations.Vehicle[] vehiclesConfigs;

    private ArrayList movingVehicles = new ArrayList();

    public Dictionary<String, float> timeTables = new Dictionary<string, float>();

    private class GameObjectInLane
    {
        public String id;
        public float nextSpawnTime;
        public Configurations.Vehicle config;
        public GameObject gameObject;

        public GameObjectInLane(float nextSpawnTime, GameObject gameObject, Configurations.Vehicle vehicleConfig)
        {
            this.nextSpawnTime = nextSpawnTime;
            this.gameObject = gameObject;
            this.config = vehicleConfig;
        }
    }

    public Lane(Tilemap tilemap, Configurations.Vehicle[] vehiclesConfigs)
    {
        this.tilemap = tilemap;
        this.vehiclesConfigs = vehiclesConfigs;
    }

    public void Update()
    {
        Spawn();
    }

    public void FixedUpdate()
    {
        foreach(var item in movingVehicles)
        {
            var movingVehicle = (GameObjectInLane) item;
            var vectorDirection = movingVehicle.config.direction == Configurations.Vehicle.Direction.Left ? new Vector2(-1, 0) : new Vector2(1, 0);
            movingVehicle.gameObject.GetComponent<Rigidbody>().velocity = vectorDirection * movingVehicle.config.speed;

            TryToDestroyMovingObject(movingVehicle);
        }
    }

    private void InstantiateVehicleFrom(Configurations.Vehicle vehicleConfig)
    {
        var vehicleMoving = InitializeMovingVehicleWith(vehicleConfig);
        movingVehicles.Add(vehicleMoving);
        timeTables[vehicleConfig.id] = vehicleMoving.nextSpawnTime;
    }

    private void Spawn()
    {
        foreach(var vehicleConfig in vehiclesConfigs)
        {
            if (!timeTables.ContainsKey(vehicleConfig.id))
            {
                InstantiateVehicleFrom(vehicleConfig);
            }

            if (Time.time > timeTables[vehicleConfig.id])
            {
                InstantiateVehicleFrom(vehicleConfig);
            }
        }
    }

    private GameObjectInLane InitializeMovingVehicleWith(Configurations.Vehicle vehicleConfig)
    {
        var startingX = vehicleConfig.direction == Configurations.Vehicle.Direction.Left ? tilemap.grid.GetRightEnd() : tilemap.grid.GetLeftEnd();
        var quaternion = vehicleConfig.direction == Configurations.Vehicle.Direction.Left ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);

        var startingPosition = new Vector2(startingX, tilemap.grid.GetHeightAtPosition(vehicleConfig.startingPositionAtY));
        var movingVehicle = (GameObject)GameObject.Instantiate(vehicleConfig.prefab, startingPosition, quaternion);

        return new GameObjectInLane(Time.time + vehicleConfig.frequency, movingVehicle, vehicleConfig);
    }

    private void TryToDestroyMovingObject(GameObjectInLane movingObject)
    {
        if(movingObject == null) { return; }

        var renderer = movingObject.gameObject.GetComponent<SpriteRenderer>();

        if (movingObject.config.direction == Configurations.Vehicle.Direction.Left)
        {
            if (renderer.bounds.max.x < tilemap.grid.GetLeftEnd())
            {
                GameObject.DestroyImmediate(movingObject.gameObject);
                movingVehicles.Remove(movingObject);
            }
        }
        else
        {
            if (renderer.bounds.min.x > tilemap.grid.GetRightEnd())
            {
                GameObject.DestroyImmediate(movingObject.gameObject);
                movingVehicles.Remove(movingObject);
            }
        }
    }
}
