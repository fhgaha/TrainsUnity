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
                //rb.end = GetSnappedEnd(rb.DetectedRoad.Points, hit.point, rb.end.pos - rb.tangent1);
                rb.end = GetSnappedEnd(new List<Vector3> { rb.DetectedStation.Entry1, rb.DetectedStation.Entry2 }, hit.point, rb.end.pos - rb.tangent1);
                rb.CalculateDubinsPoints();
            }
            else

            if (rb.DetectedRoad != null)
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
