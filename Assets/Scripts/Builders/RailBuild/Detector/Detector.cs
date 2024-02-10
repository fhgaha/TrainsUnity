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
        public RoadSegment Other { get; set; }
        public bool IsSentByMainDetChild { get; set; }

        public RoadDetectorEventArgs(RoadSegment other, bool isSentByMainDetChild)
        {
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
        private List<DetChild> children;    //should not be serialized
        private DetChild mainChild;

        private void Awake()
        {
            children = new List<DetChild>();
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

            if (!children.Contains(mainChild))
                children.Add(mainChild);
        }

        private void OnChildDetectedRoad(object sender, DetChildEventArgs e)
        {
            Debug.Log($"{sender}, enter {e.IsEnter}, {e.CollidedWith}");

            DetChild child = (DetChild)sender;

            if (e.IsEnter)
                DetectRoad(child, e.CollidedWith);
            else
                UndetectRoad(child, e.CollidedWith);
        }

        private void OnDisable()
        {
            DetChild.OnRoadDetected -= OnChildDetectedRoad;

            DestroyChildren();
        }

        public void DestroyChildren()
        {
            for (int i = 0; i < children.Count; i++)
            {
                DetChild c = children[i];
                if (c == mainChild) continue;

                children.Remove(c);
                Destroy(c.gameObject);
            }
        }

        private void Update()
        {
            UpdateChildren();
            UpdateColor();
        }

        //private void UpdateChildren()
        //{
        //    if (curSegm is null || (curSegm.Points?.Count) <= 1) return;

        //    Vector3 dir = (curSegm.Points[^1] - curSegm.Points[0]).normalized;
        //    if (MyMath.Approx(dir, Vector3.zero)) return;

        //    transform.rotation = Quaternion.LookRotation(dir);

        //    //children = GetComponentsInChildren<Collider>().ToList();
        //    int fittingAmnt = (int)(curSegm.GetApproxLength() / childWidth);

        //    if (fittingAmnt > children.Count)
        //    {
        //        //if move fast children on top of each other
        //        //add more chidren
        //        var newAmnt = fittingAmnt - children.Count;
        //        var newPos = children.Last().transform.position;
        //        for (int i = 0; i < newAmnt; i++)
        //        {
        //            //some children with no colliders
        //            var copy = Instantiate(mainChild, transform, false).Configure(curSegm);
        //            copy.PaintRed();
        //            newPos += childWidth * (-dir);
        //            copy.transform.position = newPos;
        //            children.Add(copy);
        //            //check if OnTriggerEnter gets called
        //        }
        //    }
        //    else if (fittingAmnt < children.Count)
        //    {
        //        //remove children that dont fit
        //        int amntToDestroy = Mathf.Abs(fittingAmnt - children.Count);
        //        for (int i = 0; i < amntToDestroy; i++)
        //        {
        //            if (children.Count == 1)
        //                return;

        //            DetChild last = children.Last();
        //            children.Remove(last.GetComponent<DetChild>());
        //            Destroy(last.gameObject);
        //            //if there is collider in detectedTimes we should decrease times value
        //        }
        //    }
        //}

        private void UpdateChildren()
        {
            if (curSegm is null || curSegm.Points is null) return;
            if (curSegm.Points?.Count < 1)
            {
                DestroyChildren();
                return;
            }

            Vector3 dir = (curSegm.Points[^1] - curSegm.Points[0]).normalized;
            if (MyMath.Approx(dir, Vector3.zero)) return;

            transform.rotation = Quaternion.LookRotation(dir);

            int fittingAmnt = (int)(curSegm.GetApproxLength() / childWidth);

            if (fittingAmnt > children.Count)
            {
                //if move fast children on top of each other
                //add more chidren
                var newAmnt = fittingAmnt - children.Count;
                for (int i = 0; i < newAmnt; i++)
                {
                    //some children with no colliders
                    var copy = Instantiate(mainChild, transform, false).Configure(curSegm);
                    copy.PaintRed();
                    children.Add(copy);
                }
            }
            else if (fittingAmnt < children.Count)
            {
                //remove children that dont fit
                int amntToDestroy = Mathf.Abs(fittingAmnt - children.Count);
                for (int i = 0; i < amntToDestroy; i++)
                {
                    DetChild last = children.Last();

                    if (last == mainChild) continue;

                    children.Remove(last.GetComponent<DetChild>());
                    Destroy(last.gameObject);
                }
            }

            //get list of fewer points
            var pts = curSegm.Points;
            int ptsPerChild = pts.Count / children.Count;
            List<Vector3> newList = new();
            for (int i = pts.Count - 1; i >= 0; i -= ptsPerChild)
            {
                newList.Add(pts[i]);
            }

            //set positions
            for (int i = 1; i < children.Count; i++)
            {
                children[i].transform.position = newList[i - 1];
            }

            Debug.Assert(children.Count != 0, "children count is zero 1");

            UnityEngine.Assertions.Assert.IsTrue(children.Count != 0, "children count is zero 2");

            System.Diagnostics.Debug.Assert(children.Count != 0, "children count is zero 3");
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

        private void DetectRoad(DetChild sender, RoadSegment rs)
        {
            if (!children.Contains(sender))
                children.Add(sender);

            if (AtLeastOneTimesNotZero())
                curSegm.PaintRed();

            if (sender == mainChild)
                OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs(rs, sender == mainChild));
        }

        private void UndetectRoad(DetChild sender, RoadSegment rs)
        {
            if (AreAllTimesZero())
            {
                curSegm.PaintGreen();
                OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs(other: null, isSentByMainDetChild: true));
                return;
            }

            OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs(other: null, isSentByMainDetChild: sender == mainChild));
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

        private void UpdateColor()
        {
            if (AtLeastOneTimesNotZero())
                curSegm.PaintRed();
            else
                curSegm.PaintGreen();
        }

        bool AreAllTimesZero() => !AtLeastOneTimesNotZero();
        bool AtLeastOneTimesNotZero() => children.Any(c => c.Detected.Count > 0);
    }
}
