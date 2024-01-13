using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    [Serializable]
    public class Route
    {
        public List<Station> Stations;
        public List<Vector3> AsPoints { get; set; }
        public List<Vector3> StationsAsPoints
        {
            get
            {
                //var graph = ...
                //var nodes = graph. get stations as nodes
                //var list = Global.Instance.RailContainer.GetRoadsAsPoints(nodes)
                //return list

                return null;
            }
        }


    }
}
