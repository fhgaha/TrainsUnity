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

        public StationDetectorEventArgs(Station station)
        {
            Station = station;
        }
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
        private Camera cam;
        private RoadSegment startSnappedRd;

        private void Awake()
        {
            children = new List<DetChild>();
            mainChild = GetComponentInChildren<DetChild>(true);
            mainChild.PaintBlue();
            childWidth = 2 * mainChild.GetComponent<CapsuleCollider>().radius * mainChild.transform.localScale.x;
        }

        public void Configure(RailBuilder parent, RoadSegment curRS, IPlayer owner, Camera cam)
        {
            rb = parent;
            curSegm = curRS;
            Owner = owner;
            this.cam = cam;

            mainChild.Configure(curSegm);
            mainChild.name = "MainChld";
        }

        private void OnEnable()
        {
            DetChild.OnRoadDetected += OnChildDetectedRoad;
            DetChild.OnStationDetected += OnChildDetectedStation;
        }

        private void OnDisable()
        {
            DetChild.OnRoadDetected -= OnChildDetectedRoad;
            DetChild.OnStationDetected -= OnChildDetectedStation;

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
            bool wasHit = Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f, LayerMask.GetMask("Ground"));
            if (wasHit)
                transform.position = hit.point;

            UpdateChildren();
            TryPaintGreen();
            TryUndetectRoad();

            if (mainChild.DetectedRoads.Count > 0)
            {
                RoadSegment r = mainChild.DetectedRoads.First();
                OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs(other: r, isSentByMainDetChild: true));
            }
            else
            {
                OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs(other: null, isSentByMainDetChild: true));
            }
        }

        private void UpdateChildren()
        {
            if (curSegm == null || curSegm.Points == null || curSegm.Points.Count == 0)
            {
                DestroyChildren();
                return;
            }

            Assert.IsTrue(curSegm.Points != null && curSegm.Points.Count > 0);

            int fittingAmnt = (int)(curSegm.GetApproxLength() / childWidth);// - amntToSkip;
            if (fittingAmnt < 0) return;
            else if (fittingAmnt > children.Count)
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

            if (children.Count == 0) return;

            //get new chlidren positions from thinned out list of points
            var pts = curSegm.Points;
            int ptsPerChild = pts.Count / children.Count;
            List<Vector3> newPos = new();
            //skip forst cause its close to zero
            for (int i = 1; i < pts.Count; i += ptsPerChild)
            {
                newPos.Add(pts[i]);
            }

            //set positions
            for (int i = 0; i < children.Count; i++)
            {
                children[i].transform.position = newPos[i];
            }
        }

        private void OnChildDetectedRoad(object sender, DetChildEventArgs<RoadSegment> e)
        {
            //print($"Detector.OnChildDetectedRoad {sender}, enter {e.IsEnter}, {e.CollidedWith}");

            if (e.IsEnter)
                DetectRoad((DetChild)sender, e.CollidedWith);
        }

        private void DetectRoad(DetChild sender, RoadSegment rs)
        {
            bool green = TryPaintGreen();
            if (rs.Owner == owner
                //&& green
                )
            {
                isColliding = true;
                //OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs(other: rs, isSentByMainDetChild: sender == mainChild));
            }
        }

        private void TryUndetectRoad()
        {
            if (children.Count < 2) return;
            if (!isColliding) return;

            var dist = (mainChild.transform.position - children[1].transform.position).magnitude;
            //print($"childWidth {childWidth}, dist {dist}");

            if (dist > (childWidth + thresh))
            {
                //OnRoadDetected?.Invoke(this, new RoadDetectorEventArgs(other: null, isSentByMainDetChild: true));
                isColliding = false;
                TryPaintGreen();
            }
        }


        private void OnChildDetectedStation(object sender, DetChildEventArgs<Station> e)
        {
            //Debug.Log($"OnChildDetectedStation: {sender}, enter {e.IsEnter}, {e.CollidedWith}");

            DetChild child = (DetChild)sender;
            if (child != mainChild) return;

            if (e.IsEnter)
                DetectStation(child, e.CollidedWith);
            else
                UndetectStation(child, e.CollidedWith);
        }


        private void DetectStation(DetChild sender, Station st)
        {
            Debug.Log($"Detector.DetectStation: {sender}   detected {st}");

            TryPaintGreen();
            OnStationDetected?.Invoke(this, new StationDetectorEventArgs(station: st));
        }

        void UndetectStation(DetChild sender, Station st)
        {
            Debug.Log($"Detector.UndetectStation: {sender} undetected {st}");

            TryPaintGreen();
            OnStationDetected?.Invoke(this, new StationDetectorEventArgs(station: null));
        }

        private bool TryPaintGreen()
        {
            //check colliding with station
            List<Station> childrenDetectedStants = children.SelectMany(c => c.DetectedStations).ToList();
            List<Station> otherChildrenStants = children.Where(c => c != mainChild).SelectMany(c => c.DetectedStations).ToList();

            if (mainChild.DetectedStations.Count > 0 && mainChild.DetectedStations.Any(s => s.Owner != Owner))
            {
                curSegm.PaintRed();
                return false;
            }


            //check colliding with roads
            List<RoadSegment> childrenDetectedRds = children
                .Where(c => c != mainChild).SelectMany(c => c.DetectedRoads).Where(r => r != rb.SnappedStartRoad).ToList();
            List<RoadSegment> otherOwnerRds = children.SelectMany(c => c.DetectedRoads.Where(cr => cr.Owner != Owner)).ToList();

            if (otherOwnerRds.Count > 0
                || childrenDetectedRds.Count > 0 && childrenDetectedRds.Any(r => !mainChild.DetectedRoads.Contains(r))
                )
            {
                curSegm.PaintRed();
                return false;
            }

            curSegm.PaintGreen();
            return true;
        }
    }
}
