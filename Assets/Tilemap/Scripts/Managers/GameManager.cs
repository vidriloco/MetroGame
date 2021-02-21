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

    public Station currentStation;
    public Station boundStation;
    private bool areStationsSet = false;

    public GameManager SetResourcesObject(ResourceManager resourceManager)
    {
        var stations = resourceManager.knownImages.GetStationConfigurations();

        if(!areStationsSet)
        {
            manager.currentStation = stations.Item1;
            manager.boundStation = stations.Item2;
            areStationsSet = true;
        }

        return this;
    }

}
