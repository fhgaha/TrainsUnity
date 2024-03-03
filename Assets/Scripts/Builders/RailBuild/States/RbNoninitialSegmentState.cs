using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class RbNoninitialSegmentState : RbBaseState
    {
        private RailBuilder rb;
        private RegisterHelper regHelp;
        private RbStateMachine machine;
        private Vector3 mousePos = Vector3.positiveInfinity;

        public RbNoninitialSegmentState(RailBuilder rb, RegisterHelper regHelp, RbStateMachine machine) : base()
        {
            this.rb = rb;
            this.regHelp = regHelp;
            this.machine = machine;
        }

        public override void OnEnter(RbStateMachine machine)
        {
        }

        public override void OnExit(RbStateMachine machine)
        {
            mousePos = Vector3.positiveInfinity;
        }

        public override void UpdateState(RbStateMachine machine, bool wasHit, Vector3 hitPoint, bool lmbPressed, bool rmbPressed)
        {
            HandleMouseMovement(machine, wasHit, hitPoint);

            //on lmb save drawn segment to a rail container
            if (lmbPressed)
            {
                HandleLmbPressed(machine, hitPoint);
                return;
            }

            //on rmb cancel drawing
            if (rmbPressed)
            {
                rb.RemoveMesh();
                machine.SwitchStateTo(machine.SelectStartState);
                return;
            }
        }

        private void HandleMouseMovement(RbStateMachine machine, bool wasHit, Vector3 hitPoint)
        {
            if (!wasHit) return;
            //if (mousePos == hitPoint) return;

            if (rb.DetectedByEndStation != null)
            {
                rb.end = machine.GetSnappedEnd(new List<Vector3> { rb.DetectedByEndStation.Entry1, rb.DetectedByEndStation.Entry2 }, hitPoint, rb.end.pos - rb.tangent1);
                rb.CalcDrawDubinsPoints();
            }
            else if (rb.DetectedByEndRoad != null)
            {
                rb.end = machine.GetSnappedEnd(rb.DetectedByEndRoad.Points, hitPoint, rb.end.pos - rb.tangent1);
                rb.CalcDrawDubinsPoints();
            }
            else
            {
                rb.CalculateCurveWithStriaghtPoints(rb.start.pos, rb.start.heading, hitPoint);
            }
            mousePos = hitPoint;
        }

        private void HandleLmbPressed(RbStateMachine machine, Vector3 hitPoint)
        {
            if (rb.Points.Count == 0) return;
            if (!rb.AllowedToBuild) return;

            RoadSegment copy = rb.PlaceSegment();

            //start is always snapped
            if (rb.DetectedByEndStation != null)
            {
                regHelp.RegisterC(rb.Segment);
            }
            else if (rb.DetectedByEndRoad != null)
            {
                //snapped start, snapped end
                if (rb.DetectedByEndRoad.IsPointSnappedOnEnding(rb.Segment.End))
                {
                    regHelp.RegisterC(rb.Segment);
                }
                else
                {
                    //ending to mid
                    regHelp.RegisterIT(rb.DetectedByEndRoad, rb.Segment, rb.Segment.End);
                }
            }
            else
            {
                regHelp.RegisterII(rb.Segment.End, rb.Segment.Start);

                rb.SnapStart(
                    newStartPos: rb.end.pos,
                    snappedStartRoad: copy,
                    snappedStartPoints: copy.Points.Select(p => p).ToList()
                );
            }

            rb.start = rb.end;
            rb.end = HeadedPoint.Empty;
        }


    }
}
