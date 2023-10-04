﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class SelectingStart : RailBuilderState
    {
        public Vector3 SnappedStart { get; set; }
        public RoadSegment SnappedStartRoad { get; private set; }
        public List<Vector3> SnappedStartPoints { get; set; } = new();
        public bool IsStartSnapped => SnappedStart != Vector3.zero && SnappedStartRoad != null;

        //move mouse around, click lmb to start drawing
        public override RailBuilderState Handle(bool wasHit, Vector3 hitPoint, bool lmbPressed, bool rmdPressed)
        {
            if (!wasHit) return this;

            if (lmbPressed)
            {
                HandleLmbPresed(hitPoint);
                return drawingInitialSegmentState;
            }

            return this;
        }

        private void HandleLmbPresed(Vector3 hitPoint)
        {
            if (rb.DetectedStation != null)
            {
                rb.start.pos = GetClosestPoint(new List<Vector3> { rb.DetectedStation.Entry1, rb.DetectedStation.Entry2 }, hitPoint);
                SnappedStart = rb.start.pos;
                SnappedStartRoad = rb.DetectedStation.segment;
                SnappedStartPoints = new List<Vector3> { SnappedStartRoad.Start, SnappedStartRoad.End };
            }
            else if (rb.DetectedRoad != null)
            {
                rb.start.pos = GetClosestPoint(rb.DetectedRoad.Points, hitPoint);
                SnappedStart = rb.start.pos;
                SnappedStartRoad = rb.DetectedRoad;
                SnappedStartPoints = rb.DetectedRoad.Points.Select(p => p).ToList();
            }
            else
            {
                rb.start.pos = hitPoint;
                UnsnapStart();
            }
        }

        public void UnsnapStart()
        {
            SnappedStart = Vector3.zero;
            SnappedStartRoad = null;
            SnappedStartPoints.Clear();
        }
    }
}
