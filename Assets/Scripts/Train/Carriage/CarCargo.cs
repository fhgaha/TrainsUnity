using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public enum CargoType { Passengers, Mail, Wood };

    [Serializable]
    public class CarCargo
    {
        public static Dictionary<CargoType, int> MaxAmnts { get; } = new()
        {
            [CargoType.Passengers] = 20,
            [CargoType.Mail] = 30,
            [CargoType.Wood] = 40
        };

        [field: SerializeField] public CargoType CargoType { get; set; }
        [field: SerializeField] public int Amnt { get; set; } = 0;


        public decimal GetWorthValue() => CargoType switch
        {
            CargoType.Passengers => Amnt * Prices.PassengerPrice,
            CargoType.Mail => Amnt * Prices.MailPrice,
            CargoType.Wood => Amnt * Prices.Wood,
            _ => throw new NotImplementedException()
        };

        public void Erase()
        {
            Amnt = 0;
        }
    }
}
