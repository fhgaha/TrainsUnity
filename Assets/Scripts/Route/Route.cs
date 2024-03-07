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
        public List<(CargoType, decimal)> GetCargoToLoad(int amnt)
        {
            //(supply - demand) * Length * Prices.PassengerPrice

            List<(CargoType ct, decimal profit)> differences = new();

            var demandAmnts = StationTo.CargoHandler.Demand.Amnts;
            var supplyAmnts = StationFrom.Cargo.Amnts;

            foreach (CargoType ct in Enum.GetValues(typeof(CargoType)))
            {
                decimal diff = (demandAmnts[ct] - supplyAmnts[ct]) * Prices.PassengerPrice;
                differences.Add((ct, diff));
            }

            differences = differences.OrderByDescending(p => p.profit).Take(amnt).ToList();
            return differences;

            //should retrun list of CarCargo to build cars
        }
    }
}
