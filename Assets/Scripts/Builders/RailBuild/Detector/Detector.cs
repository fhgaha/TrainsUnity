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

        private List<Collider> children;
        private float childWidth;
        // < other collider detected, times it was detected by children(once by each child) >
        private Dictionary<DetChild, Dictionary<RoadSegment, int>> childDetectedSegmTimes = new();

        private void Awake()
        {
            children = GetComponentsInChildren<Collider>().ToList();
            childWidth = 2 * children[0].GetComponent<CapsuleCollider>().radius * children[0].transform.localScale.x;
        }

        public void Configure(RailBuilder parent, RoadSegment curRS)
        {
            rb = parent;
            curSegm = curRS;
            children[0].GetComponent<DetChild>().Configure(curSegm);
        }

        private void OnEnable()
        {
            DetChild.OnRoadDetected += OnChildDetectedRoad;
        }

        private void OnChildDetectedRoad(object sender, DetChildEventArgs e)
        {
            Debug.Log($"{sender}, {e.IsEnter}, {e.CollidedWith}");

            DetChild child = ((DetChild)sender); 

            if (e.IsEnter)
                DetectRoad(child, e.CollidedWith.GetComponent<Collider>());
            else
                UndetectRoad(child, e.CollidedWith.GetComponent<Collider>());
        }

        private void OnDisable()
        {
            DetChild.OnRoadDetected -= OnChildDetectedRoad;

            for (int i = children.Count - 1; i >= 0; i--)
            {
                if (i == 0) continue;

                Destroy(children[i].gameObject);
                children.RemoveAt(i);
            }
            childDetectedSegmTimes.Clear();
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
            //DetectRoad(other);
            DetectStation(other);
        }

        private void OnTriggerExit(Collider other)
        {
            //UndetectRoad(other);
            UndetectStation(other);
        }

        private void DetectRoad(DetChild sender, Collider other)
        {
            if (!children.Contains(other)
                && other.TryGetComponent<RoadSegment>(out var rs)
                && rs != curSegm)
            {
                if (childDetectedSegmTimes.Keys.Contains(sender))
                {
                    if (childDetectedSegmTimes[sender].Keys.Contains(rs))
                    {
                        childDetectedSegmTimes[sender][rs]++;
                    }
                    else
                    {
                        childDetectedSegmTimes[sender].Add(rs, 1);
                    }
                }
                else
                {
                    childDetectedSegmTimes.Add(sender, new Dictionary<RoadSegment, int> { [rs] = 1});
                }

                if (childDetectedSegmTimes[sender][rs] == 1)
                {
                    //logic here
                    curSegm.BecomeRed();

                    OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs { CurrentRoad = curSegm, Other = rs });
                    //Debug.Log($"enter other {rs}, times {detectedTimes[other]}");
                }

            }
        }

        private void UndetectRoad(DetChild sender, Collider other)
        {
            if (!children.Contains(other)
                && other.TryGetComponent<RoadSegment>(out var rs)
                && rs != curSegm)
            {
                childDetectedSegmTimes[sender][rs]--;
                if (childDetectedSegmTimes[sender][rs] < 0)
                    throw new Exception($"should not be!");

                if (childDetectedSegmTimes[sender][rs] == 0)
                {
                    //logic here
                    curSegm.BecomeGreen();

                    OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs { CurrentRoad = curSegm, Other = null });
                    //Debug.Log($"exit other {"null"}, times {detectedTimes[other]}");
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
