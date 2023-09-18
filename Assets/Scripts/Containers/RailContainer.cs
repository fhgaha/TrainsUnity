using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class RailContainer : MonoBehaviour
    {
        [SerializeField] private Dictionary<int, RoadSegment> segments = new();

        public void Add(RoadSegment original)
        {
            RoadSegment copy = Instantiate(original, transform);
            copy.CopyPoints(original);
            copy.SetMesh(original.GetMesh());
            copy.SetCollider(original.GetMesh());
            copy.name = $"Road Segment {copy.GetInstanceID()}";

            segments.Add(copy.GetInstanceID(), copy);
        }

        public RoadSegment Get(int index) => segments[index];

        //Removes the value with the specified key
        public void Remove(int index) => segments.Remove(index);

        public bool IsLastSegment(RoadSegment segment) => segment != null && segments.Count > 0 && segment == segments.Last().Value;
    }
}
