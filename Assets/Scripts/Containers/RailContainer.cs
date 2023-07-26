using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class RailContainer : MonoBehaviour
    {
        public Dictionary<int, RoadSegment> segments = new();

        public void Add(RoadSegment segm)
        {
            RoadSegment copy = Instantiate(segm, transform);
            copy.points = segm.points;
            
            int lastIndex = segments.Keys.LastOrDefault();
            segments.Add(++lastIndex, copy);
            copy.SetMesh(segm.GetMesh());
        }
    }
}
