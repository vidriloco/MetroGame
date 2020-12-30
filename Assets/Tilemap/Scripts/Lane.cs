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

    public Dictionary<String, float?> frequencies = new Dictionary<string, float?>();
    public Dictionary<String, Configurations.Vehicle.InterpolatedMovement?> sequences = new Dictionary<string, Configurations.Vehicle.InterpolatedMovement?>();

    public float lastTime = 0;
    public int index = 0;

    

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
            var movingVehicle = (MovingObject) movingVehicles[index];
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

            UpdateSpeedAsync(movingVehicle);

            if (sequences.ContainsKey(movingVehicle.config.id))
            {
                Configurations.Vehicle.InterpolatedMovement sequence = sequences[movingVehicle.config.id].GetValueOrDefault();

                movingVehicle.gameObject.GetComponent<Rigidbody>().velocity = vectorDirection * sequence.speed;
            }

            TryToDestroyMovingObject(movingVehicle);
        }
    }

    private void UpdateSpeedAsync(MovingObject vehicle)
    {
        var movements = vehicle.config.movements;

        if (index < movements.Length)
        {
            var movement = vehicle.config.movements[index];
            if (Time.time - lastTime >= movement.duration)
            {
                Debug.Log("Idx: " + index + " Speed: " + movement.speed + " Duration: " + movement.duration + " TD: " + (Time.time - lastTime));
                lastTime = Time.time;
                sequences[vehicle.config.id] = movement;
                index = index + 1;
            }
        }
    }

    private void InstantiateVehicleFrom(Configurations.Vehicle vehicleConfig)
    {
        if (!vehicleConfig.isPrefab)
        {
            vehicleConfig.prefab = floatingTilemap.GetPivotWithVisualRepresentation();
        }

        var vehicleMoving = vehicleFactory.LoadVehicleFromConfiguration(vehicleConfig);
        movingVehicles.Add(vehicleMoving);
        frequencies[vehicleConfig.id] = vehicleMoving.nextSpawnTime;
    }

    private void Spawn()
    {
        foreach (var vehicleConfig in vehiclesConfigs)
        {
            if (!frequencies.ContainsKey(vehicleConfig.id))
            {
                InstantiateVehicleFrom(vehicleConfig);
            }
        }
    }

    private void DestroyGameObject(MovingObject movingObject)
    {
        GameObject.DestroyImmediate(movingObject.gameObject);
        movingVehicles.Remove(movingObject);
        sequences.Remove(movingObject.config.id);
        frequencies.Remove(movingObject.config.id);
        index = 0;
        lastTime = 0;
    }

    private void TryToDestroyMovingObject(MovingObject movingObject)
    {
        if(movingObject == null) { return; }

        /*if(Time.time > frequencies[movingObject.config.id])
        {
            DestroyGameObject(movingObject);
            return;
        }*/

        var renderer = movingObject.gameObject.GetComponent<SpriteRenderer>();

        switch (movingObject.config.startingPosition.direction)
        {
            case Configurations.Vehicle.Direction.LeftToRight:
                if (renderer.bounds.min.x > tilemap.grid.GetRightEnd())
                {
                    DestroyGameObject(movingObject);
                }
                break;
            case Configurations.Vehicle.Direction.RightToLeft:
                if (renderer.bounds.max.x < tilemap.grid.GetLeftEnd())
                {
                    DestroyGameObject(movingObject);
                }
                break;
            case Configurations.Vehicle.Direction.TopToBottom:
                if (renderer.bounds.max.y < tilemap.grid.GetBottomEnd())
                {
                    DestroyGameObject(movingObject);
                }
                break;
            case Configurations.Vehicle.Direction.BottomToTop:
                if (renderer.bounds.min.y > tilemap.grid.GetTopEnd())
                {
                    DestroyGameObject(movingObject);
                }
                break;
        }
    }
}
