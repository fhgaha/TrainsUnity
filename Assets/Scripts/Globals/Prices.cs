using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public static class Prices
    {
        //prices are per unit
        public static decimal PassengerPrice => 10;
        public static decimal MailPrice => 5;
        public static decimal Wood => 6;

        public static Dictionary<CargoType, decimal> AsDict = new()
        {
            [CargoType.Passengers] = PassengerPrice,
            [CargoType.Mail] = MailPrice,
            [CargoType.Logs] = Wood
        };
    }
}
