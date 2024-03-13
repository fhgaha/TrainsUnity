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
        public static Cargo AllZero => new();

        [SerializedDictionary("Cargo Type", "Amnt")]
        public SerializedDictionary<CargoType, int> Amnts = new();

        public Cargo() => Erase();
       
        public void Erase()
        {
            foreach (CargoType ct in Enum.GetValues(typeof(CargoType)))
                Amnts[ct] = 0;
        }

        public void Add(CarCargo toAdd) => Amnts[toAdd.CargoType] += toAdd.Amnt;

        public int SubtractFullCarAmnt(CargoType ct)
        {
            int toSubstract = Mathf.Clamp(Amnts[ct], 0, CargoInfo.MaxAmntPerCar[ct]);
            Amnts[ct] -= toSubstract;
            return toSubstract;
        }

        public Cargo With(CargoType cargoType, int amnt)
        {
            Amnts[cargoType] = amnt;
            return this;
        }
    }
}
