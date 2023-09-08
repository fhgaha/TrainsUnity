using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class RailBuilderState
    {
        //road is built segment by segemnt one after another. each time you select start position first, then you select second position of the first segment.
        //that is initial state. after that you do start = end and continue placing segments by selecting next point. that is non inintial state.
        //initial segment can start from snapped point (from point of another segemnt), or from freely selected point on a ground.
        //non inintal segment is always starts from snapped point.
        //you can snap last point of a segment to another segment or put it freely somewhere on the ground.
        public static SelectingStart selectingStartState;
        public static DrawingInitialSegment drawingInitialSegmentState;
        public static DrawingNoninitialSegment drawingNoninitialSegmentState;
        protected static RailBuilder rb;
        protected static Vector3 mousePos;

        public static RailBuilderState Configure(RailBuilder rb)
        {
            selectingStartState = new();
            drawingInitialSegmentState = new();
            drawingNoninitialSegmentState = new();
            RailBuilderState.rb = rb;
            return selectingStartState;
        }

        public virtual RailBuilderState Handle(Camera camera) => this;
        protected bool HitGround(Camera camera, out RaycastHit hit) =>
            Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 1000f, LayerMask.GetMask("Ground"));

        private float minDistToDraw = 5;
        protected bool ArePointsToCloseToDraw(Vector3 from, Vector3 to) => (to - from).magnitude > minDistToDraw;

        protected static HeadedPoint GetSnappedEnd(List<Vector3> pts, Vector3 mousePos, Vector3 dir)
        {
            Vector3 closest = GetClosestPoint(pts, mousePos);
            float heading;
            int index = pts.IndexOf(closest);

            if (index == 0)
            {
                heading = Vector3.SignedAngle(Vector3.forward, pts[1] - closest, Vector3.up);
            }
            else if (index == pts.Count - 1)
            {
                heading = Vector3.SignedAngle(Vector3.forward, pts[^2] - closest, Vector3.up);
            }
            else
            {
                Vector3 dir1 = pts[index - 1] - closest;
                Vector3 dir2 = pts[index + 1] - closest;
                float a1 = Vector3.Angle(dir, dir1);
                float a2 = Vector3.Angle(dir, dir2);
                Vector3 snappedDir = a1 * a1 <= a2 * a2 ? dir1 : dir2;

                //Debug.Log($"a1: {a1}, a2: {a2}");

                heading = Vector3.SignedAngle(Vector3.forward, snappedDir, Vector3.up);
            }
            return new HeadedPoint(closest, heading);
        }

        //TODO: pass closest point instead of mousePos, dont calculate it again here. Tried, Look rotation error occur.
        public float GetSnappedStartHeading(List<Vector3> pts, Vector3 mousePos, Vector3 dir)
        {
            Vector3 closest = GetClosestPoint(pts, mousePos);
            float heading;
            int index = pts.IndexOf(closest);

            if (index == 0)
            {
                heading = Vector3.SignedAngle(Vector3.forward, closest - pts[1], Vector3.up);
            }
            else if (index == pts.Count - 1)
            {
                heading = Vector3.SignedAngle(Vector3.forward, closest - pts[^2], Vector3.up);
            }
            else
            {
                Vector3 dir1 = pts[index - 1] - closest;
                Vector3 dir2 = pts[index + 1] - closest;
                float a1 = Vector3.Angle(dir, dir1);
                float a2 = Vector3.Angle(dir, dir2);
                Vector3 snappedDir = a1 * a1 <= a2 * a2 ? dir1 : dir2;

                heading = Vector3.SignedAngle(Vector3.forward, snappedDir, Vector3.up);
            }
            return heading;
        }

        public static Vector3 GetClosestPoint(List<Vector3> pts, Vector3 mousePos) =>
            pts.Aggregate((clst, next) => (next - mousePos).magnitude < (clst - mousePos).magnitude ? next : clst);
    }
}
