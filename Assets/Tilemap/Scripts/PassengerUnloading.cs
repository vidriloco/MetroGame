using System.Collections;
using System.Collections.Generic;
using CodeMonkey;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;

public class PassengerUnloading: MonoBehaviour
{
    private GameObject originalPassenger = null;
    private GameObject draggedPassenger = null;

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

    private void OnPassengerDropped()
    {
        if (draggedPassenger == null) { return; }

        var platformCollider = GameObject.FindGameObjectWithTag(Tags.Platform).GetComponent<BoxCollider2D>();
        var passengerCollider = draggedPassenger.GetComponent<BoxCollider2D>();

        if (draggedPassenger.tag != Tags.PassengerTrapped && platformCollider.bounds.Intersects(passengerCollider.bounds))
        {
            passengerCollider.tag = Tags.PassengerInPlatform;
            
            OffboardChoosenPassenger();
        }
        else
        {
            LeanTween.moveLocal(draggedPassenger, originalPassenger.transform.position, 0.5f).setDestroyOnComplete(true).setOnComplete(() => {
                LeanTween.alpha(originalPassenger, 1f, 1);
                draggedPassenger = null;
                originalPassenger = null;
            });
        }
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

    private void FillSeatOfGonePassenger()
    {
        LeanTween.delayedCall(Random.RandomRange(1f, 1.8f), () =>
        {
            if (originalPassenger != null)
            {
                var passengerObject = GameObject.FindGameObjectWithTag(Tags.VisualPassenger).GetComponent<VisualPassenger>();

                Vector3 position = originalPassenger.transform.position;
                var visualPassenger = GameObject.Instantiate(passengerObject, position, Quaternion.Euler(0, 0, 0));
                visualPassenger.SetParentAndPosition(originalPassenger.transform.parent.transform, position);
                if(visualPassenger.stationGameObject != null)
                {
                    visualPassenger.stationGameObject.tag = Tags.StationSymbolMarkedToDrop;
                }
                GameObject.DestroyImmediate(originalPassenger);
            }

        });

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

        if (hit.collider.gameObject == null) { return; }

        var spriteCombo = FetchPassengerSprite(hit.collider.gameObject);
        if (spriteCombo.Item1)
        {
            originalPassenger = spriteCombo.Item2;
            LeanTween.alpha(originalPassenger, 0.2f, 1);
            draggedPassenger = Instantiate<GameObject>(spriteCombo.Item2);
        }
    }

    private (bool, GameObject) FetchPassengerSprite(GameObject sprite)
    {
        
        if (sprite.tag == Tags.StationSymbolMarkedToDrop)
        {
            if (sprite.transform.parent.gameObject.tag == Tags.Passenger)
            {
                return (true, sprite.transform.parent.gameObject);
            }
        }
        else if (sprite.tag == Tags.Passenger && sprite.transform.childCount > 0)
        {
            var stationSymbolSprite = sprite.transform.GetChild(0);
            var freeSprite = sprite.transform.childCount != 0 && stationSymbolSprite.tag == Tags.StationSymbolMarkedToDrop;
            return (freeSprite, sprite);
        }

        return (false, sprite);
    }
}