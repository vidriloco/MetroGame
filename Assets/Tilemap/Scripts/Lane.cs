using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Lane
{
    public Tilemap tilemap;
    private VehicleManager vehicleManager;
    private VehicleFactory vehicleFactory;
    private Func<Configurations.Vehicle, Lane, ManagedVehicle> vehicleInitialiser;

    public Lane(Tilemap tilemap, VehicleManager vehicleManager, Func<Configurations.Vehicle, Lane, ManagedVehicle> vehicleInitialiser)
    {
        this.tilemap = tilemap;
        this.vehicleManager = vehicleManager;
        this.vehicleInitialiser = vehicleInitialiser;
    }

    public void DetectTapOnPosition(Vector2 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);

        if(hit.collider != null)
        {
            Debug.Log(hit.collider.gameObject.name);
        }
    }

    public void Update()
    {
        vehicleManager.InstantiateVehicles((Configurations.Vehicle vehicle) =>
        {
            return vehicleInitialiser(vehicle, this);
        });
    }

    public void FixedUpdate()
    {
        vehicleManager.UpdateManagedVehicles(ShouldRemoveMovingObject);
    }

    public float WorldDistanceFromOriginToPositionXAxis(int position)
    {
        return tilemap.grid.GetWidthAtPosition(position) - tilemap.grid.GetWidthAtPosition(0);
    }

    public Vector2 PositionForConfigurationVehicle(Configurations.Vehicle vehicle)
    {
        Vector2 startingPosition = Vector2.zero;

        switch (vehicle.startingPosition.direction)
        {
            case Configurations.Vehicle.Direction.LeftToRight:
                startingPosition = new Vector2(tilemap.grid.GetLeftEnd(), tilemap.grid.GetHeightAtPosition(vehicle.startingPosition.value));
                break;
            case Configurations.Vehicle.Direction.RightToLeft:
                startingPosition = new Vector2(tilemap.grid.GetRightEnd(), tilemap.grid.GetHeightAtPosition(vehicle.startingPosition.value));
                break;
            case Configurations.Vehicle.Direction.TopToBottom:
                startingPosition = new Vector2(tilemap.grid.GetWidthAtPosition(vehicle.startingPosition.value), tilemap.grid.GetTopEnd());
                break;
            case Configurations.Vehicle.Direction.BottomToTop:
                startingPosition = new Vector2(tilemap.grid.GetWidthAtPosition(vehicle.startingPosition.value), tilemap.grid.GetBottomEnd());
                break;
        }

        return startingPosition;
    }

    private bool ShouldRemoveMovingObject(ManagedVehicle vehicle)
    {
        if(vehicle == null) { return false; }

        var renderer = vehicle.gameObject.GetComponent<SpriteRenderer>();

        switch (vehicle.config.startingPosition.direction)
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
