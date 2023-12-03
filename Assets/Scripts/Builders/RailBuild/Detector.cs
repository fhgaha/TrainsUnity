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
    {
        public event EventHandler<RoadDetectorEventArgs> OnRoadDetected;
        public event EventHandler<StationDetectorEventArgs> OnStationDetected;

        private RailBuilder parent;
        [SerializeField] private RoadSegment curSegm;  //should always be the rb's segment
        [SerializeField] private List<RoadSegment> detectedRoads = new();

        public void Configure(RailBuilder parent, RoadSegment curRS)
        {
            this.parent = parent;
            this.curSegm = curRS;
        }

        //TODO: should i use on triggers stay instdead? and send road or null all the time?
        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log(otherSegment.transform.position);

            DetectRoad();
            DetectStation();

            void DetectRoad()
            {
                if (!other.CompareTag("Road")) return;

                RoadSegment otherSegment = other.GetComponent<RoadSegment>();
                if (otherSegment == curSegm) return;

                OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs { CurrentRoad = curSegm, Other = otherSegment });
                detectedRoads.Add(otherSegment);

                //Debug.Log($"road detected: {otherSegment}");
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
            UndetectRoad();
            UndetectStation();

            void UndetectRoad()
            {
                if (!other.CompareTag("Road")) return;

                RoadSegment otherSegment = other.GetComponent<RoadSegment>();
                detectedRoads.Remove(otherSegment);
                OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs { CurrentRoad = curSegm, Other = detectedRoads.LastOrDefault() });
            }

            void UndetectStation()
            {
                if (!other.CompareTag("Station")) return;

                OnStationDetected?.Invoke(this, new StationDetectorEventArgs { Station = null });
            }
        }
    }
}
