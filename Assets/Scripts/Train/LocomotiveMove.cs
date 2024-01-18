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
        public bool KeepMoving = true;
        public string CurPathAsString;
        public bool LoopThroughPoints = true;
        public PathMovementStyle MovementStyle;

        public float SlowSpeed = 2;
        public float MaxSpeed = 50;
        public float curSpeed = 1000;
        public float rotSpeed = 10;

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
        private int assumedLocoLengthInIndices = 10;

        private void Start()
        {
            StartCoroutine(Move_Routine(3));
        }

        private void Update()
        {
            if (KeepMoving && !isCoroutineRunning)
            {
                StartCoroutine(Move_Routine(1));
            }
        }

        public IEnumerator Move_Routine(float seconds)
        {
            if (isCoroutineRunning == true) yield return null;

            isCoroutineRunning = true;
            curTargetIdx = 10;
            while (KeepMoving)
            {
                if (CurPath != PathForward && CurPath != PathBack) CurPath = PathForward;

                var distance = Vector3.Distance(supportFront.position, CurPath[curTargetIdx]);
                if (distance * distance < 0.1f)
                {
                    curTargetIdx++;
                }

                if (curTargetIdx >= CurPath.Count)
                {
                    //reached end
                    if (LoopThroughPoints)
                    {
                        curTargetIdx = 10;
                        CurPath = CurPath == PathForward ? PathBack : PathForward;
                        yield return new WaitForSeconds(seconds);

                        //flip loco instantly
                        supportFront.position = CurPath[curTargetIdx];
                        supportBack.position = CurPath[0];
                        transform.position = (supportFront.position + supportBack.position) / 2;
                        transform.Rotate(0, 180, 0, Space.Self);
                        yield return new WaitForSeconds(1);
                    }
                    else
                    {
                        curTargetIdx--;
                    }
                }

                //move supports
                supportFront.position = MovementStyle switch
                {
                    PathMovementStyle.Lerp => Vector3.Lerp(supportFront.position, CurPath[curTargetIdx], curSpeed * Time.deltaTime),
                    PathMovementStyle.Slerp => Vector3.Slerp(supportFront.position, CurPath[curTargetIdx], curSpeed * Time.deltaTime),
                    _ => Vector3.MoveTowards(supportFront.position, CurPath[curTargetIdx], curSpeed * Time.deltaTime),
                };

                if (curTargetIdx > assumedLocoLengthInIndices)
                    supportBack.position = MovementStyle switch
                    {
                        PathMovementStyle.Lerp => Vector3.Lerp(supportBack.position, CurPath[curTargetIdx - assumedLocoLengthInIndices - 1], curSpeed * Time.deltaTime),
                        PathMovementStyle.Slerp => Vector3.Slerp(supportBack.position, CurPath[curTargetIdx - assumedLocoLengthInIndices - 1], curSpeed * Time.deltaTime),
                        _ => Vector3.MoveTowards(supportBack.position, CurPath[curTargetIdx - assumedLocoLengthInIndices - 1], curSpeed * Time.deltaTime),
                    };

                //set pos and rot smoothly
                var dir = supportBack.position - supportFront.position;
                var midPt = (supportFront.position + supportBack.position) / 2;
                transform.SetPositionAndRotation(
                    position: Vector3.MoveTowards(transform.position, midPt, curSpeed * Time.deltaTime),
                    rotation: Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime)
                );

                yield return new WaitForFixedUpdate();
            }
            isCoroutineRunning = false;
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
        }
    }
}
