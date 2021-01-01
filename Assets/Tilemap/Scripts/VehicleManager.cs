using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Configurations.Vehicle;

public class VehicleManager
{

    private Dictionary<String, TimeSequence> sequences = new Dictionary<string, TimeSequence>();
    private ArrayList movingVehicles = new ArrayList();
    private VehicleFactory vehicleFactory;
    private Func<GameObject> gameObjectCreator;
    private Configurations.Vehicle[] vehiclesConfigs;

    public VehicleManager(Tilemap tilemap, Configurations.Vehicle[] vehiclesConfigs, Func<GameObject> gameObjectCreator)
    {
        this.vehicleFactory = new VehicleFactory(tilemap.grid);
        this.gameObjectCreator = gameObjectCreator;
        this.vehiclesConfigs = vehiclesConfigs;
    }

    private Vector2 CalculateVectorDirectionFor(Direction direction) {
        Vector2 vectorDirection = Vector2.zero;

        switch (direction)
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

        return vectorDirection;
    }

    private void UpdateTimingsFor(MovingObject vehicle)
    {
        var movements = vehicle.config.movements;

        if (!sequences.ContainsKey(vehicle.config.id))
        {
            sequences[vehicle.config.id] = new TimeSequence(0, 0, vehicle.nextSpawnTime);
        }

        var timing = sequences[vehicle.config.id];
        if (timing.index < movements.Length)
        {
            var movement = movements[timing.index];
            if (Time.time - timing.lastTime >= movement.duration)
            {
                Debug.Log("Idx: " + timing.index + " Speed: " + movement.speed + " Duration: " + movement.duration + " TD: " + (Time.time - timing.lastTime));
                sequences[vehicle.config.id].lastTime = Time.time;
                sequences[vehicle.config.id].movement = movement;
                sequences[vehicle.config.id].index = timing.index + 1;
            }
        }
    }

    private void UpdateSegmentSpeedForVehicle(MovingObject vehicle)
    {
        UpdateTimingsFor(vehicle);

        if (sequences.ContainsKey(vehicle.config.id))
        {
            var movement = sequences[vehicle.config.id].movement;

            if (movement != null)
            {
                var vectorDirection = CalculateVectorDirectionFor(vehicle.config.startingPosition.direction);
                vehicle.gameObject.GetComponent<Rigidbody>().velocity = vectorDirection * movement.GetValueOrDefault().speed;
            }
        }
    }

    private void DestroyObject(MovingObject movingObject)
    {
        GameObject.DestroyImmediate(movingObject.gameObject);
        movingVehicles.Remove(movingObject);
        sequences.Remove(movingObject.config.id);
    }

    public void UpdateManagedVehicles(Func<MovingObject, bool> vehicleShouldBeDestroyed)
    {
        foreach (var movingVehicle in movingVehicles)
        {
            var vehicle = (MovingObject) movingVehicle;
            UpdateSegmentSpeedForVehicle(vehicle);

            if (vehicleShouldBeDestroyed(vehicle))
            {
                DestroyObject(vehicle);
            }
        }
    }

    public void InstantiateVehicles()
    {
        for (var i = 0; i < vehiclesConfigs.Length; i++)
        {
            var vehicleConfig = vehiclesConfigs[i];
            if (!sequences.ContainsKey(vehicleConfig.id))
            {
                vehicleConfig.prefab = gameObjectCreator();
                var vehicleMoving = vehicleFactory.LoadVehicleFromConfiguration(vehicleConfig);
                movingVehicles.Add(vehicleMoving);
            }
        }
    }

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
}
