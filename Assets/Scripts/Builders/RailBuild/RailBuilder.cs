using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class RailBuilder : MonoBehaviour
    {
        public override string ToString() => $"{base.ToString()} {GetInstanceID()}";
        public IPlayer Owner
        {
            get => owner;
            private set
            {
                ownerName = $"{value.GetType()}, id: {value.Id}";
                owner = value;
            }
        }
        [SerializeField] private string ownerName = "---";
        private IPlayer owner;

        public List<Vector3> Points { get; private set; } = new();
        public bool HasPoints => Points != null && Points.Count > 0;

        //start
        [field: SerializeField] public RoadSegment SnappedStartRoad { get; set; }
        public Vector3 SnappedStart { get; set; }
        public List<Vector3> SnappedStartPoints { get; set; } = new();
        public bool IsStartSnapped => SnappedStart != Vector3.zero && SnappedStartRoad != null;
        

        [field: SerializeField] public RoadSegment DetectedByEndRoad { get; set; }
        public Station DetectedByEndStation { get; set; }
        public HeadedPoint start, end;
        public Vector3 tangent1, tangent2;

        public RoadSegment Segment => segment;
        [SerializeField] private RoadSegment segment;
        [SerializeField] private Camera cam;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private RailContainer railContainer;
        [SerializeField] private RbStateMachine stateMachine;

        private RegisterHelper regHelp;
        private DubinsGeneratePaths dubinsPathGenerator = new();
        private Detector detector;
        private float driveDist = 1f;
        private GameObject visual1, visual2, visual3, visual4;    //use like this: DebugVisual(ref visual1, Color.blue, pos);

        public RailBuilder Configure(IPlayer owner, Camera camera)
        {
            Owner = owner;
            segment.Owner = Owner;
            cam = camera;
            detector.Configure(this, segment, Owner, cam);

            return this;
        }

        private void Awake()
        {
            detector = GetComponentInChildren<Detector>();

            regHelp = GetComponent<RegisterHelper>();
            regHelp.Configure(segment, railContainer, this);

            stateMachine = new RbStateMachine(this, regHelp);
            segment.PaintGreen();
        }

        private void OnEnable()
        {
            detector.OnRoadDetected += OnRoadDetected;
            detector.OnStationDetected += OnStationDetected;
        }

        private void OnDisable()
        {
            detector.OnRoadDetected -= OnRoadDetected;
            detector.OnStationDetected -= OnStationDetected;
            RemoveMesh();

            ResetCustom();
        }

        private void Update()
        {
            if (Owner is not HumanPlayer) return;

            //TODO main camera assigned! ai player should have its own camera
            bool wasHit = Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f, LayerMask.GetMask("Ground"));
            stateMachine.UpdateState(wasHit, hit.point, Input.GetKeyUp(KeyCode.Mouse0), Input.GetKeyUp(KeyCode.Mouse1));

            if (HasPoints)
            {
                //draw line
                lineRenderer.positionCount = Points.Count;
                lineRenderer.SetPositions(Points.ToArray());
            }
        }

        public void ResetCustom()
        {
            RemoveMesh();
            stateMachine = new RbStateMachine(this, regHelp);
        }

        private void OnRoadDetected(object sender, RoadDetectorEventArgs e)
        {
            //print($"Rb.OnRoadDetected {sender}, {e.Other}, sent by main chld: {e.IsSentByMainDetChild}");
            if (sender is not Detector d || d != detector) return;
            
            RoadSegment detected = e.Other;

            if (detected == null)
            {
                if (e.IsSentByMainDetChild)
                    DetectedByEndRoad = null;
            }
            else
            {
                if (detected.Owner == Owner)
                    if (e.IsSentByMainDetChild)
                        DetectedByEndRoad = e.Other;
            }
        }

        private void OnStationDetected(object sender, StationDetectorEventArgs e)
        {
            DetectedByEndStation = e.Station;
        }

        public IEnumerator BuildRoad_Routine(Vector3 start, Vector3 goal)
        {
            //selecting start state
            //move det, set up detected road if any
            detector.transform.position = start;
            yield return new WaitForFixedUpdate();

            //switch to drawing initial segment state
            stateMachine.UpdateState(wasHit: true, hitPoint: start, lmbPressed: true, rmbPressed: false);

            //move det, set up detected road if any
            detector.transform.position = goal;
            yield return new WaitForFixedUpdate();

            yield return new WaitWhile(() => detector.gameObject.transform.position == start);

            //switch to drawing noninitial segment state
            stateMachine.UpdateState(wasHit: true, hitPoint: goal, lmbPressed: true, rmbPressed: false);
            yield return new WaitUntil(() => stateMachine.CurrentState is RbNoninitialSegmentState);
            if (detector.gameObject.transform.position != goal || detector.gameObject.transform.position == start)
                throw new Exception($"Detector pos: {detector.transform.position}, should be {goal}");

            //switch to select start state
            stateMachine.UpdateState(wasHit: true, hitPoint: goal, lmbPressed: false, rmbPressed: true);
            yield return new WaitUntil(() => stateMachine.CurrentState is RbSelectStartState);
        }

        public void RemoveRoad(Vector3 start, Vector3 end)
        {
            railContainer.RemoveSegm(start, end);
            RouteManager.Instance.UnregisterEdge(start, end);
        }

        public void CalculateStraightPoints(Vector3 startPos, Vector3 endPos)
        {
            start = new HeadedPoint(startPos, Vector3.SignedAngle(Vector3.forward, endPos - startPos, Vector3.up));
            end = new HeadedPoint(endPos, Vector3.SignedAngle(Vector3.forward, endPos - startPos, Vector3.up));

            MyMath.CalculateStraightLine(Points, startPos, endPos, driveDist);
            segment.UpdateMeshAndCollider(Points);
        }

        private void UpdatePoints(List<Vector3> pts)
        {
            Points = pts;
        }

        ///Curve + straight
        public void CalculateCurveWithStriaghtPoints() => CalculateCurveWithStriaghtPoints(start.pos, start.heading, end.pos);

        public void CalculateCurveWithStriaghtPoints(Vector3 startPos, float startHeading, Vector3 endPos)
        {
            float endHeading = MyMath.CalculateCurveWithStriaghtPoints(Points, startPos, startHeading, endPos, driveDist, out tangent1, out _, out _);
            //UpdateSegmentEndings(Points);
            tangent2 = Vector3.positiveInfinity;
            end = new HeadedPoint(endPos, endHeading);
            segment.UpdateMeshAndCollider(Points);
        }

        public void CalculateCurveWithStriaghtPointsReversed() => CalculateCurveWithStriaghtPointsPointsReversed(start.pos, end.pos, end.heading);

        public void CalculateCurveWithStriaghtPointsPointsReversed(Vector3 startPos, Vector3 endPos, float endHeading)
        {
            List<Vector3> ptsReversed = new();
            float reversedEndHeading = MyMath.ClampToPlusMinus180DegreesRange(endHeading + 180);
            float reversedStartHeading = MyMath.CalculateCurveWithStriaghtPoints(ptsReversed, endPos, reversedEndHeading, startPos, driveDist, out tangent1, out _, out _);
            tangent2 = Vector3.positiveInfinity;
            ptsReversed.Reverse();
            UpdatePoints(ptsReversed);
            float newStartHeading = MyMath.ClampToPlusMinus180DegreesRange(reversedStartHeading + 180);

            start = new HeadedPoint(startPos, newStartHeading);
            segment.UpdateMeshAndCollider(Points);
        }

        public void CalcDrawDubinsPoints() => CalcDrawDubinsPoints(start.pos, start.heading, end.pos, end.heading);

        public void CalcDrawDubinsPoints(Vector3 startPos, float startHeading, Vector3 endPos, float endHeading)
        {
            var paths = dubinsPathGenerator.GetAllDubinsPaths_UseDegrees(startPos, startHeading, endPos, endHeading);
            var ordered = paths.OrderBy(p => p.totalLength);
            OneDubinsPath shortest = ordered.FirstOrDefault();

            //var shortest1 = ordered.FirstOrDefault();
            //var shortest2 = ordered.Skip(1).FirstOrDefault();

            //if (shortest1 != null && shortest2 != null)
            //{
            //    var tresh = 3f;
            //    var diff = shortest1.totalLength - shortest2.totalLength;
            //    if (diff * diff < tresh)
            //    {
            //        shortest = new[] { shortest1, shortest2 }.Where(s => s.segment1TurningRight).First();
            //    }
            //}

            var pts = shortest.pathCoordinates;
            tangent1 = shortest.tangent1;
            tangent2 = shortest.tangent2;
            Points = pts;
            segment.UpdateMeshAndCollider(Points);
        }

        public RoadSegment PlaceSegment()
        {
            segment.SetPointsAndOwner(Points, Owner);
            RoadSegment copy = railContainer.AddCreateInstance(segment);
            return copy;
        }

        private void DebugVisual(ref GameObject go, Color color, Vector3 pos)
        {
            go = DebugCreateVisual(go, color);
            go.transform.position = pos;
            go.transform.SetParent(this.transform);
        }

        private GameObject DebugCreateVisual(GameObject go, Color color)
        {
            if (go != null) return go;

            var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "debug-" + visual.GetInstanceID().ToString();
            visual.GetComponent<MeshRenderer>().material.color = color;
            visual.GetComponent<Collider>().enabled = false;
            visual.transform.localScale = new Vector3(3f, 5f, 3f);
            return visual;
        }

        public void RemoveMesh()
        {
            segment.CopyMeshValuesFrom(null);
            Points.Clear();
        }

        public void UnsnapStart()
        {
            SnappedStart = Vector3.zero;
            SnappedStartRoad = null;
            SnappedStartPoints.Clear();
        }

        public void SnapStart(Vector3 newStartPos, RoadSegment snappedStartRoad, List<Vector3> snappedStartPoints)
        {
            start.pos = newStartPos;
            SnappedStart = start.pos;
            SnappedStartRoad = snappedStartRoad;
            SnappedStartPoints = snappedStartPoints;
        }
    }
}
