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
        public RoadSegment DetectedRoad { get; set; }
        public HeadedPoint start, end;
        public Vector3 tangent1, tangent2;

        [SerializeField] private Camera cam;
        [SerializeField] private string stateName;  //to display in editor
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private RailContainer railContainer;
        [SerializeField] private RoadSegment segment;
        private RailBuilderState state;
        private DubinsGeneratePaths dubinsPathGenerator = new();
        private RoadDetector detector;
        private float driveDist = 1f;

        private GameObject visual1, visual2, visual3, visual4;    //use like this: DebugVisual(ref visual1, Color.blue, pos);

        private void Awake()
        {
            detector = GetComponentInChildren<RoadDetector>();
            detector.Configure(this, segment, cam);
        }

        private void Start()
        {
            detector.OnDetected += RoadDetector_onDetected;

            gameObject.SetActive(false);
        }

        private void RoadDetector_onDetected(object sender, RoadDetectorEventArgs e)
        {
            DetectedRoad = e.Other;
        }

        private void OnEnable()
        {
            state = RailBuilderState.Configure(this);
        }

        private void OnDisable()
        {
            state = null;
            RemoveMesh();
        }

        private void Update()
        {
            state = state.Handle(cam);
            stateName = state.GetType().Name;

            if (HasPoints)
            {
                //draw line
                lineRenderer.positionCount = Points.Count;
                lineRenderer.SetPositions(Points.ToArray());
            }
        }

        public void CalculateStraightPoints(Vector3 startPos, Vector3 endPos)
        {
            start = new HeadedPoint(startPos, Vector3.SignedAngle(Vector3.forward, endPos - startPos, Vector3.up));
            end = new HeadedPoint(endPos, Vector3.SignedAngle(Vector3.forward, endPos - startPos, Vector3.up));

            MyMath.CalculateStraightLine(Points, startPos, endPos, driveDist);
            segment.GenerateMeshSafely(Points);
        }

        public void CalculateCSPoints() => CalculateCSPoints(start.pos, start.heading, end.pos);

        public void CalculateCSPoints(Vector3 startPos, float startHeading, Vector3 endPos)
        {
            float endHeading = MyMath.CalculateCSPoints(Points, startPos, startHeading, endPos, driveDist, out tangent1, out _, out _);
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
            Points = ptsReversed;
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
            Points = shortest.pathCoordinates;
            segment.GenerateMeshSafely(Points);

            //DebugVisual(ref visual1, Color.blue, tangent1);
            //DebugVisual(ref visual2, Color.red, tangent2);
            //DebugVisual(ref visual3, Color.green, start.pos);
            //DebugVisual(ref visual4, Color.yellow, end.pos);
        }

        public void PutDrawnSegmentIntoContainer()
        {
            segment.data = new RoadSegmentData(start, end, tangent1, tangent2);
            segment.Points = Points;
            railContainer.Add(segment);
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
    }
}
