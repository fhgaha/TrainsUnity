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
        private float childWidth;
        // < other collider detected, times it was detected by children(once by each child) >
        private Dictionary<Collider, int> detectedTimes = new();

        private void Awake()
        {
            children = GetComponentsInChildren<Collider>().ToList();
            childWidth = 2 * children[0].GetComponent<CapsuleCollider>().radius * children[0].transform.localScale.x;
        }

        public void Configure(RailBuilder parent, RoadSegment curRS)
        {
            this.rb = parent;
            this.curSegm = curRS;
        }

        private void OnEnable()
        {

        }

        //odisable
        //ontriggerenter
        //update


        private void OnDisable()
        {
            for (int i = 1; i < children.Count; i++)
            {
                Destroy(children[i].gameObject);
                detectedRoads.Clear();
            }
        }

        private void Update()
        {
            UpdateChildren();
        }

        private void UpdateChildren()
        {
            if (curSegm is null || (curSegm.Points?.Count) <= 1) return;

            Vector3 dir = (curSegm.Points[^1] - curSegm.Points[0]).normalized;
            if (MyMath.Approx(dir, Vector3.zero)) return;

            transform.rotation = Quaternion.LookRotation(dir);

            children = GetComponentsInChildren<Collider>().ToList();
            int fittingAmnt = (int)(curSegm.GetApproxLength() / childWidth);

            if (fittingAmnt > children.Count)
            {
                //if move fast children on top of each other
                //add more chidren
                var newAmnt = fittingAmnt - children.Count;
                var newPos = children[^1].transform.position;
                for (int i = 0; i < newAmnt; i++)
                {
                    //some children with no colliders
                    var copy = Instantiate(children[0], transform, false);
                    copy.name = $"DetChild {copy.GetInstanceID()}";
                    newPos += childWidth * (-dir);
                    copy.transform.position = newPos;

                    //check if OnTriggerEnter gets called
                }
            }
            else if (fittingAmnt < children.Count)
            {
                //remove children that dont fit
                int amntToDestroy = Mathf.Abs(fittingAmnt - children.Count);
                for (int i = 0; i < amntToDestroy; i++)
                {
                    if (children.Count == 1)
                        return;

                    Destroy(children[^1].gameObject);
                    //if there is collider in detectedTimes we should decrease times value
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            DetectRoad(other);
            DetectStation(other);
        }

        private void OnTriggerExit(Collider other)
        {
            UndetectRoad(other);
            UndetectStation(other);
        }

        private void DetectRoad(Collider other)
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
                    //logic here

                    OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs { CurrentRoad = curSegm, Other = rs });
                    Debug.Log("enter");
                }

                if (detectedTimes[other] > children.Count)
                    throw new Exception("should not be");
            }
        }

        private void UndetectRoad(Collider other)
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
                    //logic here

                    OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs { CurrentRoad = curSegm, Other = null });
                    Debug.Log($"exit");
                }

            }
        }

        private void DetectStation(Collider other)
        {
            //should i have list for stations as well, like for the roads?
            if (!other.CompareTag("Station")) return;

            OnStationDetected?.Invoke(this, new StationDetectorEventArgs { Station = other.GetComponent<Station>() });
        }

        void UndetectStation(Collider other)
        {
            if (!other.CompareTag("Station")) return;

            OnStationDetected?.Invoke(this, new StationDetectorEventArgs { Station = null });
        }


    }
}
