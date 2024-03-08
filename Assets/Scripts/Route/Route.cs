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

        public List<CarCargo> GetCargoToLoad(int amnt)
        {
            List<CarCargo> res = new();
            List<(CargoType ct, int amnt)> demands = StationTo.CargoHandler.Demand.Amnts
                .Select(p => (p.Key, p.Value))
                .OrderByDescending(p => p.Value).ToList();

            if (demands.All(p => p.amnt == 0))
            {
                for (int i = 0; i < amnt; i++)
                    res.Add(CarCargo.Empty);

                return res;
            }

            for (int i = 0; i < amnt; i++)
            {
                if (demands.Count == 0)
                {
                    res.Add(CarCargo.Empty);
                    continue;
                }

                //use demand of StationTo cause we want to send only goods that can be accepted by StationTo
                //15
                (CargoType ct, int amnt) demand = demands.First();
                int maxAmnt = CarCargo.MaxAmnts[demand.ct];
                //10
                int supply = StationFrom.CargoHandler.Supply.Amnts[demand.ct];
                //10
                int loadAmnt = supply == maxAmnt ? maxAmnt : supply % maxAmnt;
                res.Add(new CarCargo { CargoType = demand.ct, Amnt = loadAmnt });
                StationFrom.CargoHandler.Supply.Amnts[demand.ct] -= loadAmnt;

                if (StationFrom.CargoHandler.Supply.Amnts[demand.ct] <= 0)
                    demands.RemoveAt(0);
            }

            return res;
        }

        public List<CarCargo> GetCargoToLoad_NoSubtraction(int amnt)
        {
            //get CarCargo list with cargoTypes set based on demand and amnts empty
            List<CarCargo> res = new();
            List<Tuple<CargoType, int, decimal>> demand = StationTo.CargoHandler.Demand.Amnts
                .Select(p => Tuple.Create(p.Key, p.Value, Prices.AsDict[p.Key]))
                .OrderByDescending(p => p.Item2 * p.Item3)
                .ToList();

            //all vals zero
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
                int maxAmnt = CarCargo.MaxAmnts[desType];
                int supAmnt = supply[desType];
                int toLoad = supAmnt % maxAmnt;
                if (toLoad == 0) toLoad = maxAmnt;
                CarCargo r = new() { CargoType = desType, Amnt = 0 };
                res.Add(r);
                demand[0] = Tuple.Create(desType, desAmnt - toLoad, price);
            }

            return res;
        }


        //https://wiki.openttd.org/en/Manual/Game%20Mechanics/Cargo%20income

    }
}
