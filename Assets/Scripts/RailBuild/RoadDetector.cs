using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class RoadDetectorEventArgs : EventArgs
    {
        public RoadSegment CurrentRoad { get; set; }
        public RoadSegment Other { get; set; }
    }

    public class RoadDetector : MonoBehaviour
    {
        public event EventHandler<RoadDetectorEventArgs> OnDetected;

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
            if (!other.CompareTag("Road")) return;

            RoadSegment otherSegment = other.GetComponent<RoadSegment>();
            if (otherSegment == curRS) return;

            OnDetected?.Invoke(this, new RoadDetectorEventArgs { CurrentRoad = curRS, Other = otherSegment });
            detectedRoads.Add(otherSegment);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Road")) return;
            RoadSegment otherSegment = other.GetComponent<RoadSegment>();
            detectedRoads.Remove(otherSegment);
            if (detectedRoads.Count == 0)
            {
                OnDetected?.Invoke(this, new RoadDetectorEventArgs { CurrentRoad = curRS, Other = null });
            }
        }

        void Update()
        {
            //Vector3 endDir = new Vector3 { x = Mathf.Sin(parent.end.heading * Mathf.Deg2Rad), z = Mathf.Cos(parent.end.heading * Mathf.Deg2Rad) }.normalized;
            //float dist = transform.lossyScale.x * coll.radius + Mathf.Epsilon;
            //transform.position = parent.end.pos + dist * endDir;

            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f, LayerMask.GetMask("Ground")))
            {
                transform.position = hit.point;
            }
        }
    }
}
