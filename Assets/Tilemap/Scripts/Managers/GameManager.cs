using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StatsModel
{
    public int coins;
    public int experience;

    public void PassengerMoved(int number)
    {
        coins += number;
    }

    public void NewPassengerOnBoard()
    {
        coins += 1;
    }

    public void ExperienceGained()
    {
        experience = +1;
    }
}

public struct Seat
{
    public bool isFree;
    public Vector3 position;

    public Seat(bool isFree, Vector3 position)
    {
        this.isFree = isFree;
        this.position = position;
    }
}

public class GameManager
{
    public static GameManager manager = new GameManager();

    public StatsModel stats = new StatsModel();
    public ArrayList stationList;
    public ArrayList coveredStationList;

    public Dictionary<string, Seat> seatsAvailable = new Dictionary<string, Seat>();

    public Station currentStation;
    public Station boundStation;
    private bool areStationsSet = false;

    public GameManager SetResourcesObject(ResourceManager resourceManager)
    {
        if(!areStationsSet)
        {
            var stations = resourceManager.knownImages.GetStationConfigurations();
            currentStation = stations.Item1;
            boundStation = stations.Item2;
            coveredStationList = stations.Item3;
            stationList = new ArrayList(resourceManager.knownImages.GetStations());
            areStationsSet = true;
        }

        return this;
    }

    public int GetScoreForBoardingWithStation(Station selectedStation)
    {
        var selectedStationIndex = stationList.IndexOf(selectedStation);
        var currentStationIndex = stationList.IndexOf(currentStation);
        var boundStationIndex = stationList.IndexOf(boundStation);

        if (currentStationIndex < boundStationIndex)
        {
            if (selectedStationIndex < boundStationIndex && selectedStationIndex > currentStationIndex)
            {
                //Debug.Log("Boarding correct D -> I");
                return 1;
            }
        }
        else
        {
            if (selectedStationIndex < currentStationIndex && selectedStationIndex > boundStationIndex)
            {
                //Debug.Log("Boarding correct D <- I");
                return 1;
            }
        }

        //Debug.Log("Other case");
        return selectedStation.identifier == boundStation.identifier ? 2 : -2;
    }

    public int GetScoreForOffboardingAtStation(Station selectedStation)
    {
        var selectedStationIndex = stationList.IndexOf(selectedStation);
        var currentStationIndex = stationList.IndexOf(currentStation);
        var boundStationIndex = stationList.IndexOf(boundStation);

        if(currentStationIndex < boundStationIndex) {
            if (selectedStationIndex < currentStationIndex)
            {
                //Debug.Log("Offboarding correct D <- I");
                return 1;
            }
        } else if(currentStationIndex > boundStationIndex) {
            if (selectedStationIndex > currentStationIndex)
            {
                //Debug.Log("Offboarding correct D <- I");
                return 1;
            }
        }

        return selectedStation.identifier == currentStation.identifier ? 2 : -2;
    }

    public void RegisterPassengerAtSeat(string name, Vector3 location)
    {
        seatsAvailable.Add(name, new Seat(false, location));
    }

    public void ClearPassengerSeats()
    {
        seatsAvailable.Clear();
    }

    public void FreeSeatWithID(string name)
    {
        var seat = new Seat(true, seatsAvailable[name].position);
        seatsAvailable[name] = seat;
        Debug.Log("Freed seat: " + name);
    }

    public bool HasAFreeSeat()
    {
        foreach (var key in seatsAvailable.Keys)
        {
            if (seatsAvailable[key].isFree)
            {
                return true;
            }
        }

        return false;
    }

    public Seat? GetNextFreeSeat()
    {
        Seat? seat = null;
        foreach(var key in seatsAvailable.Keys)
        {
            if (seatsAvailable[key].isFree)
            {
                seat = seatsAvailable[key];
                seatsAvailable[key] = new Seat(false, seat.GetValueOrDefault().position);
                break;
            }
        }
        return seat;
    }
}
