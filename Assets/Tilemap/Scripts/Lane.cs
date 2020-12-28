using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane
{
    public Tilemap tilemap;
    private Configurations.Vehicle[] vehiclesConfigs;
    private VehicleFactory vehicleFactory;
    private FloatingTilemapVisual floatingTilemap;

    private ArrayList movingVehicles = new ArrayList();

    public Dictionary<String, float> timeTables = new Dictionary<string, float>();

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

            movingVehicle.gameObject.GetComponent<Rigidbody>().velocity = vectorDirection * movingVehicle.config.speed;

            TryToDestroyMovingObject(movingVehicle);
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
        timeTables[vehicleConfig.id] = vehicleMoving.nextSpawnTime;
    }

    private void Spawn()
    {
        foreach(var vehicleConfig in vehiclesConfigs)
        {
            if (!timeTables.ContainsKey(vehicleConfig.id) || Time.time > timeTables[vehicleConfig.id])
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
                }
                break;
            case Configurations.Vehicle.Direction.RightToLeft:
                if (renderer.bounds.max.x < tilemap.grid.GetLeftEnd())
                {
                    GameObject.DestroyImmediate(movingObject.gameObject);
                    movingVehicles.Remove(movingObject);
                }
                break;
            case Configurations.Vehicle.Direction.TopToBottom:
                if (renderer.bounds.max.y < tilemap.grid.GetBottomEnd())
                {
                    GameObject.DestroyImmediate(movingObject.gameObject);
                    movingVehicles.Remove(movingObject);
                }
                break;
            case Configurations.Vehicle.Direction.BottomToTop:
                if (renderer.bounds.min.y > tilemap.grid.GetTopEnd())
                {
                    GameObject.DestroyImmediate(movingObject.gameObject);
                    movingVehicles.Remove(movingObject);
                }
                break;
        }
    }
}
