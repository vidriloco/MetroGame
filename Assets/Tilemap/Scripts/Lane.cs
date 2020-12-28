using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane
{
    public Tilemap tilemap;
    private Configurations.Vehicle[] vehiclesConfigs;

    private ArrayList movingVehicles = new ArrayList();

    public Dictionary<String, float> timeTables = new Dictionary<string, float>();

    private class GameObjectInLane
    {
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
        for(var index = 0; index < movingVehicles.Count; index++)
        {
            var movingVehicle = (GameObjectInLane) movingVehicles[index];
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
        Vector2 startingPosition = Vector2.zero;
        Quaternion quaternion = Quaternion.identity;

        switch (vehicleConfig.startingPosition.direction)
        {
            case Configurations.Vehicle.Direction.LeftToRight:
                startingPosition = new Vector2(tilemap.grid.GetLeftEnd(), tilemap.grid.GetHeightAtPosition(vehicleConfig.startingPosition.value));;
                quaternion = Quaternion.Euler(0, 180, 0);
                break;
            case Configurations.Vehicle.Direction.RightToLeft:
                startingPosition = new Vector2(tilemap.grid.GetRightEnd(), tilemap.grid.GetHeightAtPosition(vehicleConfig.startingPosition.value));
                quaternion = Quaternion.Euler(0, 0, 0);
                break;
            case Configurations.Vehicle.Direction.TopToBottom:
                startingPosition = new Vector2(tilemap.grid.GetWidthAtPosition(vehicleConfig.startingPosition.value), tilemap.grid.GetTopEnd());
                quaternion = Quaternion.Euler(0, 0, 90);
                break;
            case Configurations.Vehicle.Direction.BottomToTop:
                startingPosition = new Vector2(tilemap.grid.GetWidthAtPosition(vehicleConfig.startingPosition.value), tilemap.grid.GetBottomEnd());
                quaternion = Quaternion.Euler(0, 0, -90);
                break;
        }

        var movingVehicle = vehicleConfig.prefab;
        movingVehicle.transform.position = startingPosition;

        if (vehicleConfig.isPrefab) {
            movingVehicle = (GameObject)GameObject.Instantiate(vehicleConfig.prefab, startingPosition, quaternion);
        }

        InstantiatePrefabsForGameObject(vehicleConfig.id, movingVehicle, vehicleConfig.childrenObjects);

        var vehicleMoving = new GameObjectInLane(Time.time + vehicleConfig.frequency, movingVehicle, vehicleConfig);
        movingVehicles.Add(vehicleMoving);
        timeTables[vehicleConfig.id] = vehicleMoving.nextSpawnTime;
    }

    private void InstantiatePrefabsForGameObject(String id, GameObject gameObject, GameObject[] childrenPrefabs)
    {

        foreach (var prefab in childrenPrefabs)
        {
            var attachedPrefab = (GameObject) GameObject.Instantiate(prefab, gameObject.transform.position + new Vector3(2,3), Quaternion.Euler(0, 0, 0));
            attachedPrefab.GetComponent<SpriteRenderer>().sortingOrder = 1;
            attachedPrefab.transform.SetParent(gameObject.gameObject.transform);
        }

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

    private void TryToDestroyMovingObject(GameObjectInLane movingObject)
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
