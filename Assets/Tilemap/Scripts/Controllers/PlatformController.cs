using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformController : MonoBehaviour
{

    [SerializeField] public MetroController metroController;

    private BoxCollider2D platformBoxCollider;

    private UnityEngine.Tilemaps.Tilemap tilemap;
    private ArrayList movingSpots = new ArrayList();
    private GameObject platform;

    private readonly float speed = 2f;
    private float waitTime = 2f;
    private readonly float startWaitTime;
    private readonly float offset = 10;

    private ResourceManager resourceManager;

    private void Start()
    {
        resourceManager = GameObject.FindObjectOfType<ResourceManager>();

        tilemap = GetComponentInChildren<UnityEngine.Tilemaps.Tilemap>();
        platform = GameObject.FindGameObjectWithTag(Tags.Platform);

        platformBoxCollider = GameObject.FindGameObjectWithTag("platform-area").GetComponentInChildren<BoxCollider2D>();

        GameManager.manager.SetResourcesObject(resourceManager);

        SpawnPassengers();
        StartCoroutine(ReSpawnPassengers());

        metroController.MetroStatusChanged += MetroController_MetroStatusChanged;

    }

    private void DestroyRandomPassengers()
    {
        for(var idx = 0; idx < Random.Range(0,5); idx++)
        {
            var passenger = GameObject.FindGameObjectWithTag("passenger-in-platform");
            LeanTween.alpha(passenger, 0, 1).setDestroyOnComplete(true);
        }
        
    }

    IEnumerator ReSpawnPassengers()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(15, 30));
        SpawnPassengers();
        StartCoroutine(ReSpawnPassengers());
    }

    private void SpawnPassengers()
    {

        var passengersCount = UnityEngine.Random.Range(0, 5);
        for (int idx = 0; idx < passengersCount; idx++)
        {
            var position = RandomPosition();

            var passengerData = resourceManager.knownImages.GetRandomPassenger();
            var stationData = resourceManager.knownImages.GetRandomStation();

            PassengerGenerator.Instance.GenerateWith(position, passengerData, platform, Area.Platform, stationData);

            movingSpots.Add(position);
        }
    }

    private void MetroController_MetroStatusChanged(VehicleStatus status)
    {
        switch(status)
        {
            case VehicleStatus.CloseDoors:
                MarkPassengersWithColor(Color.red);
                break;
            case VehicleStatus.WillDepart:
                MarkPassengersWithColor(Color.white);
                DestroyRandomPassengers();
                break;
            case VehicleStatus.OpenDoors:
                SpawnPassengers();
                break;
        }
    }

    private void MarkPassengersWithColor(Color color)
    {
        for (var idx = 0; idx < platform.transform.childCount; idx++)
        {
            var passengerGameObject = platform.transform.GetChild(idx).gameObject;
            var spriteRenderer = passengerGameObject.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
        }
    }

    public UnityEngine.Tilemaps.Tilemap GetTilemap()
    {
        return tilemap;
    }

    private Vector2 RandomPosition()
    {
        var xPosition = Random.Range(platformBoxCollider.bounds.min.x + offset, platformBoxCollider.bounds.max.x - offset);
        var yPosition = Random.Range(platformBoxCollider.bounds.min.y + offset, platformBoxCollider.bounds.max.y - offset);
        Debug.Log(xPosition + " : " + yPosition);
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