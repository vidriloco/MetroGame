using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane
{
    private Tilemap tilemap;
    private Configurations.Vehicle[] vehiclesConfigs;
    private GameObjectInLane[] movingVehicles;

    private float nextSpawnTime;

    private class GameObjectInLane
    {
        public float nextSpawnTime;
        public GameObject gameObject;

        public GameObjectInLane(float nextSpawnTime, GameObject gameObject)
        {
            this.nextSpawnTime = nextSpawnTime;
            this.gameObject = gameObject;
        }
    }

    public Lane(Tilemap tilemap, Configurations.Vehicle[] vehiclesConfigs)
    {
        this.tilemap = tilemap;
        this.vehiclesConfigs = vehiclesConfigs;
        this.movingVehicles = new GameObjectInLane[vehiclesConfigs.Length];
    }

    public void Update()
    {
        Spawn();
    }

    public void FixedUpdate()
    {
        for (var i = 0; i < movingVehicles.Length; i++)
        {
            if(movingVehicles[i] != null)
            {
                var vectorDirection = vehiclesConfigs[i].direction == Configurations.Vehicle.Direction.Left ? new Vector2(-1, 0) : new Vector2(1, 0);
                movingVehicles[i].gameObject.GetComponent<Rigidbody>().velocity = vectorDirection * vehiclesConfigs[i].speed;
            }
        }
    }

    private void Spawn()
    {
        for(var i = 0; i < vehiclesConfigs.Length; i++)
        {
            var vehicleConfig = vehiclesConfigs[i];

            if (movingVehicles[i] == null || Time.time > movingVehicles[i].nextSpawnTime)
            {
                movingVehicles[i] = InitializeMovingVehicleWith(vehicleConfig);
            } else
            {
                if (DestroyMovingObject(movingVehicles[i], vehicleConfig))
                {
                    movingVehicles[i] = null;
                }
            }
        }
    }

    private GameObjectInLane InitializeMovingVehicleWith(Configurations.Vehicle vehicleConfig)
    {
        var startingX = vehicleConfig.direction == Configurations.Vehicle.Direction.Left ? tilemap.grid.GetRightEnd() : tilemap.grid.GetLeftEnd();
        var quaternion = vehicleConfig.direction == Configurations.Vehicle.Direction.Left ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);

        var startingPosition = new Vector2(startingX, tilemap.grid.GetHeightAtPosition(vehicleConfig.startingPositionAtY));
        var movingVehicle = (GameObject)GameObject.Instantiate(vehicleConfig.prefab, startingPosition, quaternion);

        return new GameObjectInLane(Time.time + vehicleConfig.frequency, movingVehicle);
    }

    private bool DestroyMovingObject(GameObjectInLane movingObject, Configurations.Vehicle vehicleConfig)
    {
        if(movingObject == null) { return false; }

        var renderer = movingObject.gameObject.GetComponent<SpriteRenderer>();

        if (vehicleConfig.direction == Configurations.Vehicle.Direction.Left)
        {
            if (renderer.bounds.max.x < tilemap.grid.GetLeftEnd())
            {
                GameObject.DestroyImmediate(movingObject.gameObject);
                return true;
            }
        }
        else
        {
            if (renderer.bounds.min.x > tilemap.grid.GetRightEnd())
            {
                GameObject.DestroyImmediate(movingObject.gameObject);
                return true;
            }
        }

        return false;
    }
}
