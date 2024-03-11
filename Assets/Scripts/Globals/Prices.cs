using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public enum CargoType { Passengers, Mail, Logs, Lumber };

    public static class Prices
    {
        //prices are per unit
        public static Dictionary<CargoType, decimal> AsDict = new()
        {
            [CargoType.Passengers] = 10,
            [CargoType.Mail] = 5,
            [CargoType.Logs] = 6,
            [CargoType.Lumber] = 10,
        };

        public static decimal GetPrice(CargoType ct, int amnt) => AsDict[ct] * amnt;
    }

    public static class CargoValues
    {
        public static Dictionary<CargoType, int> MaxAmntPerCar { get; } = new()
        {
            [CargoType.Passengers] = 20,
            [CargoType.Mail] = 30,
            [CargoType.Logs] = 40,
            [CargoType.Lumber] = 60,
        };
    }
}
