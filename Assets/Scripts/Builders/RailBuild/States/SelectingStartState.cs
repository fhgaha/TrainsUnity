using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class SelectingStartState : RailBuilderState
    {
        //move mouse around, click lmb to start drawing
        public override RailBuilderState Handle(bool wasHit, Vector3 hitPoint, bool lmbPressed, bool rmdPressed)
        {
            if (!wasHit) return this;

            if (lmbPressed)
            {
                HandleLmbPresed(hitPoint);
                return BaseClass.drawingInitialSegmentState;
            }

            return BaseClass.selectingStartState;
        }

        private void HandleLmbPresed(Vector3 hitPoint)
        {
            if (rb.DetectedStation != null)
            {
                rb.start.pos = GetClosestPoint(new List<Vector3> { rb.DetectedStation.Entry1, rb.DetectedStation.Entry2 }, hitPoint);
                rb.SnappedStart = rb.start.pos;
                rb.SnappedStartRoad = rb.DetectedStation.Segment;
                rb.SnappedStartPoints = new List<Vector3> { rb.SnappedStartRoad.Start, rb.SnappedStartRoad.End };
            }
            else if (rb.DetectedRoadByEnd != null)
            {
                rb.start.pos = GetClosestPoint(rb.DetectedRoadByEnd.Points, hitPoint);
                rb.SnappedStart = rb.start.pos;
                rb.SnappedStartRoad = rb.DetectedRoadByEnd;
                rb.SnappedStartPoints = rb.DetectedRoadByEnd.Points.Select(p => p).ToList();
            }
            else
            {
                rb.start.pos = hitPoint;
                rb.UnsnapStart();
            }
        }
    }
}
