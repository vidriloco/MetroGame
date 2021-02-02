using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleFactory
{
    public VehicleFactory() { }

    public void applyRotation(Configurations.Vehicle configuration)
    {
        var quaternion = Quaternion.identity;

        switch (configuration.startingPosition.direction)
        {
            case Configurations.Vehicle.Direction.LeftToRight:
                quaternion = Quaternion.Euler(0, 180, 0);
                break;
            case Configurations.Vehicle.Direction.RightToLeft:
                quaternion = Quaternion.Euler(0, 0, 0);
                break;
            case Configurations.Vehicle.Direction.TopToBottom:
                quaternion = Quaternion.Euler(0, 0, 90);
                break;
            case Configurations.Vehicle.Direction.BottomToTop:
                quaternion = Quaternion.Euler(0, 0, -90);
                break;
        }
        configuration.prefab.transform.rotation = quaternion;
    }

    public ManagedVehicle LoadVehicleFromConfiguration(Configurations.Vehicle configuration)
    {   
        InstantiatePrefabsForGameObject(configuration.id, configuration.prefab, configuration.childrenObjects);

        float frequency = 0;
        foreach(var movement in configuration.movements)
        {
            frequency += movement.duration;
        }

        return new ManagedVehicle(Time.time + frequency, configuration.prefab, configuration);
    }

    private void InstantiatePrefabsForGameObject(String id, GameObject gameObject, GameObject[] childrenPrefabs)
    {

        foreach (var prefab in childrenPrefabs)
        {
            var attachedPrefab = (GameObject)GameObject.Instantiate(prefab, gameObject.transform.position + new Vector3(2, 3), Quaternion.Euler(0, 0, 0));
            attachedPrefab.GetComponent<SpriteRenderer>().sortingOrder = 1;
            attachedPrefab.transform.SetParent(gameObject.gameObject.transform);
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
