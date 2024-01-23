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
        public Vector3 StationFromEntry { get; set; }
        public Vector3 StationToEntry { get; set; }
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
    }
}
