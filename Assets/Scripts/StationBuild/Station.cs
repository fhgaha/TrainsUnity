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

        private void Awake()
        {
            segment = GetComponentInChildren<RoadSegment>();
            GetComponentInChildren<Rotator>().Configure(this);
            GetComponentInChildren<MeshCollider>().sharedMesh = segment.GetMesh();
        }

        public void SetUpRoadSegment()
        {
            List<Vector3> stationRoadPoints = new();
            Vector3 p1 = new() { x = 0, y = 0, z = 10 };
            Vector3 p2 = new() { x = 0, y = 0, z = -10 };
            MyMath.CalculateStraightLine(stationRoadPoints, p1, p2, Global.Instance.DriveDistance);
            segment.GenerateMeshSafely(stationRoadPoints);
        }

    }
}
