using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{

    [SerializeField] public Images knownImages;

    [System.Serializable]
    public struct Images
    {
        [SerializeField] public GameObject frontCar;
        [SerializeField] public GameObject closedCar;
        [SerializeField] public GameObject carCover;
        [SerializeField] public GameObject boundText;
        [SerializeField] public GameObject stationNameText;

        [SerializeField] private Sprite[] passengers;
        [SerializeField] private Station[] stations;
        [SerializeField] private Sprite[] selectedStations;


        public Station GetStationAt(int index)
        {
            return stations[index];
        }

        public Passenger randomPassenger()
        {
            var passengersCount = passengers.Length;
            var randomNumber =  Random.RandomRange(0, passengersCount);
            var passenger = new Passenger();
            passenger.image = passengers[randomNumber];
            return passenger;
        }

        public Station randomStation()
        {
            var stationsCount = stations.Length;
            var randomNumber = Random.RandomRange(0, stationsCount);
            return stations[randomNumber];
        }
    }
}
