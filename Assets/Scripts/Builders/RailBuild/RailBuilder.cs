using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class RailBuilder : MonoBehaviour
    {
        public List<Vector3> Points { get; private set; } = new();
        public bool HasPoints => Points != null && Points.Count > 0;
        public RoadSegment DetectedRoad { get; set; }
        public Station DetectedStation { get; set; }
        public HeadedPoint start, end;
        public IPlayer Parent;
        public Vector3 tangent1, tangent2;

        [SerializeField] private Vector3 snappedStartPos;   //to display in editor
        [SerializeField] private Camera cam;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private RailContainer railContainer;
        [SerializeField] private RoadSegment segment;
        public RoadSegment Segment => segment;

        #region snapped start info
        public Vector3 SnappedStart { get; set; }
        public RoadSegment SnappedStartRoad { get; set; }
        public List<Vector3> SnappedStartPoints { get; set; } = new();
        public bool IsStartSnapped => SnappedStart != Vector3.zero && SnappedStartRoad != null;
        #endregion

        private RegisterHelper regHelp;
        private DubinsGeneratePaths dubinsPathGenerator = new();
        private Detector detector;
        private float driveDist = 1f;

        private GameObject visual1, visual2, visual3, visual4;    //use like this: DebugVisual(ref visual1, Color.blue, pos);

        [SerializeField] private RbStateMachine stateMachine;

        public override string ToString() => $"{base.ToString()} {GetInstanceID()}";

        private void Awake()
        {
            detector = GetComponentInChildren<Detector>();
            detector.Configure(this, segment);

            regHelp = GetComponent<RegisterHelper>();
            regHelp.Configure(segment, railContainer);

            stateMachine = new RbStateMachine(this, regHelp);
        }

        private void Start()
        {
        }

        private void OnEnable()
        {
            detector.OnRoadDetected += SetDetectedRoad;
            detector.OnStationDetected += (sender, e) => DetectedStation = e.Station;

            //if (state is null)
            //{
            //    state = new RailBuilderState().Configure(this, regHelp); //all other states are null
            //}
        }

        private void OnDisable()
        {
            detector.OnRoadDetected -= SetDetectedRoad;
            detector.OnStationDetected -= (sender, e) => DetectedStation = e.Station;

            //state = new RailBuilderState().Configure(this, regHelp);
            RemoveMesh();
        }

        private void SetDetectedRoad(object sender, RoadDetectorEventArgs e)
        {
            if (sender is not Detector d || d != detector) return;

            DetectedRoad = e.Other;
        }

        public void Update()
        {
            if (Parent is AiPlayer) return;

            bool wasHit = Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f, LayerMask.GetMask("Ground"));
            if (wasHit)
            {
                detector.transform.position = hit.point;
            }

            //state = state.Handle(wasHit, hit.point, Input.GetKeyUp(KeyCode.Mouse0), Input.GetKeyUp(KeyCode.Mouse1));
            //stateName = state.GetType().Name;
            stateMachine.UpdateState(wasHit, hit.point, Input.GetKeyUp(KeyCode.Mouse0), Input.GetKeyUp(KeyCode.Mouse1));

            if (HasPoints)
            {
                //draw line
                lineRenderer.positionCount = Points.Count;
                lineRenderer.SetPositions(Points.ToArray());
            }

            //debug
            snappedStartPos = SnappedStart;
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



        //public void RemoveBuiltRoads(params Vector3[] pts)
        //{
        //    RouteManager rm = RouteManager.Instance;
        //    for (int i = 0; i < pts.Length - 1; i++)
        //    {
        //        Vector3 start = pts[i];
        //        Vector3 end = pts[i + 1];
        //        railContainer.RemoveSegm(start, end);
        //        rm.UnregisterNodes(start);
        //    }
        //    rm.UnregisterNode(pts[^1]);
        //}

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
            //UpdateSegmentEndings(Points);
            segment.GenerateMeshSafely(Points);
        }

        private void UpdatePoints(List<Vector3> pts)
        {
            Points = pts;
            //UpdateSegmentEndings(pts);
        }

        public void UpdateSegmentEndings() => UpdateSegmentEndings(Points);
        private void UpdateSegmentEndings(List<Vector3> pts)
        {
            segment.Start = pts[0];
            segment.End = pts[^1];
        }

        ///Curve + straight
        public void CalculateCSPoints() => CalculateCSPoints(start.pos, start.heading, end.pos);

        public void CalculateCSPoints(Vector3 startPos, float startHeading, Vector3 endPos)
        {
            float endHeading = MyMath.CalculateCSPoints(Points, startPos, startHeading, endPos, driveDist, out tangent1, out _, out _);
            //UpdateSegmentEndings(Points);
            tangent2 = Vector3.positiveInfinity;
            end = new HeadedPoint(endPos, endHeading);
            segment.GenerateMeshSafely(Points);
        }

        public void CalculateCSPointsReversed() => CalculateCSPointsReversed(start.pos, end.pos, end.heading);

        public void CalculateCSPointsReversed(Vector3 startPos, Vector3 endPos, float endHeading)
        {
            List<Vector3> ptsReversed = new();
            float reversedEndHeading = MyMath.ClampToPlusMinus180DegreesRange(endHeading + 180);
            float reversedStartHeading = MyMath.CalculateCSPoints(ptsReversed, endPos, reversedEndHeading, startPos, driveDist, out tangent1, out _, out _);
            tangent2 = Vector3.positiveInfinity;
            ptsReversed.Reverse();
            UpdatePoints(ptsReversed);
            float newStartHeading = MyMath.ClampToPlusMinus180DegreesRange(reversedStartHeading + 180);

            start = new HeadedPoint(startPos, newStartHeading);
            segment.GenerateMeshSafely(Points);
        }

        public void CalculateDubinsPoints() => CalculateDubinsPoints(start.pos, start.heading, end.pos, end.heading);

        public void CalculateDubinsPoints(Vector3 startPos, float startHeading, Vector3 endPos, float endHeading)
        {
            OneDubinsPath shortest = dubinsPathGenerator.GetAllDubinsPaths_UseDegrees(startPos, startHeading, endPos, endHeading).First();
            tangent1 = shortest.tangent1;
            tangent2 = shortest.tangent2;
            UpdatePoints(shortest.pathCoordinates);
            segment.GenerateMeshSafely(Points);
        }

        public void PutDrawnSegmentIntoContainer()
        {
            //Debug.DrawRay(Points[0], 20 * Vector3.up, Color.green, float.PositiveInfinity);
            //Debug.DrawRay(Points[^1], 20 * Vector3.up, Color.red, float.PositiveInfinity);

            segment.data = new RoadSegmentData(start, end, tangent1, tangent2); //not used
            segment.Points = Points;
            UpdateSegmentEndings();
            railContainer.AddCreateInstance(segment);
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
            segment.SetMesh(null);
            Points.Clear();
        }

        public void UnsnapStart()
        {
            SnappedStart = Vector3.zero;
            SnappedStartRoad = null;
            SnappedStartPoints.Clear();
        }


    }
}
