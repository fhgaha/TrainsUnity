using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class DrawingNoninitialSegment : RailBuilderState
    {
        public override RailBuilderState Handle(Camera camera)
        {
            HandleMouseMovement(camera);

            //on lmb save drawn segment to a rail container
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                rb.PutDrawnSegmentIntoContainer();

                //start is always snapped
                if (rb.DetectedStation != null)
                {
                    //TODO
                    rb.RegisterC(rb.Segment);
                }
                else if (rb.DetectedRoad != null)
                {
                    //snapped start, snapped end
                    if (rb.Segment.End == rb.DetectedRoad.Start || rb.Segment.End == rb.DetectedRoad.End)
                    {
                        rb.RegisterC(rb.Segment);
                    }
                    else
                    {
                        //ending to mid
                        rb.RegisterIT(rb.DetectedRoad, rb.Segment, rb.Segment.End);
                    }
                }
                else
                {
                    rb.RegisterII(rb.Segment.End, rb.Segment.Start);
                }

                //Debug.DrawRay(rb.start.pos, 20 * Vector3.up, Color.green, float.PositiveInfinity);
                //Debug.DrawRay(rb.end.pos, 20 * Vector3.up, Color.red, float.PositiveInfinity);

                rb.start = rb.end;
                rb.end = HeadedPoint.Empty;

                return this;
            }

            //on rmb cancel drawing
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                rb.RemoveMesh();
                return selectingStartState;
            }

            return this;
        }

        private void HandleMouseMovement(Camera camera)
        {
            if (!HitGround(camera, out RaycastHit hit)) return;
            if (mousePos == hit.point) return;
            //if (!ArePointsToCloseToDraw(rb.start.pos, hit.point)) return;

            if (rb.DetectedStation != null)
            {

                rb.end = GetSnappedEnd(new List<Vector3> { rb.DetectedStation.Entry1, rb.DetectedStation.Entry2 }, hit.point, rb.end.pos - rb.tangent1);
                rb.CalculateDubinsPoints();
            }
            else if (rb.DetectedRoad != null)
            {
                rb.end = GetSnappedEnd(rb.DetectedRoad.Points, hit.point, rb.end.pos - rb.tangent1);
                rb.CalculateDubinsPoints();
            }
            else
            {
                rb.CalculateCSPoints(rb.start.pos, rb.start.heading, hit.point);
            }
            mousePos = hit.point;
        }
    }
}
