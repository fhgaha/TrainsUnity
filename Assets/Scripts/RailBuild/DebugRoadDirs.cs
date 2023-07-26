using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class DebugRoadDirs : MonoBehaviour
    {
        [SerializeField] RailBuilder rb;
        [SerializeField] private GameObject startFirst;
        [SerializeField] private GameObject startSecond;
        [SerializeField] private GameObject endFirst;
        [SerializeField] private GameObject endSecond;

        // Update is called once per frame
        void Update()
        {
            var points = rb.points;
            if (points == null || points.Count < 2) return;

            var dirFromStart = (points[0] - points[1]).normalized;
            var dirFromEnd = (points[points.Count - 1] - points[points.Count - 2]).normalized;

            //DrawArrow.ForDebug(points[0], dirFromStart);
            //DrawArrow.ForDebug(points.Last(), dirFromEnd);

            //DrawArrow.ForGizmo(points[0], dirFromStart);
            //DrawArrow.ForGizmo(points.Last(), dirFromEnd);

            startFirst = CreateCube(startFirst, Color.green);
            startFirst.transform.position = points[0];

            startSecond = CreateCube(startSecond, Color.green);
            startSecond.transform.position = points[0] + dirFromStart * 10f;


            endFirst = CreateCube(endFirst, Color.red);
            endFirst.transform.position = points.Last();

            endSecond = CreateCube(endSecond, Color.red);
            endSecond.transform.position = points.Last() + dirFromEnd * 10f;
        }

        private GameObject CreateCube(GameObject go, Color color)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<MeshRenderer>().material.color = color;
            cube.transform.localScale = new Vector3(3f, 3f, 3f);
            go = go != null ? go : cube;
            return go;
        }
    }

    public static class DrawArrow
    {
        public static void ForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }

        public static void ForGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }

        public static void ForDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Debug.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Debug.DrawRay(pos + direction, right * arrowHeadLength);
            Debug.DrawRay(pos + direction, left * arrowHeadLength);
        }

        public static void ForDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Debug.DrawRay(pos, direction, color);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
            Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
        }
    }
}
