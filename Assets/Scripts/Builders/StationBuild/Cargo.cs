using System;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    [Serializable]
    public class Cargo
    {
        public static Cargo Empty
        {
            get
            {
                Cargo c = new();
                c.Erase();
                return c;
            }
        }

        [field: SerializeField] public Dictionary<CargoType, int> Amnts { get; private set; } = new()
        {
            [CargoType.Passengers] = 5,
            [CargoType.Mail] = 6,
            [CargoType.Wood] = 7
        };

        public Dictionary<CargoType, int> MaxAmnts { get; } = new()
        {
            [CargoType.Passengers] = 15,
            [CargoType.Mail] = 20,
            [CargoType.Wood] = 50
        };

        public void Add(CarriageCargo toAdd) => Amnts[toAdd.CargoType] += toAdd.Amnt;

        public void Erase()
        {
            Amnts[CargoType.Passengers] = 0;
            Amnts[CargoType.Mail] = 0;
            Amnts[CargoType.Wood] = 0;
        }

        public Cargo With(CargoType cargoType, int amnt)
        {
            Amnts[cargoType] = amnt;
            return this;
        }
    }
}
