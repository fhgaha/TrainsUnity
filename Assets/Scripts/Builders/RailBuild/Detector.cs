using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class RoadDetectorEventArgs : EventArgs
    {
        public RoadSegment CurrentRoad { get; set; }
        public RoadSegment Other { get; set; }
    }

    public class StationDetectorEventArgs : EventArgs
    {
        public Station Station { get; set; }
    }


    public class Detector : MonoBehaviour
    //public class Detector : DetectorWithChildren
    {
        public event EventHandler<RoadDetectorEventArgs> OnRoadDetected;
        public event EventHandler<StationDetectorEventArgs> OnStationDetected;
        public override string ToString() => $"Detector {GetInstanceID()}";

        private RailBuilder rb;
        [SerializeField] private RoadSegment curSegm;  //should always be the rb's segment
        [SerializeField] private List<RoadSegment> detectedRoads = new();

        private List<Collider> children;
        private Dictionary<Collider, int> detectedTimes = new();

        private void Awake()
        {
            children = GetComponentsInChildren<Collider>().ToList();
        }

        public void Configure(RailBuilder parent, RoadSegment curRS)
        {
            this.rb = parent;
            this.curSegm = curRS;
        }

        private void OnTriggerEnter(Collider other)
        {
            Check(other, null);

            DetectRoad();
            DetectStation();

            void DetectRoad()
            {
                if (other.TryGetComponent<RoadSegment>(out var otherSegm))
                {
                    if (otherSegm == curSegm) return;
                    //Debug.Log($"{this} Det on trig ent");

                    detectedRoads.Add(otherSegm);
                    OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs { CurrentRoad = curSegm, Other = otherSegm });
                    //Debug.Log($"road detected: {otherSegment}");
                }
            }

            void DetectStation()
            {
                //should i have list for stations as well, like for the roads?
                if (!other.CompareTag("Station")) return;

                OnStationDetected?.Invoke(this, new StationDetectorEventArgs { Station = other.GetComponent<Station>() });
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Uncheck(other, null);

            UndetectRoad();
            UndetectStation();

            void UndetectRoad()
            {
                if (other.TryGetComponent<RoadSegment>(out var otherSegm))
                {
                    if (otherSegm == curSegm) return;
                    //Debug.Log($"{this} Det on trig exit");

                    //bool managedToRemove = detectedRoads.Remove(otherSegm);
                    //if (!managedToRemove) throw new Exception("couldnt remove from list");
                    OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs { CurrentRoad = curSegm, Other = detectedRoads.LastOrDefault() });
                }
            }

            void UndetectStation()
            {
                if (!other.CompareTag("Station")) return;

                OnStationDetected?.Invoke(this, new StationDetectorEventArgs { Station = null });
            }
        }


        private void Check(Collider other, Action action)
        {
            if (!children.Contains(other)
                && other.TryGetComponent<RoadSegment>(out var rs)
                && rs != curSegm)
            {
                if (detectedTimes.Keys.Contains(other))
                    detectedTimes[other]++;
                else
                    detectedTimes.Add(other, 1);

                if (detectedTimes[other] == 1)
                {
                    action();
                    Debug.Log("enter");
                }

                if (detectedTimes[other] > children.Count)
                    throw new Exception("should not be");
            }
        }

        private void Uncheck(Collider other, Action action)
        {
            if (!children.Contains(other)
                && other.TryGetComponent<RoadSegment>(out var rs)
                && rs != curSegm)
            {
                detectedTimes[other]--;
                if (detectedTimes[other] < 0)
                    throw new Exception($"should not be!");

                if (detectedTimes[other] == 0)
                {
                    action();
                    Debug.Log($"exit");
                }

            }
        }
    }
}
