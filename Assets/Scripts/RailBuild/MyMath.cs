using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

namespace Trains
{
    //TODO: there should not be distance between road segments
    public static class MyMath
    {
        //returns at least one point which is startPos
        public static List<Vector3> CalculateStraightLine(Vector3 startPos, Vector3 endPos, float driveDistance)
        {
            float dist = (endPos - startPos).magnitude;
            if (dist < Epsilon) return new List<Vector3>(); //not doing this causes 'Look rotation viewing vector is zero' in OrientedPoint constructor

            int segments = Mathf.FloorToInt(dist / driveDistance);
            Vector3 dir = (endPos - startPos).normalized;
            Vector3 cur = startPos;
            List<Vector3> pts = new() { startPos };
            for (int i = 0; i <= segments; i++)
            {
                cur += driveDistance * dir;
                pts.Add(cur);
            }
            pts.Insert(pts.Count - 1, endPos);  //in case last point is not same as endPos
            return pts;
        }

        //returns at least one point which is startPos
        public static List<Vector3> CalculateArcPoints(Vector3 startPos, float headingDeg, float arcLength, float r, bool isRight, float driveDistance)
        {
            float parameter = isRight ? 1 : -1; //which side are we going on arc
            List<Vector3> pts = new() { startPos };
            Vector3 currentPos = startPos;
            float theta = headingDeg * Deg2Rad;
            int segments = FloorToInt(arcLength / driveDistance);
            for (int i = 0; i < segments; i++)
            {
                currentPos.x += driveDistance * Sin(theta);
                currentPos.z += driveDistance * Cos(theta);
                theta += parameter * driveDistance / r;
                pts.Add(currentPos);
            }
            return pts;
        }

        //CS is Curve + Straght
        public static float CalculateCSPoints(List<Vector3> resultPoints, Vector3 startPos, float headingDeg, Vector3 endPos, float driveDistance, out Vector3 t1, out Vector3 t2)
        {
            resultPoints.Clear();
            float endHeadingDeg = headingDeg;
            Vector3 startDir = new Vector3 { x = Sin(Deg2Rad * headingDeg), y = 0, z = Cos(Deg2Rad * headingDeg) };
            float r = DubinsMath.turningRadius;

            Vector3 lc = DubinsMath.GetLeftCircleCenterPos(startPos, headingDeg * Deg2Rad);
            Vector3 rc = DubinsMath.GetRightCircleCenterPos(startPos, headingDeg * Deg2Rad);

            float angle = Vector3.SignedAngle(startDir, endPos - startPos, Vector3.up);
            List<Vector3> arc = new();
            bool endIsToTheRight = angle >= 0;

            //right side
            if (endIsToTheRight)
            {
                (Vector3 p1, Vector3 p2) = FindTangentsanAlytically(rc, endPos, r);
                p1 += rc; p2 += rc;
                t1 = p1; t2 = p2;

                float arc1Length = GetArcLength(p1, startPos, r, rc, endIsToTheRight);
                float length1 = arc1Length + (endPos - p1).magnitude;

                float arc2Length = GetArcLength(p2, startPos, r, rc, endIsToTheRight);
                float length2 = arc2Length + (endPos - p2).magnitude;

                //if we go right we only need p1 which is always closest to start tangent for right side.
                //but due to analytical method of finding tangents it is possible for p1 to go behind start, which will make length1 > length2.
                //since we know that length1 has to be the shortest, we will draw nothing otherwise.
                //same logic works for left side.
                //this is not really obvious or intentional way to prevent drawing inside of start circle, but seems to work for now.
                if (length1 >= length2) return endHeadingDeg;    //this should not happen, dont draw anything

                arc = CalculateArcPoints(startPos, headingDeg, arc1Length, r, isRight: endIsToTheRight, driveDistance);
                endHeadingDeg = Vector3.SignedAngle(Vector3.forward, endPos - p1, Vector3.up);
            }
            //left side
            else
            {
                (Vector3 p1, Vector3 p2) = FindTangentsanAlytically(lc, endPos, r);
                p1 += lc; p2 += lc;
                t1 = p1; t2 = p2;

                float arc1Length = GetArcLength(p1, startPos, r, lc, endIsToTheRight);
                float length1 = arc1Length + (endPos - p1).magnitude;

                float arc2Length = GetArcLength(p2, startPos, r, lc, endIsToTheRight);
                float length2 = arc2Length + (endPos - p2).magnitude;

                if (length1 <= length2) return endHeadingDeg;    //this should not happen, dont draw anything

                arc = CalculateArcPoints(startPos, headingDeg, arc2Length, r, isRight: endIsToTheRight, driveDistance);
                endHeadingDeg = Vector3.SignedAngle(Vector3.forward, endPos - p2, Vector3.up);
            }

            List<Vector3> straight = CalculateStraightLine(arc[^1], endPos, driveDistance);
            if (straight.Count > 1) straight.RemoveAt(0);

            resultPoints.AddRange(arc);
            resultPoints.AddRange(straight);

            return endHeadingDeg;
        }

        private static float GetArcLength(Vector3 p, Vector3 startPos, float radius, Vector3 circleCenter, bool isRight)
        {
            Vector3 axis = isRight ? Vector3.up : Vector3.down;
            float arcAngleDeg = Vector3.SignedAngle(startPos - circleCenter, p - circleCenter, axis);
            if (arcAngleDeg < 0) arcAngleDeg += 360;
            return arcAngleDeg * Deg2Rad * radius;
        }

        private static (Vector3, Vector3) FindTangentsanAlytically(Vector3 center, Vector3 endPos, float r)
        {
            //Find tangents with analytic geometry: https://en.wikipedia.org/wiki/Tangent_lines_to_circles#With_analytic_geometry

            float x0 = (endPos - center).x;
            float y0 = (endPos - center).z;
            float d0Squared = x0 * x0 + y0 * y0;
            if (d0Squared < r * r) return (Vector3.zero, Vector3.zero);
            float k = r * r / d0Squared;
            float m = (r / d0Squared) * Mathf.Sqrt(d0Squared - r * r);

            Vector3 p1 = Vector3.zero;
            Vector3 p2 = Vector3.zero;

            p1.x += k * x0 - m * y0;
            p1.z += k * y0 + m * x0;

            p2.x += k * x0 + m * y0;
            p2.z += k * y0 - m * x0;

            return (p1, p2);
        }
    }
}