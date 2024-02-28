using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class RbSelectStartState : RbBaseState
    {
        private RailBuilder rb;

        public RbSelectStartState(RailBuilder rb) : base()
        {
            this.rb = rb;
        }

        public override void OnEnter(RbStateMachine machine)
        {
            //Debug.Log($"{rb.Parent?.GetType()} entered state {this.GetType()}");

            rb.start = HeadedPoint.Empty;
            rb.end = HeadedPoint.Empty;
            rb.UnsnapStart();
        }

        public override void OnExit(RbStateMachine machine)
        {
        }

        public override void UpdateState(RbStateMachine machine, bool wasHit, Vector3 hitPoint, bool lmbPressed, bool rmbPressed)
        {
            if (!wasHit) return;

            if (lmbPressed)
            {
                HandleLmbPresed(machine, hitPoint);
                machine.SwitchStateTo(machine.InitialSegmentState);
                return;
            }

            return;
        }

        private void HandleLmbPresed(RbStateMachine machine, Vector3 hitPoint)
        {
            if (rb.DetectedByEndStation != null)
            {
                rb.SnapStart(
                    newStartPos: machine.GetClosestPoint(new List<Vector3> { rb.DetectedByEndStation.Entry1, rb.DetectedByEndStation.Entry2 }, hitPoint),
                    snappedStartRoad: rb.DetectedByEndStation.Segment,
                    snappedStartPoints: new List<Vector3> { rb.SnappedStartRoad.Start, rb.SnappedStartRoad.End }
                );
            }
            else if (rb.DetectedByEndRoad != null)
            {
                rb.SnapStart(
                    newStartPos: machine.GetClosestPoint(rb.DetectedByEndRoad.Points, hitPoint),
                    snappedStartRoad: rb.DetectedByEndRoad,
                    snappedStartPoints: rb.DetectedByEndRoad.Points.Select(p => p).ToList()
                );
            }
            else
            {
                rb.start.pos = hitPoint;
                rb.UnsnapStart();
            }
        }

    }
}
