using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

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

        public IPlayer Owner
        {
            get => owner;
            set
            {
                OwnerName = value.ToString();
                owner = value;
            }
        }
        [SerializeField] public string OwnerName = "---";
        private IPlayer owner;

        [SerializeField] private RoadSegment curSegm;  //should always be the rb's segment
        [SerializeField] private bool isColliding = false;
        [SerializeField] private float thresh = 2;
        [SerializeField] private RailBuilder rb;

        [SerializeField] private List<DetChild> children;    //should not be serialized
        private float childWidth;
        private DetChild mainChild;

        private void Awake()
        {
            children = new List<DetChild>();
            mainChild = GetComponentInChildren<DetChild>(true);
            mainChild.PaintBlue();
            childWidth = 2 * mainChild.GetComponent<CapsuleCollider>().radius * mainChild.transform.localScale.x;
        }

        public void Configure(RailBuilder parent, RoadSegment curRS, IPlayer owner)
        {
            rb = parent;
            curSegm = curRS;
            Owner = owner;

            mainChild.Configure(curSegm);

        }

        private void OnEnable()
        {
            DetChild.OnRoadDetected += OnChildDetectedRoad;

            if (!children.Contains(mainChild))
                children.Add(mainChild);
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
            if (curSegm.Points.Count > 0)
                UpdateRot((curSegm.Points[^1] - curSegm.Points[0]).normalized);
            
            UpdateChildren();
            TryUndetectRoad();
        }

        private void UpdateRot(Vector3 dir)
        {
            if (MyMath.Approx(dir, Vector3.zero))
                transform.rotation = Quaternion.LookRotation(dir);
        }

        private void UpdateChildren()
        {
            if (curSegm == null || curSegm.Points == null || curSegm.Points.Count == 0)
            {
                DestroyChildren();
                return;
            }

            Assert.IsTrue(curSegm.Points != null && curSegm.Points.Count > 0);

            int fittingAmnt = (int)(curSegm.GetApproxLength() / childWidth);
            if (fittingAmnt > children.Count)
            {
                //add more chidren
                var newAmnt = fittingAmnt - children.Count;
                for (int i = 0; i < newAmnt; i++)
                {
                    //some children with no colliders
                    DetChild copy = Instantiate(mainChild, transform, false).Configure(curSegm);
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
                children[i].transform.position = newList[i];
            }
        }

        private void OnChildDetectedRoad(object sender, DetChildEventArgs<RoadSegment> e)
        {
            //Debug.Log($"{sender}, enter {e.IsEnter}, {e.CollidedWith}");

            DetChild child = (DetChild)sender;

            if (e.IsEnter)
                DetectRoad(child, e.CollidedWith);
            else
                UndetectRoad(child, e.CollidedWith);
        }

        private void DetectRoad(DetChild sender, RoadSegment rs)
        {
            bool green = TryPaintGreen();
            if (rs.Owner == owner && green)
            {
                isColliding = true;
                OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs(other: rs, isSentByMainDetChild: sender == mainChild));
            }
        }

        private void UndetectRoad(DetChild sender, RoadSegment rs)
        {
            //if (children.Count >= 2)
            //{
            //    var dist = (mainChild.transform.position - children[1].transform.position).magnitude;
            //    print($"childWidth {childWidth}, dist {dist}");
            //    //this check should be in unptate
            //    if (dist > childWidth)
            //    {
            //        //bool red = !UpdateColorSetGreen();
            //        //if (rs.Owner == owner && red)
            //        if (rs.Owner == owner )
            //            OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs(other: null, isSentByMainDetChild: sender == mainChild));

            //    }
            //}
        }

        private void TryUndetectRoad()
        {
            if (children.Count < 2) return;
            if (!isColliding) return;

            var dist = (mainChild.transform.position - children[1].transform.position).magnitude;
            print($"childWidth {childWidth}, dist {dist}");

            if (dist > (childWidth + thresh))
            {
                OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs(other: null, isSentByMainDetChild: true));
                isColliding = false;
                TryPaintGreen();
            }
        }


        private void OnChildDetectedStation(object sender, DetChildEventArgs<Station> e)
        {
            //Debug.Log($"{sender}, enter {e.IsEnter}, {e.CollidedWith}");

            DetChild child = (DetChild)sender;

            if (e.IsEnter)
                DetectStation(child, e.CollidedWith);
            else
                UndetectStation(child, e.CollidedWith);
        }


        private void DetectStation(DetChild sender, Station st)
        {
            //Debug.Log($"{sender}   detected {st}");

            if (TryPaintGreen())
                OnStationDetected?.Invoke(this, new StationDetectorEventArgs { Station = st });
        }

        void UndetectStation(DetChild sender, Station st)
        {
            //Debug.Log($"{sender} undetected {st}");

            if (!TryPaintGreen())
                OnStationDetected?.Invoke(this, new StationDetectorEventArgs { Station = null });
        }

        private bool TryPaintGreen()
        {
            //rb.SnappedStartRoad == null 
            RoadSegment mainChildDetRoad = mainChild.DetectedRoads.FirstOrDefault(r => r != null);
            List<RoadSegment> roadsWithAnotherOwner = children.SelectMany(c => c.DetectedRoads.Where(r => r.Owner != owner)).ToList();
            List<RoadSegment> RoadsWithSameOwner = children.SelectMany(c => c.DetectedRoads.Where(r => r.Owner == owner)).ToList();
            bool AllRoadsAreDetectedByMain = RoadsWithSameOwner.All(rs => mainChild.DetectedRoads.Contains(rs));

            if (
                roadsWithAnotherOwner.Count == 0
                &&
                AllRoadsAreDetectedByMain
                )
            {
                curSegm.PaintGreen();
                return true;
            }
            else
            {
                curSegm.PaintRed();
                return false;
            }
        }

        bool NoDetectedRoads_All() => !HaveDetectedRoads_All();
        bool HaveDetectedRoads_All() => children.Any(c => c.DetectedRoads.Count > 0);

        bool DetectedContains_All(RoadSegment rs) => children.Any(c => c.DetectedRoads.Contains(rs));

        bool NoDetectedRoads_ExclMain() => !HaveDetectedRoads_ExclMain();
        bool HaveDetectedRoads_ExclMain()
        {
            foreach (var c in children)
            {
                if (c == mainChild) continue;

                if (c.DetectedRoads.Count > 0)
                    return true;
            }
            return false;
        }

        bool OtherChildDetectedAnotherRoadThan(RoadSegment rs, out DetChild child)
        {
            foreach (DetChild c in children)
            {
                if (c == mainChild) continue;

                if (c.DetectedRoads.Any(r => r != null && r != rs))
                {
                    child = c;
                    return true;
                }
            }

            child = null;
            return false;
        }

        bool ChildrenDetectedRoad(RoadSegment rs, out DetChild detectedBy)
        {
            foreach (DetChild c in children)
            {
                if (c == mainChild) continue;

                if (c.DetectedRoads.Contains(rs))
                {
                    detectedBy = c;
                    return true;
                }
            }

            detectedBy = null;
            return false;
        }
    }
}
