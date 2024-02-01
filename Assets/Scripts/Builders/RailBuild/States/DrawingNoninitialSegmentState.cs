using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class DrawingNoninitialSegmentState : RailBuilderState
    {
        public override RailBuilderState Handle(bool wasHit, Vector3 hitPoint, bool lmbPressed, bool rmbPressed)
        {
            HandleMouseMovement(wasHit, hitPoint);

            //on lmb save drawn segment to a rail container
            if (lmbPressed)
            {
                HandleLmbPressed();
                return BaseClass.drawingNoninitialSegmentState;
            }

            //on rmb cancel drawing
            if (rmbPressed)
            {
                rb.RemoveMesh();
                return BaseClass.selectingStartState;
            }

            return BaseClass.drawingNoninitialSegmentState;
        }

        private void HandleLmbPressed()
        {
            rb.PlaceSegment();

            //start is always snapped
            if (rb.DetectedStation != null)
            {
                //TODO
                regHelp.RegisterC(rb.Segment);
            }
            else if (rb.DetectedRoadByEnd != null)
            {
                //snapped start, snapped end
                //if (rb.Segment.End == rb.DetectedRoad.Start || rb.Segment.End == rb.DetectedRoad.End)
                if (rb.DetectedRoadByEnd.IsPointSnappedOnEnding(rb.Segment.End))
                {
                    regHelp.RegisterC(rb.Segment);
                }
                else
                {
                    //ending to mid
                    regHelp.RegisterIT(rb.DetectedRoadByEnd, rb.Segment, rb.Segment.End);
                }
            }
            else
            {
                regHelp.RegisterII(rb.Segment.End, rb.Segment.Start);
            }

            //Debug.DrawRay(rb.start.pos, 20 * Vector3.up, Color.green, float.PositiveInfinity);
            //Debug.DrawRay(rb.end.pos, 20 * Vector3.up, Color.red, float.PositiveInfinity);

            rb.start = rb.end;
            rb.end = HeadedPoint.Empty;
        }

        private void HandleMouseMovement(bool wasHit, Vector3 hitPoint)
        {
            if (!wasHit) return;
            if (mousePos == hitPoint) return;
            //if (!ArePointsToCloseToDraw(rb.start.pos, hit.point)) return;

            if (rb.DetectedStation != null)
            {
                rb.end = GetSnappedEnd(new List<Vector3> { rb.DetectedStation.Entry1, rb.DetectedStation.Entry2 }, hitPoint, rb.end.pos - rb.tangent1);
                rb.CalculateDubinsPoints();
            }
            else if (rb.DetectedRoadByEnd != null)
            {
                rb.end = GetSnappedEnd(rb.DetectedRoadByEnd.Points, hitPoint, rb.end.pos - rb.tangent1);
                rb.CalculateDubinsPoints();
            }
            else
            {
                rb.CalculateCSPoints(rb.start.pos, rb.start.heading, hitPoint);
            }
            mousePos = hitPoint;
        }
    }
}
