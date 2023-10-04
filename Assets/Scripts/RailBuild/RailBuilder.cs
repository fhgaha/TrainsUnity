using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{

    public class RailBuilder : MonoBehaviour
    {
        public List<Vector3> Points { get; private set; } = new();
        public bool HasPoints => Points != null && Points.Count > 0;
        public RoadSegment DetectedRoad;
        public Station DetectedStation { get; set; }
        public HeadedPoint start, end;
        public Vector3 tangent1, tangent2;

        [SerializeField] private Vector3 snappedStartPos;   //to display in editor
        [SerializeField] private Camera cam;
        [SerializeField] private string stateName;          //to display in editor
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private RailContainer railContainer;
        [SerializeField] private RoadSegment segment;
        public RoadSegment Segment => segment;
        private RailBuilderState state;
        private DubinsGeneratePaths dubinsPathGenerator = new();
        private Detector detector;
        private float driveDist = 1f;

        private GameObject visual1, visual2, visual3, visual4;    //use like this: DebugVisual(ref visual1, Color.blue, pos);

        private void Awake()
        {
            detector = GetComponentInChildren<Detector>();
            detector.Configure(this, segment, cam);
        }

        private void Start()
        {
            //detector.OnRoadDetected += (sender, e) => DetectedRoad = e.Other;
            //detector.OnStationDetected += (sender, e) => DetectedStation = e.Station;

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            detector.OnRoadDetected += (sender, e) => DetectedRoad = e.Other;
            detector.OnStationDetected += (sender, e) => DetectedStation = e.Station;
            
            if (state is null) state = RailBuilderState.Configure(this);
        }

        private void OnDisable()
        {
            detector.OnRoadDetected -= (sender, e) => DetectedRoad = e.Other;
            detector.OnStationDetected -= (sender, e) => DetectedStation = e.Station;

            state = RailBuilderState.selectingStartState;
            RemoveMesh();
        }

        private void Update()
        {
            bool wasHit = Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f, LayerMask.GetMask("Ground"));
            state = state.Handle(wasHit, hit.point, Input.GetKeyUp(KeyCode.Mouse0), Input.GetKeyUp(KeyCode.Mouse1));
            stateName = state.GetType().Name;

            if (HasPoints)
            {
                //draw line
                lineRenderer.positionCount = Points.Count;
                lineRenderer.SetPositions(Points.ToArray());
            }

            //debug
            snappedStartPos = RailBuilderState.selectingStartState.SnappedStart;
        }

        public void BuildRoad(Vector3 start, RoadSegment snappedStartRoad, Vector3 goal, RoadSegment snappedEndRoad)
        {
            state = RailBuilderState.drawingInitialSegmentState;

            switch (snappedStartRoad, snappedEndRoad)
            {
                case (RoadSegment, RoadSegment):
                    
                    break;
                case (RoadSegment, null):
                    break;
                case (null, RoadSegment):
                    break;
                case (null, null):
                    break;
            }
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

        public void RegisterI(Vector3 start, Vector3 end) => RouteManager.Instance.RegisterI(start, end, segment.GetApproxLength());

        public void RegisterII(Vector3 newNodePos, Vector3 nodeWeConnectedToPos) => RouteManager.Instance.RegisterII(newNodePos, nodeWeConnectedToPos, segment.GetApproxLength());

        public void RegisterT(Vector3 start, Vector3 connection, RoadSegment otherRoad)
        {
            (RoadSegment segment1, RoadSegment segment2) = SplitSegment(otherRoad, connection);

            RouteManager.Instance.RegisterT(
                start, connection, RoadSegment.GetApproxLength(Points),
                otherRoad.Start, otherRoad.End,
                segment1.GetApproxLength(), segment2.GetApproxLength()
            );

            railContainer.Remove(otherRoad);
            railContainer.AddDontCreateInstance(segment1);
            railContainer.AddDontCreateInstance(segment2);
        }

        public void RegisterH(
            RoadSegment oldRoad1, Vector3 oldRoad1Connection,
            RoadSegment oldRoad2, Vector3 oldRoad2Connection,
            RoadSegment newSegm
        )
        {
            //oldRoad2.points does not contain oldRoad2Connection
            (RoadSegment ae, RoadSegment eb) = SplitSegment(oldRoad1, oldRoad1Connection);
            (RoadSegment cf, RoadSegment fd) = SplitSegment(oldRoad2, oldRoad2Connection);

            RouteManager.Instance.RegisterH(
                oldRoad1Connection, oldRoad2Connection, newSegm.GetApproxLength(),
                oldRoad1.Start, ae.GetApproxLength(),
                oldRoad1.End, eb.GetApproxLength(),
                oldRoad2.Start, cf.GetApproxLength(),
                oldRoad2.End, fd.GetApproxLength()
            );

            railContainer.Remove(oldRoad1);
            railContainer.Remove(oldRoad2);
            railContainer.AddDontCreateInstance(ae);
            railContainer.AddDontCreateInstance(eb);
            railContainer.AddDontCreateInstance(cf);
            railContainer.AddDontCreateInstance(fd);
        }

        public void RegisterC(RoadSegment newRoad) => RouteManager.Instance.RegisterC(newRoad.Start, newRoad.End, newRoad.GetApproxLength());

        public void RegisterIT(RoadSegment roadMidConnected, RoadSegment newRoad, Vector3 connection)
        {
            (RoadSegment ad, RoadSegment db) = SplitSegment(roadMidConnected, connection);
            Vector3 end = connection == newRoad.End ? newRoad.Start : newRoad.End;

            RouteManager.Instance.RegisterIT(
                connection, ad.GetApproxLength(), db.GetApproxLength(), newRoad.GetApproxLength(),
                roadMidConnected.Start, roadMidConnected.End, end
            );

            railContainer.Remove(roadMidConnected);
            railContainer.AddDontCreateInstance(ad);
            railContainer.AddDontCreateInstance(db);
        }

        private (RoadSegment, RoadSegment) SplitSegment(RoadSegment roadToSplit, Vector3 splitPt)
        {
            RoadSegment segment1 = Instantiate(roadToSplit, railContainer.transform);
            RoadSegment segment2 = Instantiate(roadToSplit, railContainer.transform);

            (List<Vector3> splitted1, List<Vector3> splitted2) = SplitPointsInTwoSets(roadToSplit.Points, splitPt);

            segment1.ConfigureFrom(splitted1);
            segment2.ConfigureFrom(splitted2);

            return (segment1, segment2);
        }

        private (List<Vector3>, List<Vector3>) SplitPointsInTwoSets(List<Vector3> originalPts, Vector3 splitPt)
        {
            List<Vector3> newPts1 = new();
            List<Vector3> newPts2 = new();

            bool sendToFirst = true;
            foreach (var p in originalPts)
            {
                if (p == splitPt)
                {
                    sendToFirst = false;
                    newPts1.Add(p);
                }

                if (sendToFirst)
                    newPts1.Add(p);
                else
                    newPts2.Add(p);
            }
            return (newPts1, newPts2);
        }

        private float GetPartialLength(Vector3 from, Vector3 to, List<Vector3> pts)
        {
            if (from != pts[0] && from != pts[^1]) throw new Exception($"Point {from} is not start or end of points {pts}");

            List<Vector3> foundPts = new();
            for (int i = 0; i < pts.Count; i++)
            {
                if (pts[i] != from) break;

                foundPts.Add(pts[i]);
                if (pts[i] == to) break;
            }

            for (int i = pts.Count - 1; i >= 0; i--)
            {
                if (pts[i] != from) break;

                foundPts.Add(pts[i]);
                if (pts[i] == to) break;
            }
            float startToConnectionLength = RoadSegment.GetApproxLength(foundPts);
            return startToConnectionLength;
        }
    }
}
