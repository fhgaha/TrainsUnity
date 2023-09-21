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
        private RoadSegment curRS;  //should always be the rb's segment
        private Camera cam;
        private List<RoadSegment> detectedRoads = new();

        public void Configure(RailBuilder parent, RoadSegment curRS, Camera cam)
        {
            this.parent = parent;
            this.curRS = curRS;
            this.cam = cam;
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
                if (otherSegment == curRS) return;

                OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs { CurrentRoad = curRS, Other = otherSegment });
                detectedRoads.Add(otherSegment);
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
                OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs { CurrentRoad = curRS, Other = detectedRoads.LastOrDefault() });
            }

            void UndetectStation()
            {
                if (!other.CompareTag("Station")) return;

                OnStationDetected?.Invoke(this, new StationDetectorEventArgs { Station = null });
            }
        }


        void Update()
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f, LayerMask.GetMask("Ground")))
            {
                transform.position = hit.point;
            }
        }
    }
}
