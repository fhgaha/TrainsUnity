using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public enum CargoType { Passengers, Mail, Wood };

    public class CarriageCargo
    {
        public CargoType CargoType { get; set; }
        public int Amnt { get; set; } = 0;

        public decimal GetWorthValue() => CargoType switch
        {
            CargoType.Passengers => Amnt * Prices.PassengerPrice,
            CargoType.Mail       => Amnt * Prices.MailPrice,
            CargoType.Wood       => Amnt * Prices.Wood,
            _ => throw new NotImplementedException()
        };

        public void Erase()
        {
            Amnt = 0;
        }
    }
}
