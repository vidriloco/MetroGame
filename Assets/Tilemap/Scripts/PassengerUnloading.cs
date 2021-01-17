﻿using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;

public class PassengerUnloading: MonoBehaviour
{
    private GameObject originalPassenger = null;
    private GameObject draggedPassenger = null;

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);

            if (hit.collider.gameObject == null) { return; }

            var spriteCombo = ShouldFreePassengerGivenSprite(hit.collider.gameObject);
            if (spriteCombo.Item1)
            {
                originalPassenger = spriteCombo.Item2;
                LeanTween.alpha(originalPassenger, 0.2f, 1);
                draggedPassenger = Instantiate<GameObject>(spriteCombo.Item2);
            }
        }

        if (Input.GetMouseButton(0))
        {
            if(draggedPassenger == null) { return; }

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - draggedPassenger.transform.position;
            draggedPassenger.transform.Translate(mousePosition);
        }


        if(Input.GetMouseButtonUp(0))
        {
            if(draggedPassenger == null) { return; }

            var platformCollider = GameObject.FindGameObjectWithTag(Tags.Passenger).GetComponent<BoxCollider2D>();
            var passengerCollider = draggedPassenger.GetComponent<BoxCollider2D>();

            if(draggedPassenger.tag != Tags.PassengerTrapped && platformCollider.bounds.Intersects(passengerCollider.bounds))
            {
                passengerCollider.tag = Tags.PassengerInPlatform;
                OffboardChoosenPassenger();
            } else
            {
                LeanTween.moveLocal(draggedPassenger, originalPassenger.transform.position, 0.5f).setDestroyOnComplete(true).setOnComplete(() => {
                    LeanTween.alpha(originalPassenger, 1f, 1);
                    draggedPassenger = null;
                    originalPassenger = null;
                });
            }
        }
    }

    private void OffboardChoosenPassenger()
    {
        GameObject.FindObjectOfType<SoundManager>().PlayRandomHumanSound();

        if(draggedPassenger != null)
        {
            LeanTween.scale(draggedPassenger, new Vector3(5f, 5f, 5f), 1).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.alpha(draggedPassenger, 0f, 1).setDestroyOnComplete(true);
            draggedPassenger = null;
        }

        if (originalPassenger != null)
        {
            LeanTween.alpha(originalPassenger, 0f, 1).setDestroyOnComplete(true);
            originalPassenger = null;
        }
    }

    private (bool, GameObject) ShouldFreePassengerGivenSprite(GameObject sprite)
    {
        
        if (sprite.tag == "dead-station")
        {
            if (sprite.transform.parent.gameObject.tag == "passenger")
            {
                return (true, sprite.transform.parent.gameObject);
            }
        }
        else if (sprite.tag == "passenger")
        {
            var freeSprite = sprite.transform.childCount != 0 && sprite.transform.GetChild(0).tag == "dead-station";
            return (freeSprite, sprite);
        }

        return (false, sprite);
    }
}

//private LineRenderer lineRenderer;
//private PolygonCollider2D polygonCollider;

// Uncomment this block in order to select multiple passengers within an enclosing group
/*void Update()
{
    if (Input.GetMouseButtonDown(0))
    {

        lineRenderer.sortingOrder = 12;
        lineRenderer.positionCount = 0;
    }

    if (Input.GetMouseButton(0))
    {
        FreeDraw();
    }

    if(Input.GetMouseButtonUp(0))
    {
        Vector2[] positions = new Vector2[lineRenderer.positionCount];

        for(var index = 0; index < lineRenderer.positionCount; index++)
        {
            positions[index] = lineRenderer.GetPosition(index);
        }

        polygonCollider = gameObject.GetComponent<PolygonCollider2D>();
        polygonCollider.SetPath(0, positions);

        //gameObject.transform.position = polygonCollider.bounds.center;
    }
}

void FreeDraw()
{

    lineRenderer.startWidth = 0.1f;
    lineRenderer.endWidth = 0.1f;
    Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
    lineRenderer.positionCount++;
    lineRenderer.SetPosition(lineRenderer.positionCount - 1, Camera.main.ScreenToWorldPoint(mousePos));

}

*/