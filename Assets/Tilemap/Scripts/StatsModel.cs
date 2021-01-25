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

    public static void NewPassengerDelivered()
    {
        shared.coins += 2;
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
