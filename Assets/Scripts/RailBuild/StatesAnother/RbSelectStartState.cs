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
            Debug.Log($"{rb.Parent?.GetType()} entered state {this.GetType()}");

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
            if (rb.DetectedStation != null)
            {
                rb.start.pos = machine.GetClosestPoint(new List<Vector3> { rb.DetectedStation.Entry1, rb.DetectedStation.Entry2 }, hitPoint);
                rb.SnappedStart = rb.start.pos;
                rb.SnappedStartRoad = rb.DetectedStation.segment;
                rb.SnappedStartPoints = new List<Vector3> { rb.SnappedStartRoad.Start, rb.SnappedStartRoad.End };
            }
            else if (rb.DetectedRoad != null)
            {
                rb.start.pos = machine.GetClosestPoint(rb.DetectedRoad.Points, hitPoint);
                rb.SnappedStart = rb.start.pos;
                rb.SnappedStartRoad = rb.DetectedRoad;
                rb.SnappedStartPoints = rb.DetectedRoad.Points.Select(p => p).ToList();
            }
            else
            {
                rb.start.pos = hitPoint;
                rb.UnsnapStart();
            }
        }
    }
}
