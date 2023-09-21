using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class RailContainer : MonoBehaviour
    {
        [SerializeField] private Dictionary<int, RoadSegment> segments = new();
        //we dont want railBuilder to detect last placed road. This is to make logic simplier, maybe temporary.
        public RoadSegment LastAdded => segments.Values.LastOrDefault();

        public void AddCreateInstance(RoadSegment original)
        {
            RoadSegment copy = Instantiate(original, transform);
            copy.ConfigureFrom(original);
            segments.Add(copy.GetInstanceID(), copy);
        }

        public void AddDontCreateInstance(RoadSegment segm)
        {
            segments.Add(segm.GetInstanceID(), segm);
        }

        public RoadSegment Get(int index) => segments[index];

        //Removes the value with the specified key
        public void RemoveAt(int index)
        {
            RoadSegment toRemove = segments[index];
            segments.Remove(index);
            Destroy(toRemove.gameObject);
        }

        public void Remove(RoadSegment toRemove)
        {
            var pair = segments.First(pair => pair.Value == toRemove);
            segments.Remove(pair.Key);
            Destroy(pair.Value.gameObject);
        }

        public bool IsLastSegment(RoadSegment segment) => segment != null && segments.Count > 0 && segment == segments.Last().Value;
    }
}
