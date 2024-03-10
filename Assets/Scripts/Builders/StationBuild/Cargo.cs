using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

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

        [SerializedDictionary("Cargo Type", "Amnt")]
        public SerializedDictionary<CargoType, int> Amnts = new()
        {
            [CargoType.Passengers] = 0,
            [CargoType.Mail]       = 0,
            [CargoType.Logs]       = 0,
        };

        public Cargo()
        {
            Erase();
        }

        public void Add(CarCargo toAdd) => Amnts[toAdd.CargoType] += toAdd.Amnt;

        public int SubtractFullCarAmnt(CargoType ct)
        {
            int toSubstract = Mathf.Clamp(Amnts[ct], 0, CarCargo.MaxAmnts[ct]);
            Amnts[ct] -= toSubstract;
            return toSubstract;
        }

        public void Erase()
        {
            Amnts[CargoType.Passengers] = 0;
            Amnts[CargoType.Mail]       = 0;
            Amnts[CargoType.Logs]       = 0;
        }

        public Cargo With(CargoType cargoType, int amnt)
        {
            Amnts[cargoType] = amnt;
            return this;
        }
    }
}
