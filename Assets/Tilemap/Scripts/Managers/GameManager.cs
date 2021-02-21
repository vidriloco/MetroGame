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

public class GameManager
{
    public static GameManager manager = new GameManager();

    public StatsModel stats = new StatsModel();
    public ArrayList stationList;

    public Station currentStation;
    public Station boundStation;
    private bool areStationsSet = false;

    public GameManager SetResourcesObject(ResourceManager resourceManager)
    {
        if(!areStationsSet)
        {
            var stations = resourceManager.knownImages.GetStationConfigurations();
            manager.currentStation = stations.Item1;
            manager.boundStation = stations.Item2;
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

        Debug.Log("Other case");
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

}
