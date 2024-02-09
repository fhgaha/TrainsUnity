using AYellowpaper.SerializedCollections;
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
        public bool IsSentByMainDetChild { get; set; }

        public RoadDetectorEventArgs(RoadSegment currentRoad, RoadSegment other, bool isSentByMainDetChild)
        {
            CurrentRoad = currentRoad;
            Other = other;
            IsSentByMainDetChild = isSentByMainDetChild;
        }
    }

    public class StationDetectorEventArgs : EventArgs
    {
        public Station Station { get; set; }
    }


    //create children for cur segm
    //when moving cursor update children pos and amnt
    //if one of children collide with road paint segm red
    //if none of chilren collide with road pain segm green
    public class Detector : MonoBehaviour
    {
        public event EventHandler<RoadDetectorEventArgs> OnRoadDetected;
        public event EventHandler<StationDetectorEventArgs> OnStationDetected;
        public override string ToString() => $"Detector {GetInstanceID()}";

        private RailBuilder rb;
        [SerializeField] private RoadSegment curSegm;  //should always be the rb's segment

        private float childWidth;
        // < other collider detected, times it was detected by children(once by each child) >
        [SerializeField] private List<DetChild> children = new();
        private DetChild mainChild;

        private void Awake()
        {
            //GetComponentsInChildren<DetChild>().ToList().ForEach(c => childDetectedSegmTimes.Add(c, new SerializedDictionary<RoadSegment, int>()));
            mainChild = GetComponentInChildren<DetChild>(true);
            mainChild.PaintBlue();
            childWidth = 2 * mainChild.GetComponent<CapsuleCollider>().radius * mainChild.transform.localScale.x;
        }

        public void Configure(RailBuilder parent, RoadSegment curRS)
        {
            rb = parent;
            curSegm = curRS;
            mainChild.Configure(curSegm);
        }

        private void OnEnable()
        {
            DetChild.OnRoadDetected += OnChildDetectedRoad;

            children.Add(mainChild);
        }

        private void OnChildDetectedRoad(object sender, DetChildEventArgs e)
        {
            Debug.Log($"{sender}, enter {e.IsEnter}, {e.CollidedWith}");

            DetChild child = (DetChild)sender;

            if (e.IsEnter)
                DetectRoad(child, e.CollidedWith.GetComponent<Collider>());
            else
                UndetectRoad(child, e.CollidedWith.GetComponent<Collider>());
        }

        private void OnDisable()
        {
            DetChild.OnRoadDetected -= OnChildDetectedRoad;

            DestroyChildren();
        }

        public void DestroyChildren()
        {
            for (int i = children.Count - 1; i >= 0; i--)
            {
                if (i == 0) continue;

                DetChild toRemove = children[i];
                children.Remove(toRemove);
                Destroy(toRemove.gameObject);
            }
            children.Clear();
        }

        private void Update()
        {
            UpdateChildren();
        }

        //private void UpdateChildren()
        //{
        //    if (curSegm is null || (curSegm.Points?.Count) <= 1) return;

        //    List<Vector3> pts = curSegm.Points;
        //    Vector3 dir = (pts[^1] - pts[0]).normalized;
        //    if (MyMath.Approx(dir, Vector3.zero)) return;

        //    transform.rotation = Quaternion.LookRotation(dir);

        //    children = GetComponentsInChildren<Collider>().ToList();
        //    int fittingAmnt = (int)(curSegm.GetApproxLength() / childWidth);

        //    List<Vector3> newList = new() { pts[^1] };
        //    for (int i = pts.Count - 1; i >= 0; i--)
        //    {
        //        if (Vector3.Distance(newList.Last(), pts[i]) > childWidth)
        //        {
        //            newList.Add(pts[i]);
        //            continue;
        //        }
        //    }

        //    for (int i = 0; i < newList.Count - 1; i++)
        //    {
        //        if (i == 0) continue;

        //        var copy = Instantiate(children[0], transform, false);
        //        copy.name = $"DetChild {copy.GetInstanceID()}";
        //        copy.transform.position = newList[i];
        //        children.Add(copy);
        //    }
        //}

        private void UpdateChildren()
        {
            if (curSegm is null || (curSegm.Points?.Count) <= 1) return;

            Vector3 dir = (curSegm.Points[^1] - curSegm.Points[0]).normalized;
            if (MyMath.Approx(dir, Vector3.zero)) return;

            transform.rotation = Quaternion.LookRotation(dir);

            //children = GetComponentsInChildren<Collider>().ToList();
            int fittingAmnt = (int)(curSegm.GetApproxLength() / childWidth);

            if (fittingAmnt > children.Count)
            {
                //if move fast children on top of each other
                //add more chidren
                var newAmnt = fittingAmnt - children.Count;
                var newPos = children.Last().transform.position;
                for (int i = 0; i < newAmnt; i++)
                {
                    //some children with no colliders
                    var copy = Instantiate(mainChild, transform, false).Configure(curSegm);
                    copy.PaintRed();
                    newPos += childWidth * (-dir);
                    copy.transform.position = newPos;
                    children.Add(copy);
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

                    DetChild last = children.Last();
                    children.Remove(last.GetComponent<DetChild>());
                    Destroy(last.gameObject);
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
            if (!other.TryGetComponent<DetChild>(out _)
                && other.TryGetComponent<RoadSegment>(out var rs)
                && rs != curSegm)
            {
                if (children.Contains(sender))
                {
                    if (sender.DetectedTimes.Keys.Contains(rs))
                    {
                        sender.DetectedTimes[rs]++;
                    }
                    else
                    {
                        sender.DetectedTimes.Add(rs, 1);
                    }
                }
                else
                {
                    children.Add(sender);
                }

                if (AtLeastOneTimesNotZero())
                {
                    curSegm.PaintRed();
                }


                OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs(curSegm, rs, sender == mainChild));
                //Debug.Log($"enter other {rs}, times {detectedTimes[other]}");

            }
        }

        private void UndetectRoad(DetChild sender, Collider other)
        {
            if (!other.TryGetComponent<DetChild>(out _)
                && other.TryGetComponent<RoadSegment>(out var rs)
                && rs != curSegm)
            {
                sender.DetectedTimes[rs]--;
                if (sender.DetectedTimes[rs] < 0)
                    throw new Exception($"should not be!");

                if (sender.DetectedTimes[rs] == 0)
                    sender.DetectedTimes.Remove(rs);

                if (AreAllTimesZero())
                {
                    curSegm.PaintGreen();
                    OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs(curSegm, null, sender == mainChild));
                }

                //Debug.Log($"exit other {"null"}, times {detectedTimes[other]}");
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

        bool AreAllTimesZero() => children.All(c => c.DetectedTimes.Values.All(times => times == 0));
        bool AtLeastOneTimesNotZero() => children.Any(c => c.DetectedTimes.Values.Any(times => times > 0));
    }
}
