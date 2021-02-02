using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using System;

public delegate void MetroStatusChangedHandler(VehicleStatus status);

public class MetroController : MonoBehaviour {

    public GameObject metro
    {
        get {
            return GameObject.FindGameObjectWithTag(Tags.Metro);
        }
    }

    [SerializeField] public VisualPassenger visualPassenger;

    public event MetroStatusChangedHandler MetroStatusChanged;

    public TrajectoryController trajectoryController;

    private void Start() {
        trajectoryController.TrajectoryStatusChanged += TrajectoryController_TrajectoryStatusChanged;
    }

    private void TrajectoryController_TrajectoryStatusChanged(VehicleStatus status)
    {
        MetroStatusChanged?.Invoke(status);

        switch (status)
        {
            case VehicleStatus.CloseDoors:
                MetroWillCloseDoors();
                break;
            case VehicleStatus.OpenDoors:
                MetroWillOpenDoors();
                break;
            case VehicleStatus.WillDepart:
                MetroWillDepart();
                break;
            case VehicleStatus.IsArriving:
                MetroIsArriving();
                break;
            case VehicleStatus.Gone:
                MetroIsGone();
                break;
        }
    }

    private void MetroIsGone()
    {
        GameObject[] passengers = GameObject.FindGameObjectsWithTag(Tags.Passenger);
        foreach (var passenger in passengers)
        {
            GameObject.DestroyImmediate(passenger);
        }
    }

    private void MetroIsArriving()
    {
        GameObject.FindObjectOfType<SoundManager>().PlayMetroSoundArriving();
    }

    private void MarkPassengersWithColor(Color color)
    {
        GameObject[] passengers = GameObject.FindGameObjectsWithTag(Tags.Passenger);
        foreach (var passenger in passengers)
        {
            passenger.GetComponent<SpriteRenderer>().color = color;
        }
    }

    private void MetroWillCloseDoors()
    {
        GameObject.FindObjectOfType<SoundManager>().PlayMetroSoundClosingDoors();

        MarkPassengersWithColor(Color.red);

        GameObject cover = GameObject.FindGameObjectWithTag(Tags.MetroCover);
        LeanTween.delayedCall(cover, 1f, () =>
        {
            LeanTween.alpha(cover, 1f, 2f).setOnComplete(() => {
                cover.tag = Tags.DiscardObject;
            });
        }).setOnCompleteOnRepeat(false);
    }

    private void MetroWillOpenDoors()
    {
        GameObject cover = GameObject.FindGameObjectWithTag(Tags.MetroCover);

        if (cover != null)
        {
            LeanTween.alpha(cover, 0f, 1f).setEaseLinear();
        }
    }

    private void MetroWillDepart()
    {
        GameObject.FindObjectOfType<SoundManager>().PlayMetroSoundDeparting();
        MarkPassengersWithColor(Color.white);
    }
}
