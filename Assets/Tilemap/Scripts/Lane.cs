using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Lane
{
    public Tilemap tilemap;
    private VehicleFactory vehicleFactory;

    public Lane(Tilemap tilemap)
    {
        this.tilemap = tilemap;
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
}
