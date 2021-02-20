using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformController : MonoBehaviour
{

    [SerializeField] public MetroController metroController;

    private UnityEngine.Tilemaps.Tilemap tilemap;
    private ArrayList movingSpots = new ArrayList();
    private GameObject platform;

    private readonly float speed = 2f;
    private float waitTime = 2f;
    private readonly float startWaitTime;
    private readonly float offset = 10;

    private Station currentStation;
    private ResourceManager resourceManager;

    private void Start()
    {
        resourceManager = GameObject.FindObjectOfType<ResourceManager>();

        tilemap = GetComponentInChildren<UnityEngine.Tilemaps.Tilemap>();
        platform = GameObject.FindGameObjectWithTag(Tags.Platform);
        currentStation = resourceManager.knownImages.randomStation();

        // TODO: Move out the import UI
        var stationImage = GameObject.FindGameObjectWithTag(Tags.CurrentStationIcon).GetComponent<Image>();
        stationImage.sprite = currentStation.bigIcon;
        var stationNameLabel = GameObject.FindGameObjectWithTag(Tags.CurrentStationNameLabel).GetComponent<TMPro.TextMeshProUGUI>();
        stationNameLabel.text = currentStation.name;

        SpawnPassengers();
        StartCoroutine(ReSpawnPassengers());

        metroController.MetroStatusChanged += MetroController_MetroStatusChanged;

    }

    IEnumerator ReSpawnPassengers()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(5, 15));
        SpawnPassengers();
        StartCoroutine(ReSpawnPassengers());
    }

    public Station CurrentStation()
    {
        return currentStation;
    }

    private void SpawnPassengers()
    {

        var passengersCount = UnityEngine.Random.Range(0, 5);
        for (int idx = 0; idx < passengersCount; idx++)
        {
            var position = RandomPosition();

            var passengerData = resourceManager.knownImages.randomPassenger();
            var stationData = resourceManager.knownImages.randomStation();

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