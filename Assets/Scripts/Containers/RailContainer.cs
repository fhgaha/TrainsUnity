using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class RailContainer : MonoBehaviour
    {
        //[SerializeField] private Dictionary<int, RoadSegment> segments = new();
        //we dont want railBuilder to detect last placed road. This is to make logic simplier, maybe temporary.
        [SerializedDictionary("id", "segm")]
        public SerializedDictionary<int, RoadSegment> segments = new();
        public RoadSegment LastAdded => segments.Values.LastOrDefault();

        public RoadSegment AddCreateInstance(RoadSegment original)
        {
            RoadSegment copy = Instantiate(original, transform);
            copy.DestroyRigBodyCopyAndPlace(original);
            segments.Add(copy.GetInstanceID(), copy);
            return copy;
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

        public void RemoveSegm(Vector3 start, Vector3 end)
        {
            int index = segments.First(pair =>
                pair.Value.Start == start && pair.Value.End == end ||
                pair.Value.Start == end && pair.Value.End == start
            ).Key;
            RemoveAt(index);
        }

        public void Remove(RoadSegment toRemove)
        {
            var pair = segments.First(pair => pair.Value == toRemove);
            segments.Remove(pair.Key);
            Destroy(pair.Value.gameObject);
        }

        public bool IsLastSegment(RoadSegment segment) => segment != null && segments.Count > 0 && segment == segments.Last().Value;

        public List<Vector3> GetRoadsAsPoints(List<Node> nodes)
        {
            List<Vector3> finalPath = new();
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                Node from = nodes[i];
                Node to = nodes[i + 1];
                List<Vector3> path = new();
                RoadSegment segm = segments.Values.FirstOrDefault(s => from.Pos == s.Start && to.Pos == s.End);
                if (segm is not null)
                {
                    path.AddRange(segm.Points);
                }
                else
                {
                    segm = segments.Values.First(s => from.Pos == s.End && to.Pos == s.Start);
                    path.AddRange(segm.Points);
                    path.Reverse();
                }

                finalPath.AddRange(path);
            }

            return finalPath;
        }
    }
}
