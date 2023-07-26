using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class RailBuilder : MonoBehaviour
    {
        public List<Vector3> points;
        public bool HasPoints => points != null && points.Count > 0;
        public Vector3 StartPos { get; set; }
        public Vector3 EndPos { get; set; }
        public float startHeading, endHeading;

        [SerializeField] private Camera _camera;
        [SerializeField] private string _stateName;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Mesh2D _road2DSO;
        [SerializeField] private RailContainer _railContainer;
        [SerializeField] private RoadSegment _segment;
        private RailBuilderState _state;
        private DubinsGeneratePaths dubinsPathGenerator = new();

        private float driveDist = 1f;

        private void OnEnable()
        {
            _state = RailBuilderState.Init(this);
        }

        private void OnDisable()
        {
        }

        private void Update()
        {
            _state = _state.HandleInput(_camera);
            _stateName = _state.GetType().Name;

            if (HasPoints)
            {
                //draw line
                _lineRenderer.positionCount = points.Count;
                _lineRenderer.SetPositions(points.ToArray());
            }
        }

        public void CalculateStraightPoints(Vector3 startPos, Vector3 endPos)
        {
            startHeading = Vector3.SignedAngle(Vector3.forward, endPos - startPos, Vector3.up);
            endHeading = Vector3.SignedAngle(Vector3.forward, endPos - startPos, Vector3.up);

            List<Vector3> pts = MyMath.CalculateStraightLine(startPos, endPos, driveDist);

            if (pts.Count > 0)
            {
                points = pts;
                _segment.GenerateMesh(points);
            }
        }

        #region debug
        private GameObject startHeadingVisual, endHeadingVisual, visual1, visual2;
        #endregion
        public void CalculateCSPoints(Vector3 startPos, Vector3 endPos)
        {
            endHeading = MyMath.CalculateCSPoints(points, startPos, startHeading, endPos, driveDist, out Vector3 t1, out Vector3 t2);
            _segment.GenerateMesh(points);

            //PlaceVisual(ref visual1, Color.blue, t1);
            //PlaceVisual(ref visual2, Color.cyan, t2);
            //PlaceVisual(ref startHeadingVisual, new Color(0.5f, 1, 0), startPos);
            //PlaceVisual(ref endHeadingVisual, new Color(1, 0.5f, 0), endPos);
        }

        #region debug
        private void PlaceVisual(ref GameObject go, Color color, Vector3 pos)
        {
            go = CreateVisual(go, color);
            go.transform.position = pos;
            go.transform.SetParent(this.transform);
        }

        private GameObject CreateVisual(GameObject go, Color color)
        {
            if (go != null) return go;

            var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.GetComponent<MeshRenderer>().material.color = color;
            visual.GetComponent<Collider>().enabled = false;
            visual.transform.localScale = new Vector3(3f, 5f, 3f);
            return visual;
        }
        #endregion

        public void PutDrawnSegmentIntoContainer()
        {
            _segment.points = points;
            _railContainer.Add(_segment);
        }
    }
}
