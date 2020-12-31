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

    private Dictionary<String, TimeSequence> sequences = new Dictionary<string, TimeSequence>();

    private class TimeSequence
    {
        public float lastTime;
        public int index;
        public float frequency;
        public Configurations.Vehicle.InterpolatedMovement? movement;

        public TimeSequence(float lastTime, int index, float frequency)
        {
            this.lastTime = lastTime;
            this.index = index;
            this.frequency = frequency;
        }
    }


    public Lane(Tilemap tilemap, Configurations.Vehicle[] vehiclesConfigs, FloatingTilemapVisual floatingTilemap)
    {
        this.tilemap = tilemap;
        this.vehiclesConfigs = vehiclesConfigs;
        vehicleFactory = new VehicleFactory(tilemap.grid);
        this.floatingTilemap = floatingTilemap;
    }

    public void Update()
    {
        foreach (var vehicleConfig in vehiclesConfigs)
        {
            if (!sequences.ContainsKey(vehicleConfig.id))
            {
                InstantiateVehicleFrom(vehicleConfig);
            }
        }
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
                var movement = sequences[movingVehicle.config.id].movement;

                if(movement != null)
                {
                    movingVehicle.gameObject.GetComponent<Rigidbody>().velocity = vectorDirection * movement.GetValueOrDefault().speed;
                }
            }

            TryToDestroyMovingObject(movingVehicle);
        }
    }

    private void UpdateSpeedAsync(MovingObject vehicle)
    {
        var movements = vehicle.config.movements;

        if(!sequences.ContainsKey(vehicle.config.id))
        {
            sequences[vehicle.config.id] = new TimeSequence(0, 0, vehicle.nextSpawnTime);
        }

        var timing = sequences[vehicle.config.id];
        if (timing.index < movements.Length)
        {
            var movement = vehicle.config.movements[timing.index];
            if (Time.time - timing.lastTime >= movement.duration)
            {
                Debug.Log("Idx: " + timing.index + " Speed: " + movement.speed + " Duration: " + movement.duration + " TD: " + (Time.time - timing.lastTime));
                sequences[vehicle.config.id].lastTime = Time.time;
                sequences[vehicle.config.id].movement = movement;
                sequences[vehicle.config.id].index = timing.index + 1;
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
    }

    private void DestroyGameObject(MovingObject movingObject)
    {
        GameObject.DestroyImmediate(movingObject.gameObject);
        movingVehicles.Remove(movingObject);
        sequences.Remove(movingObject.config.id);
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
