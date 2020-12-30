using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configurations
{

    [System.Serializable]
    public struct Lane
    {
        public int width;
        public int height;
        public float cellSize;
        public Vector3 position;
        [SerializeField] public Vehicle[] vehicles;
    }

    [System.Serializable]
    public struct Vehicle
    {
        public string id;
        public bool isPrefab;
        public GameObject prefab;

        [SerializeField] public InitialPosition startingPosition;
        [SerializeField] public InterpolatedMovement[] movements;

        public enum Direction
        {
            LeftToRight,
            RightToLeft,
            TopToBottom,
            BottomToTop
        }

        [System.Serializable]
        public struct InterpolatedMovement
        {
            public float speed;
            public float duration;
        }

        [System.Serializable]
        public struct InitialPosition
        {
            public Direction direction;
            public int value;
        }

        [SerializeField] public GameObject[] childrenObjects;
    }
}
