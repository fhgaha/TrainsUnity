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

        public static List<(CargoType, int)> GetAmntDiffDescending(StationCargoHandler from, StationCargoHandler to)
        {
            List<(CargoType ct, int profit)> differences = new();

            var supplyAmnts = from.Supply.Amnts;
            var demandAmnts = to.Demand.Amnts;

            foreach (CargoType ct in Enum.GetValues(typeof(CargoType)))
            {
                int diff = demandAmnts[ct] - supplyAmnts[ct];
                differences.Add((ct, diff));
            }

            differences = differences.OrderByDescending(p => p.profit).ToList();
            return differences;
        }

        [SerializedDictionary("Cargo Type", "Amnt")]
        public SerializedDictionary<CargoType, int> Amnts = new();

        //public Dictionary<CargoType, int> MaxAmnts = new()
        //{
        //    [CargoType.Passengers] = 15,
        //    [CargoType.Mail] = 20,
        //    [CargoType.Wood] = 50
        //};



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
            Amnts[CargoType.Mail] = 0;
            Amnts[CargoType.Logs] = 0;
        }

        public Cargo With(CargoType cargoType, int amnt)
        {
            Amnts[cargoType] = amnt;
            return this;
        }
    }
}
