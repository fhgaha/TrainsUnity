using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    [Serializable]
    public class Route
    {
        public Station StationFrom { get; set; }
        public Station StationTo { get; set; }
        //public Vector3 StationFromEntry { get; set; }
        //public Vector3 StationToEntry { get; set; }
        public List<Station> Stations { get; set; }
        public List<Vector3> PathForward { get; set; }
        public List<Vector3> PathBack { get; set; }     //not reversing PathForward cause oath back can be different? or it cant?
        public float Length => RoadSegment.GetApproxLength(PathForward);

        public Route() { }

        public Route(List<Station> stations, List<Vector3> pathTo, List<Vector3> pathBack)
        {
            StationFrom = stations.First();
            StationTo = stations.Last();
            Stations = stations;
            PathForward = pathTo;
            PathBack = pathBack;
        }

        public Route Reversed()
        {
            Route reversed = new();

            //this may not work
            Station tempRoute = StationFrom;
            reversed.StationFrom = StationTo;
            reversed.StationTo = tempRoute;

            reversed.Stations = Stations.Reverse<Station>().ToList();

            //this may not work
            List<Vector3> tempPath = PathForward;
            PathForward = PathBack;
            PathBack = tempPath;

            return reversed;
        }

        //https://wiki.openttd.org/en/Manual/Game%20Mechanics/Cargo%20income
        public List<CarCargo> GetCargoTypesToLoad(int amnt)
        {
            //get CarCargo list with cargoTypes set based on demand and amnts empty
            List<CarCargo> res = new();
            List<Tuple<CargoType, int, decimal>> demand = StationTo.CargoHandler.Demand.Amnts
                .Select(p => Tuple.Create(p.Key, p.Value, Prices.AsDict[p.Key]))
                .OrderByDescending(p => PriceOfAmnt(p))
                .ToList();

            //no keys, no values if station has no goods
            Dictionary<CargoType, int> supply = new(StationFrom.CargoHandler.Supply.Amnts);     //copy

            for (int i = 0; i < amnt; i++)
            {
                if (supply.Count == 0) break;   //if this happens at first iteration the train will go empty
                if (demand.Count == 0) break;
                (CargoType desType, int desAmnt, decimal price) = demand[0];
                int supAmnt = supply[desType];
                if (supAmnt == 0)
                {
                    demand.RemoveAt(0);
                    i--;
                    continue;
                }
                int maxAmnt = CargoInfo.MaxAmntPerCar[desType];
                int toLoad = supAmnt % maxAmnt;
                if (supAmnt != 0 && toLoad == 0) toLoad = maxAmnt;
                CarCargo r = new() { CargoType = desType, Amnt = 0 };
                res.Add(r);
                demand[0] = Tuple.Create(desType, desAmnt - toLoad, price);
                supply[desType] -= toLoad;
            }

            return res;
        }

        public List<CargoType> GetCargoTypesToLoad_TypeList(int amnt)
        {
            //get CarCargo list with cargoTypes set based on demand and amnts empty
            List<CargoType> res = new();
            List<Tuple<CargoType, int, decimal>> demand = StationTo.CargoHandler.Demand.Amnts
                .Select(p => Tuple.Create(p.Key, p.Value, Prices.AsDict[p.Key]))
                //.OrderByDescending(p => p.Item2 * p.Item3)
                .OrderByDescending(p => PriceOfAmnt(p))
                .ToList();

            Dictionary<CargoType, int> supply = StationFrom.CargoHandler.Supply.Amnts.ToDictionary(p => p.Key, p => p.Value);

            for (int i = 0; i < amnt; i++)
            {
                if (demand.Count == 0) break;
                (CargoType desType, int desAmnt, decimal price) = demand[0];
                if (desAmnt == 0)
                {
                    demand.RemoveAt(0);
                    continue;
                }
                res.Add(desType);
                int maxAmnt = CargoInfo.MaxAmntPerCar[desType];
                int supAmnt = supply[desType];
                int toLoadAmnt = supAmnt % maxAmnt;
                if (toLoadAmnt == 0) toLoadAmnt = maxAmnt;
                demand[0] = Tuple.Create(desType, desAmnt - toLoadAmnt, price);
            }

            return res;
        }

        private int PriceOfAmnt(Tuple<CargoType, int, decimal> t)
        {
            (CargoType ct, int amnt, decimal price) = t;
            int maxAmnt = CargoInfo.MaxAmntPerCar[ct];
            if (amnt == 0)
                return 0;
            if (amnt % maxAmnt == 0)
                return maxAmnt * (int)price;
            return amnt % maxAmnt * (int)price;
        }
    }
}
