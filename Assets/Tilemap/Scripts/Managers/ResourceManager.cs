using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{

    [SerializeField] public Images knownImages;

    [System.Serializable]
    public struct Images
    {
        [SerializeField] private Sprite[] passengers;
        [SerializeField] private Sprite[] stations;

        public Sprite randomPassenger()
        {
            var passengersCount = passengers.Length;
            var randomNumber =  Random.RandomRange(0, passengersCount);
            return passengers[randomNumber];
        }

        public Sprite randomStation()
        {
            var stationsCount = stations.Length;
            var randomNumber = Random.RandomRange(0, stationsCount);
            return stations[randomNumber];
        }
    }
}
