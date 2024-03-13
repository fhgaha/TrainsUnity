using System;
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

    public static class CargoInfo
    {
        public static Dictionary<CargoType, int> MaxAmntPerCar { get; } = new()
        {
            [CargoType.Passengers] = 20,
            [CargoType.Mail] = 30,
            [CargoType.Logs] = 40,
            [CargoType.Lumber] = 60,
        };

        public static Dictionary<CargoType, List<CargoType>> Convertions { get; } = new()
        {
            [CargoType.Passengers] = new(),
            [CargoType.Mail] = new(),
            [CargoType.Logs] = new() { CargoType.Lumber },
            [CargoType.Lumber] = new(),
        };

        public static Dictionary<(CargoType, CargoType), int> ConvertionRatios { get; } = new()
        {
            [(CargoType.Logs, CargoType.Lumber)] = 3,
        };
    }
}
