using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{

    [SerializeField] public MetroBehaviour metroBehaviour;
    private UnityEngine.Tilemaps.Tilemap tilemap;
    private ArrayList movingSpots = new ArrayList();
    private GameObject platform;
    public  float speed = 2f;
    private float waitTime = 2f;
    public  float startWaitTime;

    private void Start()
    {
        tilemap = GetComponentInChildren<UnityEngine.Tilemaps.Tilemap>();
        platform = GameObject.FindGameObjectWithTag(Tags.Platform);

        for (int idx = 0; idx < 10; idx++)
        {
            var position = RandomPosition();

            var visualPassengerClone = GameObject.Instantiate(metroBehaviour.visualPassenger, position, Quaternion.Euler(0, 0, 0));
            visualPassengerClone.ConfigureAsPassengerInTrain(false);
            visualPassengerClone.SetParentAndPosition(platform.transform, position);

            movingSpots.Add(position);
        }
    }

    private readonly float offset = 10;

    public UnityEngine.Tilemaps.Tilemap GetTilemap()
    {
        return tilemap;
    }

    private Vector2 RandomPosition()
    {
        var xPosition = Random.Range(tilemap.localBounds.min.x + offset, tilemap.localBounds.max.x - offset);
        var yPosition = Random.Range(tilemap.localBounds.min.y + offset, tilemap.localBounds.max.y - offset);
        return new Vector2(xPosition, yPosition);
    }

    private void Update()
    {
        for (var idx = 0; idx < platform.transform.childCount; idx++)
        {
            var passengerGameObject = platform.transform.GetChild(idx).gameObject;
            var position = (Vector2) movingSpots[idx];

            passengerGameObject.transform.position = Vector2.MoveTowards(passengerGameObject.transform.position, position, speed * Time.deltaTime);

            if (Vector2.Distance(passengerGameObject.transform.position, position) < 0.2f)
            {
                if (waitTime <= 0)
                {
                    movingSpots[idx] = RandomPosition();
                    waitTime = startWaitTime;
                }
                else
                {
                    waitTime -= Time.deltaTime;
                }
            }
        }
        

    }
}