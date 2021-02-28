using System.Collections;
using System.Collections.Generic;
using CodeMonkey;
using CodeMonkey.Utils;
using UnityEngine;

public class PassengerController: MonoBehaviour
{
    private GameObject selectedPassenger = null;
    private Vector3 originalPosition;

    private Station selectedPassengerStation;

    [SerializeField] private MetroController metroController;
    [SerializeField] private PlatformController platformController;
    [SerializeField] private UIController uiController;

    private VehicleStatus metroStatus;

    private Bounds PlatformBounds => platformController.GetTilemap().localBounds;
    private Bounds? MetroBounds
    {
        get
        {
            var metroCar = metroController.Metro;
            if(metroCar != null)
            {
                return metroCar.GetComponent<BoxCollider>().bounds;
            } else
            {
                return null;
            }
        }
    }

    private bool DroppingOverPlatformBounds => PlatformBounds.Intersects(selectedPassenger.GetComponent<BoxCollider2D>().bounds);
    private bool DraggingPlatformPassenger => selectedPassenger.CompareTag(Tags.PassengerInPlatform);
    private bool PassengersCanGoInAndOutOfTrain => metroStatus == VehicleStatus.OpenDoors || metroStatus == VehicleStatus.NotifyCloseDoors;

    private bool DroppingOverMetroBounds
    {
        get
        {
            if(MetroBounds != null)
            {
                return (bool)(MetroBounds?.Intersects(selectedPassenger.GetComponent<BoxCollider2D>().bounds));
            } else
            {
                return false;
            }
        }
    }
    private bool DraggingMetroPassenger => selectedPassenger.CompareTag(Tags.Passenger);

    private ResourceManager resourceManager;

    private void Start()
    {
        metroController.MetroStatusChanged += MetroController_MetroStatusChanged;
        resourceManager = GameObject.FindObjectOfType<ResourceManager>();
    }

    private void MetroController_MetroStatusChanged(VehicleStatus status)
    {
        metroStatus = status;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnPassengerSelected();
        }

        if (Input.GetMouseButton(0))
        {
            OnPassengerDragged();
        }

        // Dropping a passenger
        if(Input.GetMouseButtonUp(0))
        {
            OnPassengerDropped();
        }
    }

    private void ReturnPassenger()
    {
        LeanTween.move(selectedPassenger, originalPosition, 1f).setOnComplete(() => {
            NullifyPassengerSelection();
        });
    }

    private void NullifyPassengerSelection()
    {
        selectedPassenger = null;
    }

    private void OnPassengerDropped()
    {
        if (selectedPassenger == null) { return; }

        var passengerCollider = selectedPassenger.GetComponent<BoxCollider2D>();

        if (DraggingPlatformPassenger)
        {
            if (DroppingOverMetroBounds)
            {

                // Allow passengers to be dropped inside when doors are still open and spaces free
                if (PassengersCanGoInAndOutOfTrain && GameManager.manager.HasAFreeSeat())
                {
                    passengerCollider.tag = Tags.Passenger;
                    InboardChoosenPassenger();
                    return;
                }
                
            } else if(DroppingOverPlatformBounds)
            {
                NullifyPassengerSelection();
                return;
            }
            ReturnPassenger();
        }
        else if(DraggingMetroPassenger)
        {
            if (DroppingOverPlatformBounds)
            {
                if (PassengersCanGoInAndOutOfTrain)
                {
                    passengerCollider.tag = Tags.Passenger;
                    DismissOffboardedPassenger();
                    return;
                } 
            }

            ReturnPassenger();
        }
    }

    private void DismissOffboardedPassenger()
    {
        if (selectedPassenger == null) { return; }

        if (selectedPassenger.transform.childCount > 0)
        {
            GameObject.DestroyImmediate(selectedPassenger.transform.GetChild(0).gameObject);
        }

        GameManager.manager.FreeSeatWithID(selectedPassenger.name);
        
        selectedPassenger.tag = Tags.DiscardObject;
        var randomY = Random.Range(PlatformBounds.min.y, PlatformBounds.max.y);
        LeanTween.move(selectedPassenger, new Vector3(PlatformBounds.max.x, randomY), 1.5f).setDestroyOnComplete(true);

        var score = GameManager.manager.SetResourcesObject(resourceManager).GetScoreForOffboardingAtStation(selectedPassengerStation);
        uiController.UpdateCoinStats(score, selectedPassenger.transform.position);
        GameObject.FindObjectOfType<SoundManager>().PlayRandomHumanSound();
    }

    private void InboardChoosenPassenger()
    {
        if (selectedPassenger == null) { return; }

        var seat = GameManager.manager.GetNextFreeSeat().GetValueOrDefault();
        var relativePosition = metroController.Metro.transform.GetChild(0).TransformPoint(seat.position);

        LeanTween.move(selectedPassenger, relativePosition, 0.5f).setOnComplete(() =>
        {
            selectedPassenger.GetComponent<SpriteRenderer>().sortingOrder = 1;
            selectedPassenger.tag = Tags.Passenger;
            selectedPassenger.transform.SetParent(metroController.Metro.transform.GetChild(0).transform);
            
            if (selectedPassenger.transform.childCount > 0)
            {
                GameObject.DestroyImmediate(selectedPassenger.transform.GetChild(0).gameObject);
            }

            var score = GameManager.manager.SetResourcesObject(resourceManager).GetScoreForBoardingWithStation(selectedPassengerStation);
            uiController.UpdateCoinStats(score, selectedPassenger.transform.position);

            GameObject.FindObjectOfType<SoundManager>().PlayRandomHumanSound();

            selectedPassenger = null;
        });
    }

    private void OnPassengerDragged()
    {
        if (selectedPassenger == null) { return; }

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - selectedPassenger.transform.position;
        selectedPassenger.transform.Translate(mousePosition);
    }

    private void OnPassengerSelected()
    {

        Vector2 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);

        if (hit.collider == null) { return; }

        UnpackSelectedPassengerStation(hit.collider.gameObject);

        selectedPassenger = FetchPassengerSprite(hit.collider.gameObject);
        originalPosition = selectedPassenger.transform.position;
    }

    private GameObject FetchPassengerSprite(GameObject sprite)
    {
        if (sprite.CompareTag(Tags.Station) || sprite.CompareTag(Tags.StationInPlatform))
        {
            var parentSprite = sprite.transform.parent.gameObject;
            return FetchPassengerSprite(parentSprite);
        }
        else if (sprite.CompareTag(Tags.Passenger) || sprite.CompareTag(Tags.PassengerInPlatform))
        {
            return sprite;
        }

        return null;
    }

    private void UnpackSelectedPassengerStation(GameObject gameObject)
    {
        var visualPassenger = gameObject.GetComponent<VisualPassenger>();
        if (visualPassenger != null)
        {
            var visualStation = visualPassenger.GetComponentInChildren<VisualStation>();

            if(visualStation != null)
            {
                selectedPassengerStation = visualStation.GetStation();
            }
        }

    }
}