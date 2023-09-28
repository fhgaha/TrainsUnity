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
        public static void CalculateStraightLine(List<Vector3> pts, Vector3 startPos, Vector3 endPos, float driveDistance)
        {
            pts.Clear();
            float dist = (endPos - startPos).magnitude;
            //not doing this causes 'Look rotation viewing vector is zero' in OrientedPoint constructor
            if (dist < Epsilon) return;
            int segments = Mathf.FloorToInt(dist / driveDistance);
            Vector3 dir = (endPos - startPos).normalized;
            Vector3 cur = startPos;
            pts.Add(startPos);
            //we go one segement more so it overlaps a bit with the next segment, meshes will look better but i doubt trains will move smoothly
            for (int i = 0; i < segments; i++)
            //for (int i = 0; i < segments + 1; i++)
            {
                cur += driveDistance * dir;
                pts.Add(cur);
            }
            //first and last pts included and only once, no need to inject or add lsat point in the end     //false for simplest straight segment

            AddEndPos(pts, startPos, endPos);

            return;
        }

        public static void AddEndPos(List<Vector3> pts, Vector3 startPos, Vector3 endPos)
        {
            if (pts.Count < 2) return;
            if (startPos == endPos) return; //TODO why do i do this?

            //i want the last pt to be the same as endPos
            //if last and prelast pts are too close remove prelast pt
            pts.Add(endPos);
            if ((pts[^1] - pts[^2]).magnitude < Global.Instance.DriveDistance)
            {
                pts.RemoveAt(pts.Count - 2);
            }
        }

        //returns at least one point which is startPos
        public static List<Vector3> CalculateArcPoints(Vector3 startPos, float headingDeg, float arcLength, float radius, bool isTurningRight, float driveDistance)
        {
            float parameter = isTurningRight ? 1 : -1; //which side are we going on arc
            List<Vector3> pts = new() { startPos };
            var anglePar = isTurningRight ? -PI / 2 : PI / 2;
            float theta = headingDeg * Deg2Rad;
            int segments = FloorToInt(arcLength / driveDistance);
            var circleCenter = isTurningRight
                ? DubinsMath.GetRightCircleCenterPos(startPos, theta)
                : DubinsMath.GetLeftCircleCenterPos(startPos, theta);

            for (int i = 0; i < segments; i++)
            {
                Vector3 currentPos = Vector3.zero;
                currentPos.x = circleCenter.x + radius * Sin(theta + anglePar);
                currentPos.z = circleCenter.z + radius * Cos(theta + anglePar);
                theta += parameter * driveDistance / radius;
                pts.Add(currentPos);
            }

            //dirty fix in case i
            if (pts.Count > 2 && (pts[0] - pts[1]).magnitude < driveDistance)
            {
                pts.RemoveAt(1);
            }

            return pts;
        }

        public static List<Vector3> CalculateArcPoints_LessPrecise(Vector3 startPos, float headingDeg, float arcLength, float radius, bool isTurningRight, float driveDistance)
        {
            float parameter = isTurningRight ? 1 : -1; //which side are we going on arc
            List<Vector3> pts = new() { startPos };
            Vector3 currentPos = startPos;
            float theta = headingDeg * Deg2Rad;
            int segments = FloorToInt(arcLength / driveDistance);
            for (int i = 0; i < segments; i++)
            {
                currentPos.x += driveDistance * Sin(theta);
                currentPos.z += driveDistance * Cos(theta);
                theta += parameter * driveDistance / radius;
                pts.Add(currentPos);
            }
            return pts;
        }


        //CS is Curve + Straght
        public static float CalculateCSPoints(
        List<Vector3> resultPoints, Vector3 startPos, float headingDeg, Vector3 endPos, float driveDistance,
        out Vector3 tangent, out Vector3 debug_t1, out Vector3 debug_t2)
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
                debug_t1 = p1; debug_t2 = p2;

                float arc1Length = GetArcLength(p1, startPos, r, rc, endIsToTheRight);
                float length1 = arc1Length + (endPos - p1).magnitude;

                float arc2Length = GetArcLength(p2, startPos, r, rc, endIsToTheRight);
                float length2 = arc2Length + (endPos - p2).magnitude;

                //if we go right we only need p1 which is always closest to start tangent for right side.
                //but due to analytical method of finding tangents it is possible for p1 to go behind start, which will make length1 > length2.
                //since we know that length1 has to be the shortest, we will draw nothing otherwise.
                //same logic works for left side.
                //this is not really obvious or intentional way to prevent drawing inside of start circle, but seems to work for now.
                if (length1 >= length2) //this should not happen, dont draw anything
                {
                    tangent = p2;
                    return endHeadingDeg;
                }
                arc = CalculateArcPoints(startPos, headingDeg, arc1Length, r, isTurningRight: endIsToTheRight, driveDistance);
                endHeadingDeg = Vector3.SignedAngle(Vector3.forward, endPos - p1, Vector3.up);
                tangent = p1;
            }
            //left side
            else
            {
                (Vector3 p1, Vector3 p2) = FindTangentsanAlytically(lc, endPos, r);
                p1 += lc; p2 += lc;
                debug_t1 = p1; debug_t2 = p2;

                float arc1Length = GetArcLength(p1, startPos, r, lc, endIsToTheRight);
                float length1 = arc1Length + (endPos - p1).magnitude;

                float arc2Length = GetArcLength(p2, startPos, r, lc, endIsToTheRight);
                float length2 = arc2Length + (endPos - p2).magnitude;

                if (length1 <= length2) //this should not happen, dont draw anything
                {
                    tangent = p1;
                    return endHeadingDeg;
                }
                arc = CalculateArcPoints(startPos, headingDeg, arc2Length, r, isTurningRight: endIsToTheRight, driveDistance);
                endHeadingDeg = Vector3.SignedAngle(Vector3.forward, endPos - p2, Vector3.up);
                tangent = p2;
            }

            List<Vector3> straight = new();
            Vector3 first = arc.Count > 0 ? arc[^1] : startPos;
            CalculateStraightLine(straight, first, endPos, driveDistance);
            if (arc.Count > 0 && straight.Count > 1) straight.RemoveAt(0);

            resultPoints.AddRange(arc);
            resultPoints.AddRange(straight);

            return endHeadingDeg;
        }

        public static float GetArcLength(Vector3 pointOnArc, Vector3 startPos, float radius, Vector3 circleCenter, bool isRight)
        {
            var arcAngleDeg = GetArcAngleDeg(pointOnArc, startPos, radius, circleCenter, isRight);
            return arcAngleDeg * Deg2Rad * radius;
        }

        public static float GetArcAngleDeg(Vector3 pointOnArc, Vector3 startPos, float radius, Vector3 circleCenter, bool isRight)
        {
            float arcAngleDeg = GetArcAngleSignedDeg(pointOnArc, startPos, radius, circleCenter, isRight);
            if (arcAngleDeg < 0) arcAngleDeg += 360;
            return arcAngleDeg;
        }

        public static float GetArcAngleSignedDeg(Vector3 pointOnArc, Vector3 startPos, float radius, Vector3 circleCenter, bool isRight)
        {
            Vector3 axis = isRight ? Vector3.up : Vector3.down;
            float arcAngleDeg = Vector3.SignedAngle(startPos - circleCenter, pointOnArc - circleCenter, axis);
            return arcAngleDeg;
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
        public static float ClampToPlusMinus180DegreesRange(float degrees)
        {
            float result = degrees % 360;
            if (result >= 180) result -= 360;
            if (result < -180) result += 360;
            return result;
        }
    }
}