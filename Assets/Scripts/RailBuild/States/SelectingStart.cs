using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class SelectingStart : RailBuilderState
    {
        public Vector3 SnappedStart { get; set; }
        public RoadSegment SnappedStartRoad { get; set; }
        public bool IsStartSnapped => SnappedStart != Vector3.zero && SnappedStartRoad != null;

        //move mouse around, click lmb to start drawing
        public override RailBuilderState Handle(Camera camera)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (HitGround(camera, out RaycastHit hit))
                {
                    if (rb.DetectedRoad == null)
                    {
                        rb.start.pos = hit.point;
                        SnappedStart = Vector3.zero;
                        SnappedStartRoad = null;
                    }
                    else
                    {
                        //snap!
                        rb.start.pos = GetClosestPoint(rb.DetectedRoad.Points, hit.point);
                        SnappedStart = rb.start.pos;
                        SnappedStartRoad = rb.DetectedRoad;
                    }
                    return drawingInitialSegmentState;
                }
            }

            return this;
        }

        public void UnsnapStart()
        {
            SnappedStart = Vector3.zero;
            SnappedStartRoad = null;
        }
    }
}
