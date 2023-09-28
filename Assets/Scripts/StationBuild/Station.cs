using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class Station : MonoBehaviour
    {
        public RoadSegment segment;
        public Vector3 Entry1 => segment.Start;
        public Vector3 Entry2 => segment.End;

        [SerializeField] private List<Vector3> originalPoints;

        private void Awake()
        {
            segment = GetComponentInChildren<RoadSegment>();
            GetComponentInChildren<StationRotator>().Configure(this);
            GetComponentInChildren<MeshCollider>().sharedMesh = segment.GetMesh();
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        public void SetUpRoadSegment()
        {
            originalPoints = new();
            Vector3 p1 = new() { x = 0, y = 0, z = 10 };
            Vector3 p2 = new() { x = 0, y = 0, z = -10 };
            MyMath.CalculateStraightLine(originalPoints, p1, p2, Global.Instance.DriveDistance);

            List<Vector3> segmentPts = new();
            segmentPts.AddRange(originalPoints);
            segment.GenerateMeshSafely(segmentPts);
        }

        public void UpdatePos(Vector3 newPos)
        {
            transform.position = newPos;
            UpdatePos();
        }

        public void UpdatePos()
        {
            for (int i = 0; i < segment.Points.Count; i++)
            {
                segment.Points[i] = transform.rotation * originalPoints[i] + segment.transform.position;
            }
            segment.Start = segment.Points[0];
            segment.End = segment.Points[^1];

            //Debug.DrawRay(segment.Start, 10 * Vector3.up, Color.white, Time.deltaTime);
            //Debug.DrawRay(segment.End, 10 * Vector3.up, Color.white, Time.deltaTime);
        }

    }
}
