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

        public Station[] GetStations()
        {
            return stations;
        }

        public Station GetStationAt(int index)
        {
            return stations[index];
        }

        public Passenger GetRandomPassenger()
        {
            var passengersCount = passengers.Length;
            var randomNumber =  Random.Range(0, passengersCount);
            var passenger = new Passenger();
            passenger.image = passengers[randomNumber];
            return passenger;
        }

        public Station GetRandomStation()
        {
            var stationsCount = stations.Length;
            var originStationIndex = Random.Range(0, stationsCount);
            return stations[originStationIndex];
        }

        public (Station, Station) GetStationConfigurations()
        {
            var stationsCount = stations.Length;
            var destinationStationIndex = Random.Range(0, 1) == 1 ? 0 : stationsCount-1;

            var positionThreshold = Random.Range(0, 4);
            var mediumPoint = stationsCount / 2;

            var originStationIndex = Random.Range(mediumPoint - positionThreshold, mediumPoint + positionThreshold);
            return (stations[originStationIndex], stations[destinationStationIndex]);
        }
    }
}
