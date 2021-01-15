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

        var renderers = SpriteRenderersOfVehicle(vehicle);

        switch (vehicle.config.startingPosition.direction)
        {
            case Configurations.Vehicle.Direction.LeftToRight:
                return renderers[0].bounds.min.x > tilemap.grid.GetRightEnd();
            case Configurations.Vehicle.Direction.RightToLeft:
                return renderers[0].bounds.max.x < tilemap.grid.GetLeftEnd();
            case Configurations.Vehicle.Direction.TopToBottom:
                return renderers[0].bounds.max.y < tilemap.grid.GetBottomEnd();
            case Configurations.Vehicle.Direction.BottomToTop:

                var minRenderer = renderers[0].bounds.min.y;

                foreach(var renderer in renderers)
                {
                    if (renderer.bounds.min.y < minRenderer)
                    {
                        minRenderer = renderer.bounds.min.y;
                    }
                }

                return minRenderer > tilemap.grid.GetTopEnd();
            default:
                return false;
        }
    }

    private List<SpriteRenderer> SpriteRenderersOfVehicle(ManagedVehicle vehicle)
    {
        List<SpriteRenderer> renderers = new List<SpriteRenderer>();

        var renderer = vehicle.gameObject.GetComponent<SpriteRenderer>();

        renderers.Add(renderer);

        var children = vehicle.gameObject.transform.GetComponentsInChildren<SpriteRenderer>();

        foreach(var childTransform in children)
        {
            renderers.Add(childTransform);
        }

        return renderers;
    }
}
