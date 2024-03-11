using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    [Serializable]
    public class CarCargo
    {
        public static CarCargo Empty => new();
        [field: SerializeField] public CargoType CargoType { get; set; }
        [field: SerializeField] public int Amnt { get; set; } = 0;

        public override string ToString() => $"{CargoType}, {Amnt}";

        public decimal GetWorthValue() => Prices.GetPrice(CargoType, Amnt);

        public void Erase()
        {
            Amnt = 0;
        }
    }
}
