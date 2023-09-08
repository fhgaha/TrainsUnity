using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class RailContainer : MonoBehaviour
    {
        [SerializeField] private Dictionary<int, RoadSegment> segments = new();

        public void Add(RoadSegment segm)
        {
            RoadSegment copy = Instantiate(segm, transform);
            copy.CopyPoints(segm);
            copy.SetMesh(segm.GetMesh());
            copy.SetCollider(segm.GetMesh());
            copy.name = $"Road Segment {copy.GetInstanceID()}";

            segments.Add(copy.GetInstanceID(), copy);
        }

        public RoadSegment Get(int index) => segments[index];

        //Removes the value with the specified key
        public void Remove(int index) => segments.Remove(index);

        public bool IsLastSegment(RoadSegment segment) => segment != null && segments.Count > 0 && segment == segments.Last().Value;
    }
}
