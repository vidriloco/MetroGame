using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane
{
    public enum Direction
    {
        Left,
        Right
    }

    private Direction direction;
    private Tilemap tilemap;
    private Testing.MovingObject[] movingObjects;

    private float nextSpawnTime;

    private Testing.MovingObject currentMovingObject;
    private GameObject movingObject;

    public Lane(Tilemap tilemap, Testing.MovingObject[] movingObjects, Direction direction)
    {
        this.tilemap = tilemap;
        this.movingObjects = movingObjects;
        this.direction = direction;
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
            var vectorDirection = direction == Direction.Left ? new Vector2(-1, 0) : new Vector2(1, 0);
            movingObject.GetComponent<Rigidbody>().velocity = vectorDirection * currentMovingObject.speed;
        }
    }

    private void Spawn()
    {
        var rand = new System.Random();
        var indexNumber = rand.Next(0, movingObjects.Length);

        currentMovingObject = movingObjects[indexNumber];

        var startingX = direction == Direction.Left ? tilemap.grid.GetRightEnd() : tilemap.grid.GetLeftEnd();
        var quaternion = direction == Direction.Left ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        
        var startingPosition = new Vector2(startingX, tilemap.grid.GetHeightAtPosition(currentMovingObject.startingPositionAtY));
        movingObject = (GameObject) GameObject.Instantiate(currentMovingObject.prefab, startingPosition, quaternion);

        nextSpawnTime = Time.time + currentMovingObject.frequency;
    }

    private void Destroy()
    {
        if(movingObject == null) { return; }

        var renderer = movingObject.GetComponent<SpriteRenderer>();

        if (direction == Direction.Left)
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
