using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Trains
{
    //Generates Dubins paths
    public class DubinsGeneratePaths
    {
        //The 4 different circles we have that sits to the left/right of the start/goal
        //Public so we can position the circle objects for debugging
        public Vector3 startLeftCircle;
        public Vector3 startRightCircle;
        public Vector3 goalLeftCircle;
        public Vector3 goalRightCircle;

        //To generate paths we need the position and rotation (heading) of the cars
        Vector3 startPos;
        Vector3 goalPos;
        //Heading is in radians
        float startHeadingRad;
        float goalHeadingRad;

        //Where we store all path data so we can sort and find the shortest path
        List<OneDubinsPath> pathDataList = new List<OneDubinsPath>();

        public List<OneDubinsPath> GetAllDubinsPaths_UseDegrees(Vector3 startPos, float startHeadingDeg, Vector3 goalPos, float goalHeadingDeg)
        {
            //if (startHeadingDeg < 0) startHeadingDeg += 360;
            //if (goalHeadingDeg < 0) goalHeadingDeg += 360;
            return GetAllDubinsPaths(startPos, startHeadingDeg * Mathf.Deg2Rad, goalPos, goalHeadingDeg * Mathf.Deg2Rad);
        }

        //Get all valid Dubins paths sorted from shortest to longest
        public List<OneDubinsPath> GetAllDubinsPaths(Vector3 startPos, float startHeadingRad, Vector3 goalPos, float goalHeadingRad)
        {
            this.startPos = startPos;
            this.goalPos = goalPos;
            this.startHeadingRad = startHeadingRad;
            this.goalHeadingRad = goalHeadingRad;

            //Reset the list with all Dubins paths
            pathDataList.Clear();

            //Position the circles that are to the left/right of the cars
            PositionLeftRightCircles();

            //Find the length of each path with tangent coordinates
            CalculateDubinsPathsLengths();

            //If we have paths
            if (pathDataList.Count > 0)
            {
                //Sort the list with paths so the shortest path is first
                pathDataList.Sort((x, y) => x.totalLength.CompareTo(y.totalLength));

                //Generate the final coordinates of the path from tangent points and segment lengths
                GeneratePathCoordinates();

                return pathDataList;
            }

            //No paths could be found
            return null;
        }


        //Position the left and right circles that are to the left/right of the target and the car
        void PositionLeftRightCircles()
        {
            //Goal pos
            goalRightCircle = DubinsMath.GetRightCircleCenterPos(goalPos, goalHeadingRad);

            goalLeftCircle = DubinsMath.GetLeftCircleCenterPos(goalPos, goalHeadingRad);


            //Start pos
            startRightCircle = DubinsMath.GetRightCircleCenterPos(startPos, startHeadingRad);

            startLeftCircle = DubinsMath.GetLeftCircleCenterPos(startPos, startHeadingRad);
        }


        //
        //Calculate the path lengths of all Dubins paths by using tangent points
        //
        void CalculateDubinsPathsLengths()
        {
            //RSR and LSL is only working if the circles don't have the same position

            //RSR
            if (!(Mathf.Approximately(startRightCircle.x, goalRightCircle.x) && Mathf.Approximately(startRightCircle.z, goalRightCircle.z)))
            {
                Get_RSR_Length();
            }

            //LSL
            if (!(Mathf.Approximately(startLeftCircle.x, goalLeftCircle.x) && !Mathf.Approximately(startLeftCircle.z, goalLeftCircle.z)))
            {
                Get_LSL_Length();
            }

            //RSL and LSR is only working of the circles don't intersect
            float comparisonSqr = DubinsMath.turningRadius * 2f * DubinsMath.turningRadius * 2f;

            //RSL
            if ((startRightCircle - goalLeftCircle).sqrMagnitude > comparisonSqr)
            {
                Get_RSL_Length();
            }

            //LSR
            if ((startLeftCircle - goalRightCircle).sqrMagnitude > comparisonSqr)
            {
                Get_LSR_Length();
            }


            //With the LRL and RLR paths, the distance between the circles have to be less than 4 * r
            comparisonSqr = 4f * DubinsMath.turningRadius * 4f * DubinsMath.turningRadius;

            //RLR        
            if ((startRightCircle - goalRightCircle).sqrMagnitude < comparisonSqr)
            {
                Get_RLR_Length();
            }

            //LRL
            if ((startLeftCircle - goalLeftCircle).sqrMagnitude < comparisonSqr)
            {
                Get_LRL_Length();
            }
        }


        //RSR
        void Get_RSR_Length()
        {
            //Find both tangent positons
            Vector3 startTangent = Vector3.zero;
            Vector3 goalTangent = Vector3.zero;

            DubinsMath.LSLorRSR(startRightCircle, goalRightCircle, false, out startTangent, out goalTangent);

            //Calculate lengths
            float length1 = DubinsMath.GetArcLength(startRightCircle, startPos, startTangent, false);

            float length2 = (startTangent - goalTangent).magnitude;

            float length3 = DubinsMath.GetArcLength(goalRightCircle, goalTangent, goalPos, false);

            //Save the data
            OneDubinsPath pathData = new OneDubinsPath(length1, length2, length3, startTangent, goalTangent, PathType.RSR);

            //We also need this data to simplify when generating the final path
            pathData.segment2Turning = false;

            //RSR
            pathData.SetIfTurningRight(true, false, true);

            //Add the path to the collection of all paths
            pathDataList.Add(pathData);
        }


        //LSL
        void Get_LSL_Length()
        {
            //Find both tangent positions
            Vector3 startTangent = Vector3.zero;
            Vector3 goalTangent = Vector3.zero;

            DubinsMath.LSLorRSR(startLeftCircle, goalLeftCircle, true, out startTangent, out goalTangent);

            //Calculate lengths
            float length1 = DubinsMath.GetArcLength(startLeftCircle, startPos, startTangent, true);

            float length2 = (startTangent - goalTangent).magnitude;

            float length3 = DubinsMath.GetArcLength(goalLeftCircle, goalTangent, goalPos, true);

            //Save the data
            OneDubinsPath pathData = new OneDubinsPath(length1, length2, length3, startTangent, goalTangent, PathType.LSL);

            //We also need this data to simplify when generating the final path
            pathData.segment2Turning = false;

            //LSL
            pathData.SetIfTurningRight(false, false, false);

            //Add the path to the collection of all paths
            pathDataList.Add(pathData);
        }


        //RSL
        void Get_RSL_Length()
        {
            //Find both tangent positions
            Vector3 startTangent = Vector3.zero;
            Vector3 goalTangent = Vector3.zero;

            DubinsMath.RSLorLSR(startRightCircle, goalLeftCircle, false, out startTangent, out goalTangent);

            //Calculate lengths
            float length1 = DubinsMath.GetArcLength(startRightCircle, startPos, startTangent, false);

            float length2 = (startTangent - goalTangent).magnitude;

            float length3 = DubinsMath.GetArcLength(goalLeftCircle, goalTangent, goalPos, true);

            //Save the data
            OneDubinsPath pathData = new OneDubinsPath(length1, length2, length3, startTangent, goalTangent, PathType.RSL);

            //We also need this data to simplify when generating the final path
            pathData.segment2Turning = false;

            //RSL
            pathData.SetIfTurningRight(true, false, false);

            //Add the path to the collection of all paths
            pathDataList.Add(pathData);
        }


        //LSR
        void Get_LSR_Length()
        {
            //Find both tangent positions
            Vector3 startTangent = Vector3.zero;
            Vector3 goalTangent = Vector3.zero;

            DubinsMath.RSLorLSR(startLeftCircle, goalRightCircle, true, out startTangent, out goalTangent);

            //Calculate lengths
            float length1 = DubinsMath.GetArcLength(startLeftCircle, startPos, startTangent, true);

            float length2 = (startTangent - goalTangent).magnitude;

            float length3 = DubinsMath.GetArcLength(goalRightCircle, goalTangent, goalPos, false);

            //Save the data
            OneDubinsPath pathData = new OneDubinsPath(length1, length2, length3, startTangent, goalTangent, PathType.LSR);

            //We also need this data to simplify when generating the final path
            pathData.segment2Turning = false;

            //LSR
            pathData.SetIfTurningRight(false, false, true);

            //Add the path to the collection of all paths
            pathDataList.Add(pathData);
        }


        //RLR
        void Get_RLR_Length()
        {
            //Find both tangent positions and the position of the 3rd circle
            Vector3 startTangent = Vector3.zero;
            Vector3 goalTangent = Vector3.zero;
            //Center of the 3rd circle
            Vector3 middleCircle = Vector3.zero;

            DubinsMath.GetRLRorLRLTangents(
                startRightCircle,
                goalRightCircle,
                false,
                out startTangent,
                out goalTangent,
                out middleCircle);

            //Calculate lengths
            float length1 = DubinsMath.GetArcLength(startRightCircle, startPos, startTangent, false);

            float length2 = DubinsMath.GetArcLength(middleCircle, startTangent, goalTangent, true);

            float length3 = DubinsMath.GetArcLength(goalRightCircle, goalTangent, goalPos, false);

            //Save the data
            OneDubinsPath pathData = new OneDubinsPath(length1, length2, length3, startTangent, goalTangent, PathType.RLR);

            //We also need this data to simplify when generating the final path
            pathData.segment2Turning = true;

            //RLR
            pathData.SetIfTurningRight(true, false, true);

            //Add the path to the collection of all paths
            pathDataList.Add(pathData);
        }


        //LRL
        void Get_LRL_Length()
        {
            //Find both tangent positions and the position of the 3rd circle
            Vector3 startTangent = Vector3.zero;
            Vector3 goalTangent = Vector3.zero;
            //Center of the 3rd circle
            Vector3 middleCircle = Vector3.zero;

            DubinsMath.GetRLRorLRLTangents(
                startLeftCircle,
                goalLeftCircle,
                true,
                out startTangent,
                out goalTangent,
                out middleCircle);

            //Calculate the total length of this path
            float length1 = DubinsMath.GetArcLength(startLeftCircle, startPos, startTangent, true);

            float length2 = DubinsMath.GetArcLength(middleCircle, startTangent, goalTangent, false);

            float length3 = DubinsMath.GetArcLength(goalLeftCircle, goalTangent, goalPos, true);

            //Save the data
            OneDubinsPath pathData = new OneDubinsPath(length1, length2, length3, startTangent, goalTangent, PathType.LRL);

            //We also need this data to simplify when generating the final path
            pathData.segment2Turning = true;

            //LRL
            pathData.SetIfTurningRight(false, true, false);

            //Add the path to the collection of all paths
            pathDataList.Add(pathData);
        }


        //
        // Generate the final path from the tangent points
        //

        //When we have found the tangent points and lengths of each path we need to get the individual coordinates
        //of the entire path so we can travel along the path
        void GeneratePathCoordinates()
        {
            for (int i = 0; i < pathDataList.Count; i++)
            {
                MyGetTotalPath(pathDataList[i]);
                //GetTotalPath(pathDataList[i]);
            }
        }


        //Find the coordinates of the entire path from the 2 tangents and length of each segment
        void GetTotalPath(OneDubinsPath pathData)
        {
            //Store the waypoints of the final path here
            List<Vector3> finalPath = new List<Vector3>();

            //Start position of the car
            Vector3 currentPos = startPos;
            //Start heading of the car
            float theta = startHeadingRad;

            //We always have to add the first position manually = the position of the car
            finalPath.Add(currentPos);

            //How many line segments can we fit into this part of the path
            int segments = 0;

            //First
            segments = Mathf.FloorToInt(pathData.length1 / DubinsMath.driveDistance);

            DubinsMath.AddCoordinatesToPath(
                ref currentPos,
                ref theta,
                finalPath,
                segments,
                true,
                pathData.segment1TurningRight);

            //Second
            segments = Mathf.FloorToInt(pathData.length2 / DubinsMath.driveDistance);

            DubinsMath.AddCoordinatesToPath(
                ref currentPos,
                ref theta,
                finalPath,
                segments,
                pathData.segment2Turning,
                pathData.segment2TurningRight);

            //Third
            segments = Mathf.FloorToInt(pathData.length3 / DubinsMath.driveDistance);

            DubinsMath.AddCoordinatesToPath(
                ref currentPos,
                ref theta,
                finalPath,
                segments,
                true,
                pathData.segment3TurningRight);

            //Add the final goal coordinate
            finalPath.Add(new Vector3(goalPos.x, currentPos.y, goalPos.z));

            //Save the final path in the path data
            pathData.pathCoordinates = finalPath;
        }


        void MyGetTotalPath(OneDubinsPath pathData)
        {
            List<Vector3> finalPath = new();
            float thetaRad = startHeadingRad;

            //first
            List<Vector3> first = MyMath.CalculateArcPoints(
                startPos: startPos,
                headingDeg: thetaRad * Mathf.Rad2Deg,
                arcLength: pathData.length1,
                radius: DubinsMath.turningRadius,
                isTurningRight: pathData.segment1TurningRight,
                driveDistance: DubinsMath.driveDistance);

            Vector3 circlePos = pathData.segment1TurningRight
                ? DubinsMath.GetRightCircleCenterPos(startPos, startHeadingRad)
                : DubinsMath.GetLeftCircleCenterPos(startPos, startHeadingRad);

            thetaRad += Vector3.SignedAngle(startPos - circlePos, pathData.tangent1 - circlePos, Vector3.up) * Mathf.Deg2Rad;

            //second
            List<Vector3> second = new();
            if (pathData.segment2Turning)
            {
                second = MyMath.CalculateArcPoints(
                    startPos: pathData.tangent1,
                    headingDeg: thetaRad * Mathf.Rad2Deg,
                    arcLength: pathData.length2,
                    radius: DubinsMath.turningRadius,
                    isTurningRight: pathData.segment2TurningRight,
                    driveDistance: DubinsMath.driveDistance);

                circlePos = pathData.segment2TurningRight
                    ? DubinsMath.GetRightCircleCenterPos(pathData.tangent1, thetaRad)
                    : DubinsMath.GetLeftCircleCenterPos(pathData.tangent1, thetaRad);

                thetaRad += Vector3.SignedAngle(pathData.tangent1 - circlePos, pathData.tangent2 - circlePos, Vector3.up) * Mathf.Deg2Rad;
            }
            else
            {
                MyMath.CalculateStraightLine(second, pathData.tangent1, pathData.tangent2, DubinsMath.driveDistance);

                if (second.Count >= 2)
                {
                    second.RemoveAt(0);
                    second.RemoveAt(second.Count - 1);
                }
            }


            //third
            List<Vector3> third = MyMath.CalculateArcPoints(
                startPos: pathData.tangent2,
                headingDeg: thetaRad * Mathf.Rad2Deg,
                arcLength: pathData.length3,
                radius: DubinsMath.turningRadius,
                isTurningRight: pathData.segment3TurningRight,
                driveDistance: DubinsMath.driveDistance);


            finalPath.AddRange(first);
            finalPath.AddRange(second);
            finalPath.AddRange(third);

            pathData.pathCoordinates = finalPath;
        }
    }
}