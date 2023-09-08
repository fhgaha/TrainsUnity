﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class DrawingInitialSegment : RailBuilderState
    {
        public override RailBuilderState Handle(Camera camera)
        {
            //https://docs.unity3d.com/Manual/CollidersOverview.html
            //mesh collider cannot collide with another mesh collider(i.e., nothing happens when they make contact).
            //You can get around this in some cases by marking the mesh collider as Convex in the Inspector
            //This generates the collider shape as a “convex hull” which is like the original mesh but with any undercuts filled in.
            //However, a good rule is to use mesh colliders for scene
            //geometry and approximate the shape of moving GameObjects using compound primitive colliders.

            HandleMouseMovement(camera);

            //on lmb save drawn segment to a rail container
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                rb.PutDrawnSegmentIntoContainer();
                rb.start = rb.end;
                rb.end = HeadedPoint.Empty;
                selectingStartState.UnsnapStart();
                return drawingNoninitialSegmentState;
            }

            //on rmb cancel drawing
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                rb.RemoveMesh();
                selectingStartState.UnsnapStart();
                return selectingStartState;
            }

            return this;
        }

        private void HandleMouseMovement(Camera camera)
        {
            if (!HitGround(camera, out RaycastHit hit)) return;
            if (mousePos == hit.point) return;
            //if (!ArePointsToCloseToDraw(rb.start.pos, hit.point)) return;

            if (selectingStartState.IsStartSnapped)
            {
                rb.start.heading = GetSnappedStartHeading(selectingStartState.SnappedStartRoad.Points, hit.point, hit.point - selectingStartState.SnappedStart);
                
                if (rb.DetectedRoad == null)
                {
                    rb.CalculateCSPoints(rb.start.pos, rb.start.heading, hit.point);
                }
                else
                {
                    rb.end = GetSnappedEnd(rb.DetectedRoad.Points, hit.point, rb.end.pos - rb.start.pos);
                    rb.CalculateDubinsPoints();
                }
            }
            else
            {
                if (rb.DetectedRoad == null)
                {
                    rb.CalculateStraightPoints(rb.start.pos, hit.point);
                }
                else
                {
                    rb.end = GetSnappedEnd(rb.DetectedRoad.Points, hit.point, rb.end.pos - rb.start.pos);
                    rb.CalculateCSPointsReversed();
                }
            }
            mousePos = hit.point;
        }
    }
}
