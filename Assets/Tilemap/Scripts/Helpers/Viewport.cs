using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ViewPort
{
    public static Vector2 defaultOrigin
    {
        get
        {
            return Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        }
    }

    public static Vector2 WithPadding(float x, float y)
    {
        return defaultOrigin + new Vector2(x, y);
    }

    public static float RightMostEdge()
    {
        var camera = Camera.main;
        var aspect = Camera.main.aspect;

        return camera.orthographicSize * aspect * 2;
    }

    public static float TopMostEdge()
    {
        var camera = Camera.main;
        return camera.orthographicSize * 2;
    }

    public static Configurations.Grid GenerateGridParametersForCameraViewport(int dimension)
    {
        var camera = Camera.main;
        var aspect = Camera.main.aspect;

        var height = camera.orthographicSize * 2;
        var width = camera.orthographicSize * aspect * 2;

        return new Configurations.Grid((int) width / dimension, (int) height / dimension, dimension);
    } 
}