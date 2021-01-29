using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{

    [SerializeField] public MetroBehaviour metroBehaviour;
    private BoxCollider2D collider;
    private ArrayList movingSpots = new ArrayList();
    private GameObject platform;

    private void Start()
    {
        collider = GetComponentInChildren<BoxCollider2D>();
        platform = GameObject.FindGameObjectWithTag(Tags.Platform);

        for (int idx = 0; idx < 10; idx++)
        {
            var xPosition = Random.RandomRange(collider.bounds.min.x, collider.bounds.max.x);
            var yPosition = Random.RandomRange(collider.bounds.min.y, collider.bounds.max.y);
            Debug.Log(xPosition + " - " + yPosition);

            var position = new Vector2(xPosition, yPosition);
            var visualPassengerClone = (VisualPassenger)GameObject.Instantiate(metroBehaviour.visualPassenger, position, Quaternion.Euler(0, 0, 0));
            visualPassengerClone.ConfigureAsPassengerInTrain(false);
            visualPassengerClone.SetParentAndPosition(platform.transform, position);

            movingSpots.Add(position);
        }
        
    }

    public float speed = 1f;
    private float waitTime = 2f;
    public float startWaitTime;

    private Vector2 randomPosition()
    {
        var xPosition = UnityEngine.Random.Range(collider.bounds.min.x, collider.bounds.max.x);
        var yPosition = UnityEngine.Random.Range(collider.bounds.min.y, collider.bounds.max.y);
        return new Vector2(xPosition, yPosition);
    }


    private void Update()
    {
        for (var idx = 0; idx < platform.transform.childCount; idx++)
        {
            var passengerGameObject = platform.transform.GetChild(idx).gameObject;
            var position = (Vector2) movingSpots[idx];
            passengerGameObject.transform.position = Vector2.MoveTowards(passengerGameObject.transform.position, position, speed * Time.deltaTime);
            Debug.Log(passengerGameObject);

            //LeanTween.move(passengerGameObject, moveSpot.position, speed);
            Debug.Log("Walking");
            if (Vector2.Distance(passengerGameObject.transform.position, position) < 0.2f)
            {
                if (waitTime <= 0)
                {
                    movingSpots[idx] = randomPosition();
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