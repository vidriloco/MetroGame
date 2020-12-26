using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane
{
    private Tilemap tilemap;
    private Configurations.Vehicle[] vehicles;
    private ArrayList movingVehicles = new ArrayList();

    private float nextSpawnTime;

    private Configurations.Vehicle currentVehicle;
    private GameObject movingObject;

    public Lane(Tilemap tilemap, Configurations.Vehicle[] vehicles)
    {
        this.tilemap = tilemap;
        this.vehicles = vehicles;
    }

    public void Update()
    {
        if(Time.time > nextSpawnTime)
        {
            Spawn();
        }

        Destroy();
    }

    public void FixedUpdate()
    {
        if (movingObject != null)
        {
            var vectorDirection = currentVehicle.direction == Configurations.Vehicle.Direction.Left ? new Vector2(-1, 0) : new Vector2(1, 0);
            movingObject.GetComponent<Rigidbody>().velocity = vectorDirection * currentVehicle.speed;
        }
    }

    private void Spawn()
    {

        var rand = new System.Random();
        var indexNumber = rand.Next(0, vehicles.Length);

        currentVehicle = vehicles[indexNumber];

        var startingX = currentVehicle.direction == Configurations.Vehicle.Direction.Left ? tilemap.grid.GetRightEnd() : tilemap.grid.GetLeftEnd();
        var quaternion = currentVehicle.direction == Configurations.Vehicle.Direction.Left ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        
        var startingPosition = new Vector2(startingX, tilemap.grid.GetHeightAtPosition(currentVehicle.startingPositionAtY));
        movingObject = (GameObject) GameObject.Instantiate(currentVehicle.prefab, startingPosition, quaternion);

        nextSpawnTime = Time.time + currentVehicle.frequency;
    }

    private void Destroy()
    {
        if(movingObject == null) { return; }

        var renderer = movingObject.GetComponent<SpriteRenderer>();

        if (currentVehicle.direction == Configurations.Vehicle.Direction.Left)
        {
            if (renderer.bounds.max.x < tilemap.grid.GetLeftEnd())
            {
                GameObject.DestroyImmediate(movingObject);
            }
        }
        else
        {
            if (renderer.bounds.min.x > tilemap.grid.GetRightEnd())
            {
                GameObject.DestroyImmediate(movingObject);
            }
        }
    }
}
