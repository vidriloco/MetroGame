using System.Collections;
using System.Collections.Generic;
using CodeMonkey;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;

public class PassengerBehaviour: MonoBehaviour
{
    private GameObject originalPassenger = null;
    private GameObject draggedPassenger = null;

    [SerializeField] private MetroBehaviour metroController;
    [SerializeField] private PlatformBehaviour platformController;

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

    private void Start()
    {
        metroController.MetroStatusChanged += MetroController_MetroStatusChanged;
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
        LeanTween.moveLocal(draggedPassenger, originalPassenger.transform.position, 0.5f).setDestroyOnComplete(true).setOnComplete(() => {
            if(originalPassenger != null)
            {
                LeanTween.alpha(originalPassenger, 1f, 1);
                originalPassenger = null;
            }

            draggedPassenger = null;
        });
    }

    private void FreePassenger()
    {
        originalPassenger.transform.position = draggedPassenger.transform.position;
        LeanTween.alpha(originalPassenger, 1, 1);
        GameObject.DestroyImmediate(draggedPassenger);
        draggedPassenger = null;
    }

    private void OnPassengerDropped()
    {
        if (draggedPassenger == null) { return; }

        var passengerCollider = draggedPassenger.GetComponent<BoxCollider2D>();
        
        if(draggedPassenger.CompareTag(Tags.PassengerTrapped))
        {
            ReturnPassenger();
            return;
        }

        if(DraggingPlatformPassenger)
        {
            if(DroppingOverMetroBounds)
            {
                if (metroStatus == VehicleStatus.OpenDoors)
                {
                    passengerCollider.tag = Tags.Passenger;
                    OffboardChoosenPassenger();
                } else
                {
                    ReturnPassenger();
                }
                
            } else if(DroppingOverPlatformBounds)
            {
                FreePassenger();
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
            LeanTween.alpha(originalPassenger, 0f, 0.3f).setDestroyOnComplete(true);
            originalPassenger = null;
        }

        if (draggedPassenger != null)
        {
            draggedPassenger.tag = Tags.DiscardObject;
            var randomY = Random.Range(PlatformBounds.min.y, PlatformBounds.max.y);
            LeanTween.move(draggedPassenger, new Vector3(PlatformBounds.max.x, randomY), 1.5f).setDestroyOnComplete(true);
        }

        UpdateCoinStats();
    }

    private void OffboardChoosenPassenger()
    {
        GameObject.FindObjectOfType<SoundManager>().PlayRandomHumanSound();

        if (draggedPassenger != null)
        {
            LeanTween.scale(draggedPassenger, new Vector3(5f, 5f, 5f), 1).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.alpha(draggedPassenger, 0f, 1).setDestroyOnComplete(true);
        }

        if (originalPassenger != null)
        {
            LeanTween.alpha(originalPassenger, 0f, 1).setDestroyOnComplete(true);
        }

        draggedPassenger = null;

        UpdateCoinStats();
    }

    private void UpdateCoinStats()
    {
        StatsManager.NewPassengerDelivered();
        Text coinsCounterText = GameObject.FindGameObjectWithTag(Tags.CoinsCounter).GetComponent<Text>();
        var coins = StatsManager.shared.coins.ToString();
        coinsCounterText.text = coins;
        CMDebug.TextPopupMouse("+ 2");

        var coinIcon = GameObject.FindGameObjectWithTag(Tags.CoinsIcon);

        LeanTween.scale(coinIcon, new Vector3(1.2f, 1.2f, 1.2f), 0.5f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            LeanTween.scale(coinIcon, new Vector3(0.8f, 0.8f, 0.8f), 0.2f);
        });
        LeanTween.rotateAroundLocal(coinIcon, new Vector3(0, 1, 0), 360, 0.5f);
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
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, -Vector2.up);

        if (hit.collider == null) { return; }

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
            Debug.Log("Fetch passenger");
            return sprite;
        }

        Debug.Log("NULL: " + sprite);
        return null;
    }
}