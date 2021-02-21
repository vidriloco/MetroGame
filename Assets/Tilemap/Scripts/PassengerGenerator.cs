using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerGenerator : MonoBehaviour
{
    private static PassengerGenerator _instance;

    public static PassengerGenerator Instance { get { return _instance; } }

    [SerializeField] public VisualPassenger visualPassenger;
    [SerializeField] public VisualStation visualStation;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void GenerateWith(Vector3 position, Passenger passengerData, GameObject parent, Area area, Station? stationData)
    {
        var passenger = VisualPassenger.SpawnWith(visualPassenger, parent, position, passengerData, area);

        if (stationData != null && Random.Range(1, 10) % 2 == 1)
        {
            VisualStation.SpawnWith(visualStation, passenger.gameObject, position, stationData.GetValueOrDefault(), area, new Vector3(0, 3));
        }
    }
}
