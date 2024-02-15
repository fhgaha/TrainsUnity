using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class RbInitialSegmentState : RbBaseState
    {
        private RailBuilder rb;
        private RegisterHelper regHelp;
        private RbStateMachine machine;
        private Vector3 mousePos = Vector3.positiveInfinity;

        public RbInitialSegmentState(RailBuilder rb, RegisterHelper regHelp, RbStateMachine machine) : base()
        {
            this.rb = rb;
            this.regHelp = regHelp;
            this.machine = machine;
        }

        public override void OnEnter(RbStateMachine machine)
        {
            //Debug.Log($"{rb.Parent?.GetType()} entered state {this.GetType()}");
            //rb = machine.Rb;
            //regHelp = machine.RegHelp;
            //this.machine = machine;
        }

        public override void OnExit(RbStateMachine machine)
        {
            mousePos = Vector3.positiveInfinity;    //in HandleMouseMovement we check previous mouse pos, so we need to reset it
        }

        public override void UpdateState(RbStateMachine machine, bool wasHit, Vector3 hitPoint, bool lmbPressed, bool rmbPressed)
        {
            //https://docs.unity3d.com/Manual/CollidersOverview.html
            //mesh collider cannot collide with another mesh collider(i.e., nothing happens when they make contact).
            //You can get around this in some cases by marking the mesh collider as Convex in the Inspector
            //This generates the collider shape as a “convex hull” which is like the original mesh but with any undercuts filled in.
            //However, a good rule is to use mesh colliders for scene
            //geometry and approximate the shape of moving GameObjects using compound primitive colliders.

            HandleMouseMovement(wasHit, hitPoint);

            // On LMB release, save drawn segment to a rail container
            if (lmbPressed)
            {
                HandleLmbPressed();
                machine.SwitchStateTo(machine.NoninitialSegmentState);
                return;
            }

            //on rmb cancel drawing
            if (rmbPressed)
            {
                rb.RemoveMesh();
                rb.UnsnapStart();
                machine.SwitchStateTo(machine.SelectStartState);
                return;
            }
        }

        private void HandleMouseMovement(bool wasHit, Vector3 hitPoint)
        {
            if (!wasHit) return;
            if (mousePos == hitPoint) return;

            if (rb.IsStartSnapped)
            {
                rb.start.heading = machine.GetSnappedStartHeading(
                    rb.SnappedStartPoints,
                    rb.SnappedStart,
                    hitPoint - rb.SnappedStart
                );

                if (rb.DetectedByEndStation != null)
                {
                    rb.end = machine.GetSnappedEnd(new List<Vector3> { rb.DetectedByEndStation.Entry1, rb.DetectedByEndStation.Entry2 }, hitPoint, rb.end.pos - rb.start.pos);
                    rb.CalculateDubinsPoints();
                }
                else if (rb.DetectedByEndRoad != null)
                {
                    rb.end = machine.GetSnappedEnd(rb.DetectedByEndRoad.Points, hitPoint, rb.end.pos - rb.start.pos);
                    rb.CalculateDubinsPoints();
                }
                else
                {
                    rb.CalculateCurveWithStriaghtPoints(rb.start.pos, rb.start.heading, hitPoint);
                }
            }
            else
            {
                if (rb.DetectedByEndStation != null)
                {
                    rb.end = machine.GetSnappedEnd(new List<Vector3> { rb.DetectedByEndStation.Entry1, rb.DetectedByEndStation.Entry2 }, hitPoint, rb.end.pos - rb.start.pos);
                    rb.CalculateCurveWithStriaghtPointsReversed();
                }
                else if (rb.DetectedByEndRoad != null)   //detected road is null why
                {
                    rb.end = machine.GetSnappedEnd(rb.DetectedByEndRoad.Points, hitPoint, rb.end.pos - rb.start.pos);
                    rb.CalculateCurveWithStriaghtPointsReversed();
                }
                else
                {
                    rb.CalculateStraightPoints(rb.start.pos, hitPoint);
                }
            }
            mousePos = hitPoint;
        }

        private void HandleLmbPressed()
        {
            if (rb.Points.Count == 0) return;

            rb.PlaceSegment();

            //register in route manager
            if (rb.IsStartSnapped)
            {
                if (rb.DetectedByEndStation != null)
                {
                    RoadSegment startRoad = rb.SnappedStartRoad;
                    Vector3 start = rb.Segment.Start;

                    //if (start == startRoad.Start || start == startRoad.End)
                    if (startRoad.IsPointSnappedOnEnding(start))
                    {
                        regHelp.RegisterC(rb.Segment);
                    }
                    else
                    {
                        regHelp.RegisterIT(roadMidConnected: startRoad, newRoad: rb.Segment, connection: start);
                    }
                }
                else if (rb.DetectedByEndRoad != null)
                {
                    //Debug.Log("HandleSnappedStartSnappedEnd();");
                    HandleSnappedStartSnappedEnd();
                }
                else
                {
                    //Debug.Log("HandleSnappedStartUnsnappedEnd();");
                    HandleSnappedStartUnsnappedEnd();
                }
            }
            else
            {
                if (rb.DetectedByEndStation != null)
                {
                    //regHelp.RegisterII(rb.Segment.Start, rb.Segment.End);
                    regHelp.RegisterC(rb.Segment.Start, rb.Segment.End, rb.Segment.GetApproxLength());
                }
                else if (rb.DetectedByEndRoad != null)
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
            rb.UnsnapStart();
        }

        private void HandleSnappedStartSnappedEnd()
        {
            RoadSegment startRoad = rb.SnappedStartRoad;
            RoadSegment endRoad = rb.DetectedByEndRoad;
            Vector3 start = rb.Segment.Start;
            Vector3 end = rb.Segment.End;

            switch (startRoad.IsPointSnappedOnEnding(start), endRoad.IsPointSnappedOnEnding(end))
            {
                case (true, true):
                    regHelp.RegisterC(rb.Segment);
                    break;
                case (false, true):
                    regHelp.RegisterIT(roadMidConnected: startRoad, newRoad: rb.Segment, connection: start);
                    break;
                case (true, false):
                    regHelp.RegisterIT(roadMidConnected: endRoad, newRoad: rb.Segment, connection: end);
                    break;
                case (false, false):
                    regHelp.RegisterH(startRoad, start, endRoad, end, rb.Segment);
                    break;
            }
        }

        private void HandleUnsnappedStartUnsnappedEnd() => regHelp.RegisterI(rb.Segment.Start, rb.Segment.End);

        private void HandleUnsnappedStartSnappedEnd()
        {
            //if (rb.Segment.End == rb.DetectedRoad.Start || rb.Segment.End == rb.DetectedRoad.End)
            if (rb.DetectedByEndRoad.IsPointSnappedOnEnding(rb.Segment.End))
            {
                regHelp.RegisterII(rb.Segment.Start, rb.Segment.End);
            }
            else
            {
                // Unsnapped start, end snapped to mid
                regHelp.RegisterT(rb.Segment.Start, rb.Segment.End, rb.DetectedByEndRoad, rb.Segment.Points);
            }
        }

        private void HandleSnappedStartUnsnappedEnd()
        {
            RoadSegment snappedRoad = rb.SnappedStartRoad;
            Vector3 start = rb.Segment.Start;
            Vector3 end = rb.Segment.End;
            //if (start == snappedRoad.Start || start == snappedRoad.End)
            if (snappedRoad.IsPointSnappedOnEnding(start))
            {
                regHelp.RegisterII(end, start);
            }
            else
            {
                //TODO why you not sending whole rb.Segment?
                regHelp.RegisterT(end, start, snappedRoad, rb.Segment.Points);
            }
        }
    }
}
