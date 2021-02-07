using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StatsModel
{
    public int coins;
    public int experience;
}

public class StatsManager
{
    public static StatsModel shared = new StatsModel();

    public static void PassengerMoved(int number)
    {
        shared.coins += number;
    }

    public static void NewPassengerOnBoard()
    {
        shared.coins += 1;
    }

    public static void ExperienceGained()
    {
        shared.experience = +1;
    }
}
