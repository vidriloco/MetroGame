using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Configurations.Vehicle;

public class VehicleManager
{

    private Dictionary<String, TimeSequence> sequences = new Dictionary<string, TimeSequence>();
    private ArrayList vehicles = new ArrayList();
    private Configurations.Vehicle[] vehiclesConfigs;

    public VehicleManager(Configurations.Vehicle[] vehiclesConfigs)
    {
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

    private void UpdateTimingsFor(ManagedVehicle vehicle)
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
                //Debug.Log("Idx: " + timing.index + " Speed: " + movement.speed + " Duration: " + movement.duration + " TD: " + (Time.time - timing.lastTime));
                sequences[vehicle.config.id].lastTime = Time.time;
                sequences[vehicle.config.id].movement = movement;
                sequences[vehicle.config.id].index = timing.index + 1;
            }
        }
    }

    private void UpdateSegmentSpeedForVehicle(ManagedVehicle vehicle)
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

    private void DestroyObject(ManagedVehicle vehicle)
    {
        GameObject.DestroyImmediate(vehicle.gameObject);
        vehicles.Remove(vehicle);
        sequences.Remove(vehicle.config.id);
    }

    public void UpdateManagedVehicles(Func<ManagedVehicle, bool> vehicleShouldBeDestroyed)
    {
        for(var index = 0; index < vehicles.Count; index++)
        {
            var vehicle = (ManagedVehicle) vehicles[index];
            UpdateSegmentSpeedForVehicle(vehicle);

            if (vehicleShouldBeDestroyed(vehicle))
            {
                DestroyObject(vehicle);
            }
        }
    }

    public void InstantiateVehicles(Func<Configurations.Vehicle, ManagedVehicle> vehicleInitialiser)
    {
        for (var i = 0; i < vehiclesConfigs.Length; i++)
        {
            var vehicleConfig = vehiclesConfigs[i];
            if (!sequences.ContainsKey(vehicleConfig.id))
            {
                var vehicleManager = vehicleInitialiser(vehicleConfig);
                vehicles.Add(vehicleManager);
                UpdateTimingsFor(vehicleManager);
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

public class ManagedVehicle
{
    public float nextSpawnTime;
    public Configurations.Vehicle config;
    public GameObject gameObject;
    public int? index = null;

    public ManagedVehicle(float nextSpawnTime, GameObject gameObject, Configurations.Vehicle vehicleConfig)
    {
        this.nextSpawnTime = nextSpawnTime;
        this.gameObject = gameObject;
        this.config = vehicleConfig;
    }
}
