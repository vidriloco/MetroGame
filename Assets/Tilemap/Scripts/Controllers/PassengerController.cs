using System.Collections;
using System.Collections.Generic;
using CodeMonkey;
using CodeMonkey.Utils;
using UnityEngine;

public class PassengerController: MonoBehaviour
{
    private GameObject originalPassenger = null;
    private GameObject draggedPassenger = null;

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
            var metroCar = metroController.metro;
            if(metroCar != null)
            {
                return metroCar.GetComponent<BoxCollider>().bounds;
            } else
            {
                return null;
            }
        }
    }

    private bool DroppingOverPlatformBounds => PlatformBounds.Intersects(draggedPassenger.GetComponent<BoxCollider2D>().bounds);
    private bool DraggingPlatformPassenger => draggedPassenger.CompareTag(Tags.PassengerInPlatform);

    private bool DroppingOverMetroBounds
    {
        get
        {
            if(MetroBounds != null)
            {
                return (bool)(MetroBounds?.Intersects(draggedPassenger.GetComponent<BoxCollider2D>().bounds));
            } else
            {
                return false;
            }
        }
    }
    private bool DraggingMetroPassenger => draggedPassenger.CompareTag(Tags.Passenger);

    private ArrayList freedPassengerSeats = new ArrayList();

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
        
        LeanTween.moveLocal(draggedPassenger, originalPassenger.transform.position, 0.5f).setOnComplete(() => {
            if(originalPassenger != null)
            {
                LeanTween.alpha(originalPassenger, 1f, 1);
                originalPassenger = null;
            }

            draggedPassenger = null;
        });
    }

    private void ReleasePassenger()
    {
        if (draggedPassenger == null || originalPassenger == null) { return; }

        originalPassenger.transform.position = draggedPassenger.transform.position;
        LeanTween.alpha(originalPassenger, 1, 1);
        GameObject.DestroyImmediate(draggedPassenger);
        draggedPassenger = null;
        originalPassenger = null;
    }

    private void OnPassengerDropped()
    {

        if (draggedPassenger == null) { return; }

        var passengerCollider = draggedPassenger.GetComponent<BoxCollider2D>();

        // Leave out passengers (off)boarding out of time
        if(draggedPassenger.CompareTag(Tags.PassengerTrapped))
        {
            ReturnPassenger();
            return;
        }

        if (DraggingPlatformPassenger)
        {
            if (DroppingOverMetroBounds)
            {
                // Allow passengers to be dropped inside when doors are still open and spaces free
                if (metroStatus == VehicleStatus.OpenDoors && freedPassengerSeats.Count > 0)
                {
                    passengerCollider.tag = Tags.Passenger;
                    InboardChoosenPassenger();
                } else
                {
                    ReturnPassenger();
                }
                
            } else if(DroppingOverPlatformBounds)
            {
                ReleasePassenger();
            } else
            {
                ReturnPassenger();
            }

        }
        else if(DraggingMetroPassenger)
        {
            if (DroppingOverPlatformBounds)
            {
                if (metroStatus == VehicleStatus.OpenDoors)
                {
                    passengerCollider.tag = Tags.Passenger;
                    DismissOffboardedPassenger();
                } else
                {
                    ReturnPassenger();
                }
                    
            }
            else
            {
                ReturnPassenger();
            }
        }
    }

    private void DismissOffboardedPassenger()
    {
        GameObject.FindObjectOfType<SoundManager>().PlayRandomHumanSound();

        if (originalPassenger != null)
        {
            freedPassengerSeats.Add(originalPassenger.transform.position);
            LeanTween.alpha(originalPassenger, 0f, 0.3f).setDestroyOnComplete(true);
            originalPassenger = null;
        }

        if (draggedPassenger != null)
        {
            if (draggedPassenger.transform.childCount > 0)
            {
                GameObject.DestroyImmediate(draggedPassenger.transform.GetChild(0).gameObject);
            }

            draggedPassenger.tag = Tags.DiscardObject;
            var randomY = Random.Range(PlatformBounds.min.y, PlatformBounds.max.y);
            LeanTween.move(draggedPassenger, new Vector3(PlatformBounds.max.x, randomY), 1.5f).setDestroyOnComplete(true);

            var score = GameManager.manager.SetResourcesObject(resourceManager).GetScoreForOffboardingAtStation(selectedPassengerStation);
            uiController.UpdateCoinStats(score, draggedPassenger.transform.position);
        }
    }

    private void InboardChoosenPassenger()
    {
        if (draggedPassenger != null)
        {
            var nextPosition = (Vector3)freedPassengerSeats[0];
            freedPassengerSeats.RemoveAt(0);

            LeanTween.moveLocal(draggedPassenger, nextPosition, 0.5f).setOnComplete(() =>
            {
                draggedPassenger.GetComponent<SpriteRenderer>().sortingOrder = 1;
                draggedPassenger.transform.SetParent(metroController.metro.transform);
                draggedPassenger.tag = Tags.Passenger;

                if (draggedPassenger.transform.childCount > 0)
                {
                    GameObject.DestroyImmediate(draggedPassenger.transform.GetChild(0).gameObject);
                }

                var score = GameManager.manager.SetResourcesObject(resourceManager).GetScoreForBoardingWithStation(selectedPassengerStation);
                uiController.UpdateCoinStats(score, draggedPassenger.transform.position);

                GameObject.FindObjectOfType<SoundManager>().PlayRandomHumanSound();

                draggedPassenger = null;
            });
        }

        if (originalPassenger != null)
        {
            LeanTween.alpha(originalPassenger, 0f, 1).setDestroyOnComplete(true);
        }
    }

    private void OnPassengerDragged()
    {
        if (draggedPassenger == null) { return; }

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - draggedPassenger.transform.position;
        draggedPassenger.transform.Translate(mousePosition);
    }

    private void OnPassengerSelected()
    {

        Vector2 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);

        if (hit.collider == null) { return; }

        UnpackSelectedPassengerStation(hit.collider.gameObject);

        var spriteCombo = FetchPassengerSprite(hit.collider.gameObject);
        if (spriteCombo != null)
        {
            originalPassenger = spriteCombo;
            LeanTween.alpha(originalPassenger, 0, 0.2f);
            draggedPassenger = Instantiate<GameObject>(spriteCombo);
        }
        
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