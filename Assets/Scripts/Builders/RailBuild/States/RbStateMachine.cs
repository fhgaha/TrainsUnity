using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    [Serializable]
    public class RbStateMachine
    {
        public RbSelectStartState SelectStartState { get; }
        public RbInitialSegmentState InitialSegmentState { get; }
        public RbNoninitialSegmentState NoninitialSegmentState { get; }

        public RbBaseState CurrentState
        {
            get => currentState; 
            private set
            {
                currentState = value;
                curStateName = value.GetType().Name;
            }
        }
        public string curStateName = "---";
        private RbBaseState currentState = null;

        public RbStateMachine(RailBuilder rb, RegisterHelper regHelp)
        {
            SelectStartState = new(rb);
            InitialSegmentState = new(rb, regHelp, this);
            NoninitialSegmentState = new(rb, regHelp, this);

            CurrentState = SelectStartState;
            CurrentState.OnEnter(this);
        }

        public void UpdateState(bool wasHit, Vector3 hitPoint, bool lmbPressed, bool rmbPressed)
            => CurrentState.UpdateState(this, wasHit, hitPoint, lmbPressed, rmbPressed);

        public void SwitchStateTo(RbBaseState state)
        {
            CurrentState.OnExit(this);
            CurrentState = state;
            CurrentState.OnEnter(this);
        }

        public HeadedPoint GetSnappedEnd(List<Vector3> pts, Vector3 mousePos, Vector3 dir)
        {
            Vector3 closest = GetClosestPoint(pts, mousePos);
            float heading;
            int index = pts.IndexOf(closest);

            if (index == 0)
            {
                heading = Vector3.SignedAngle(Vector3.forward, pts[1] - closest, Vector3.up);
            }
            else if (index == pts.Count - 1)
            {
                heading = Vector3.SignedAngle(Vector3.forward, pts[^2] - closest, Vector3.up);
            }
            else
            {
                Vector3 dir1 = pts[index - 1] - closest;
                Vector3 dir2 = pts[index + 1] - closest;
                float a1 = Vector3.Angle(dir, dir1);
                float a2 = Vector3.Angle(dir, dir2);
                Vector3 snappedDir = a1 * a1 <= a2 * a2 ? dir1 : dir2;
                heading = Vector3.SignedAngle(Vector3.forward, snappedDir, Vector3.up);
            }
            return new HeadedPoint(closest, heading);
        }

        public float GetSnappedStartHeading(List<Vector3> pts, Vector3 snapped, Vector3 dir)
        {
            float heading;
            int index = pts.IndexOf(snapped);

            //since detector has some width we need to check not only first or last index but also range of indexes, which will depend on drive distance unfotunatelly
            bool IsStartEdge = index == 0;
            bool IsEndEdge = index == pts.Count - 1;
            if (pts.Count >= 5)
            {
                IsStartEdge = Enumerable.Range(0, 5).Contains(index);
                IsEndEdge = Enumerable.Range(pts.Count - 4, pts.Count).Contains(index);
            }

            if (IsStartEdge)
            {
                heading = Vector3.SignedAngle(Vector3.forward, pts[0] - pts[1], Vector3.up);
            }
            else if (IsEndEdge)
            {
                heading = Vector3.SignedAngle(Vector3.forward, pts[^1] - pts[^2], Vector3.up);
            }
            else
            {
                Vector3 dir1 = pts[index - 1] - snapped;
                Vector3 dir2 = pts[index + 1] - snapped;
                float a1 = Vector3.Angle(dir, dir1);
                float a2 = Vector3.Angle(dir, dir2);
                Vector3 snappedDir = a1 * a1 <= a2 * a2 ? dir1 : dir2;

                heading = Vector3.SignedAngle(Vector3.forward, snappedDir, Vector3.up);
            }
            return heading;
        }

        public Vector3 GetClosestPoint(List<Vector3> pts, Vector3 mousePos)
            => pts.Aggregate((clst, next) => (next - mousePos).magnitude < (clst - mousePos).magnitude ? next : clst);
    }
}
