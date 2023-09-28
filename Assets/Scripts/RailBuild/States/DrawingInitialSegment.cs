using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class DrawingInitialSegment : RailBuilderState
    {
        public override RailBuilderState Handle(Camera camera)
        {
            //https://docs.unity3d.com/Manual/CollidersOverview.html
            //mesh collider cannot collide with another mesh collider(i.e., nothing happens when they make contact).
            //You can get around this in some cases by marking the mesh collider as Convex in the Inspector
            //This generates the collider shape as a “convex hull” which is like the original mesh but with any undercuts filled in.
            //However, a good rule is to use mesh colliders for scene
            //geometry and approximate the shape of moving GameObjects using compound primitive colliders.

            HandleMouseMovement(camera);

            // On LMB release, save drawn segment to a rail container
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                rb.PutDrawnSegmentIntoContainer();

                //register in route manager
                if (selectingStartState.IsStartSnapped)
                {
                    if (rb.DetectedStation != null)
                    {
                        RoadSegment startRoad = selectingStartState.SnappedStartRoad;
                        Vector3 start = rb.Segment.Start;

                        if (start == startRoad.Start || start == startRoad.End)
                        {
                            rb.RegisterC(rb.Segment);
                        }
                        else
                        {
                            rb.RegisterIT(roadMidConnected: startRoad, newRoad: rb.Segment, connection: start);
                        }
                    }
                    else if (rb.DetectedRoad != null)
                    {
                        HandleSnappedStartSnappedEnd();
                    }
                    else
                    {
                        HandleSnappedStartUnsnappedEnd();
                    }
                }
                else
                {
                    if (rb.DetectedStation != null)
                    {
                        rb.RegisterII(rb.Segment.Start, rb.Segment.End);
                    }
                    else if (rb.DetectedRoad != null)
                    {
                        HandleUnsnappedStartSnappedEnd();
                    }
                    else
                    {
                        HandleUnsnappedStartUnsnappedEnd();
                    }
                }

                rb.start = rb.end;
                rb.end = HeadedPoint.Empty;
                selectingStartState.UnsnapStart();
                return drawingNoninitialSegmentState;
            }

            void HandleSnappedStartSnappedEnd()
            {
                RoadSegment startRoad = selectingStartState.SnappedStartRoad;
                RoadSegment endRoad = rb.DetectedRoad;
                Vector3 start = rb.Segment.Start;
                Vector3 end = rb.Segment.End;

                switch (
                    (start == startRoad.Start || start == startRoad.End,
                     end == endRoad.Start || end == endRoad.End)
                )
                {
                    case (true, true):
                        rb.RegisterC(rb.Segment);
                        break;
                    case (false, true):
                        rb.RegisterIT(roadMidConnected: startRoad, newRoad: rb.Segment, connection: start);
                        break;
                    case (true, false):
                        rb.RegisterIT(roadMidConnected: endRoad, newRoad: rb.Segment, connection: end);
                        break;
                    case (false, false):
                        rb.RegisterH(selectingStartState.SnappedStartRoad, start, rb.DetectedRoad, end, rb.Segment);
                        break;
                }
            }

            void HandleSnappedStartUnsnappedEnd()
            {
                RoadSegment snappedRoad = selectingStartState.SnappedStartRoad;
                Vector3 start = rb.Segment.Start;
                Vector3 end = rb.Segment.End;
                if (start == snappedRoad.Start || start == snappedRoad.End)
                {
                    rb.RegisterII(end, start);
                }
                else
                {
                    rb.RegisterT(end, start, snappedRoad);
                }
            }

            void HandleUnsnappedStartSnappedEnd()
            {
                if (rb.Segment.End == rb.DetectedRoad.Start || rb.Segment.End == rb.DetectedRoad.End)
                {
                    rb.RegisterII(rb.Segment.Start, rb.Segment.End);
                }
                else
                {
                    // Unsnapped start, end snapped to mid
                    rb.RegisterT(rb.Segment.Start, rb.Segment.End, rb.DetectedRoad);
                }
            }

            void HandleUnsnappedStartUnsnappedEnd() => rb.RegisterI(rb.Segment.Start, rb.Segment.End);

            //on rmb cancel drawing
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                rb.RemoveMesh();
                selectingStartState.UnsnapStart();
                return selectingStartState;
            }

            return this;
        }

        private void HandleMouseMovement(Camera camera)
        {
            if (!HitGround(camera, out RaycastHit hit)) return;
            if (mousePos == hit.point) return;
            //if (!ArePointsToCloseToDraw(rb.start.pos, hit.point)) return;

            //if road is from station to station draw dubins

            if (selectingStartState.IsStartSnapped)
            {
                rb.start.heading = GetSnappedStartHeading(
                    selectingStartState.SnappedStartPoints,
                    selectingStartState.SnappedStart,
                    hit.point - selectingStartState.SnappedStart
                );

                if (rb.DetectedStation != null)
                {
                    rb.end = GetSnappedEnd(new List<Vector3> { rb.DetectedStation.Entry1, rb.DetectedStation.Entry2 }, hit.point, rb.end.pos - rb.start.pos);
                    rb.CalculateDubinsPoints();
                }
                else if (rb.DetectedRoad != null)
                {
                    rb.end = GetSnappedEnd(rb.DetectedRoad.Points, hit.point, rb.end.pos - rb.start.pos);
                    rb.CalculateDubinsPoints();
                }
                else
                {
                    rb.CalculateCSPoints(rb.start.pos, rb.start.heading, hit.point);
                }
            }
            else
            {
                if (rb.DetectedStation != null)
                {
                    rb.end = GetSnappedEnd(new List<Vector3> { rb.DetectedStation.Entry1, rb.DetectedStation.Entry2 }, hit.point, rb.end.pos - rb.start.pos);
                    rb.CalculateCSPointsReversed();
                }
                else if (rb.DetectedRoad != null)
                {
                    rb.end = GetSnappedEnd(rb.DetectedRoad.Points, hit.point, rb.end.pos - rb.start.pos);
                    rb.CalculateCSPointsReversed();
                }
                else
                {
                    rb.CalculateStraightPoints(rb.start.pos, hit.point);
                }
            }
            mousePos = hit.point;
        }
    }
}
