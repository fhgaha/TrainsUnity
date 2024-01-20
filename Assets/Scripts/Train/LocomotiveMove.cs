using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    //https://gist.github.com/codeimpossible/2704498b7b78240ccb08e5234b6a557c
    public class LocomotiveMove : MonoBehaviour
    {
        [field: SerializeField] public List<Vector3> PathForward { get; set; }
        [field: SerializeField] public List<Vector3> PathBack { get; set; }
        [field: SerializeField] public Transform Joint { get; set; }
        public bool KeepMoving = true;
        public string CurPathAsString;
        public bool LoopThroughPoints = true;
        public PathMovementStyle MovementStyle;

        public float SlowSpeed = 2;
        public float MaxSpeed = 10;
        public float Acceleration = 0.1f;
        public float CurSpeed = 0;
        public float RotSpeed = 10;

        public List<Vector3> CurPath
        {
            get => curPath;
            set
            {
                CurPathAsString = value == PathForward ? nameof(PathForward) : nameof(PathBack);
                curPath = value;
            }
        }

        [SerializeField] private Transform supportFront;
        [SerializeField] private Transform supportBack;

        private List<Vector3> curPath;
        private int curTargetIdx = 0;
        private bool isCoroutineRunning = false;

        public LocomotiveMove Configure(List<Vector3> pathForward, List<Vector3> pathBack, int startIdx)
        {
            PathForward = pathForward;
            PathBack = pathBack;
            transform.position = PathForward[startIdx];
            curTargetIdx = startIdx + 1;
            return this;
        }

        public IEnumerator Move_Routine(float unloadTime, float loadTime, int idx)
        {
            if (isCoroutineRunning == true) yield return null;
            isCoroutineRunning = true;

            var distBetweenSupports = Vector3.Distance(supportFront.position, supportBack.position);
            int locoLengthIndeces = (int)(distBetweenSupports / DubinsMath.driveDistance);
            CurPath = PathForward;

            new WaitForSeconds(loadTime);

            while (KeepMoving)
            {
                var distance = Vector3.Distance(transform.position, CurPath[curTargetIdx]);
                if (distance * distance < 0.1f)
                {
                    curTargetIdx++;
                }

                if (curTargetIdx >= CurPath.Count)
                {
                    //reached end
                    if (LoopThroughPoints)
                    {
                        curTargetIdx = locoLengthIndeces;
                        CurPath = CurPath == PathForward ? PathBack : PathForward;
                        yield return new WaitForSeconds(unloadTime);

                        //flip loco instantly
                        transform.position = (CurPath[curTargetIdx] + CurPath[0]) / 2;
                        transform.Rotate(0, 180, 0, Space.Self);
                        yield return new WaitForSeconds(loadTime);
                    }
                    else
                    {
                        //dont move
                        curTargetIdx--;
                    }
                }
                else
                {
                    Vector3 frontPt = curPath[curTargetIdx];
                    Vector3 backPt = curPath[curTargetIdx - locoLengthIndeces];
                    Vector3 dir = (frontPt - backPt).normalized;

                    var dot = Vector3.Dot(transform.forward, (frontPt - transform.position).normalized);
                    float t = Mathf.InverseLerp(0.96f, 1f, dot);
                    float reqSpeed = Mathf.Lerp(SlowSpeed, MaxSpeed, t);
                    if (ReachingDestination()) reqSpeed = SlowSpeed;
                    CurSpeed += (CurSpeed > reqSpeed ? -1 : 1) * Acceleration;

                    transform.SetPositionAndRotation(
                        position: Vector3.MoveTowards(transform.position, curPath[curTargetIdx], CurSpeed * Time.deltaTime),
                        rotation: Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), CurSpeed * Time.deltaTime)
                    );

                    yield return new WaitForEndOfFrame();
                }
            }
            isCoroutineRunning = false;
        }

        public bool ReachingDestination()
        {
            float threshold = 30;
            float dist = (curPath.Count - curTargetIdx) * DubinsMath.driveDistance;
            return dist < threshold;
        }

        private void OnDrawGizmosSelected()
        {
            ShowGizmos();
        }

        private void ShowGizmos()
        {
            if (CurPath == null || CurPath.Count == 0) return;

            for (int i = 0; i < CurPath.Count; i++)
            {
                Gizmos.color = Color.yellow;
                if (i < curTargetIdx) Gizmos.color = Color.red;
                if (i > curTargetIdx) Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(CurPath[i], 1f);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, CurPath[curTargetIdx]);

            Gizmos.DrawLine(supportFront.position, supportFront.position + Vector3.up * 10);
            Gizmos.DrawLine(supportBack.position, supportBack.position + Vector3.up * 10);
        }
    }
}
