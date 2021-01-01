using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Lane
{
    public Tilemap tilemap;
    private VehicleManager vehicleManager;

    public Lane(Tilemap tilemap, VehicleManager vehicleManager)
    {
        this.tilemap = tilemap;
        this.vehicleManager = vehicleManager;
    }

    public void Update()
    {
        vehicleManager.InstantiateVehicles();
    }

    public void FixedUpdate()
    {
        vehicleManager.UpdateManagedVehicles(ShouldRemoveMovingObject);
    }

    private bool ShouldRemoveMovingObject(MovingObject movingObject)
    {
        if(movingObject == null) { return false; }

        var renderer = movingObject.gameObject.GetComponent<SpriteRenderer>();

        switch (movingObject.config.startingPosition.direction)
        {
            case Configurations.Vehicle.Direction.LeftToRight:
                return renderer.bounds.min.x > tilemap.grid.GetRightEnd();
            case Configurations.Vehicle.Direction.RightToLeft:
                return renderer.bounds.max.x < tilemap.grid.GetLeftEnd();
            case Configurations.Vehicle.Direction.TopToBottom:
                return renderer.bounds.max.y < tilemap.grid.GetBottomEnd();
            case Configurations.Vehicle.Direction.BottomToTop:
                return renderer.bounds.min.y > tilemap.grid.GetTopEnd();
            default:
                return false;
        }
    }
}
