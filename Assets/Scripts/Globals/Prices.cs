using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public enum CargoType { Passengers, Mail, Logs, Lumber };

    public static class Prices
    {
        //prices are per unit
        public static decimal PassengerPrice => 10;
        public static decimal MailPrice => 5;
        public static decimal Logs => 6;
        public static decimal Lumber => 10;

        public static Dictionary<CargoType, decimal> AsDict = new()
        {
            [CargoType.Passengers] = PassengerPrice,
            [CargoType.Mail] = MailPrice,
            [CargoType.Logs] = Logs,
            [CargoType.Lumber] = Lumber,
        };
    }

    public static class CargoValues
    {
        public static Dictionary<CargoType, decimal> PricesAsDict;
        //MaxAmntsPerCar

    }
}
