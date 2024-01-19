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
        public float curSpeed = 10;
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

        public IEnumerator Move_Routine(float loadTime)
        {
            if (isCoroutineRunning == true) yield return null;
            isCoroutineRunning = true;

            var distBetweenSupports = Vector3.Distance(supportFront.position, supportBack.position);
            int locoLengthIndeces = (int)(distBetweenSupports / DubinsMath.driveDistance);

            curTargetIdx = locoLengthIndeces + 1;
            transform.SetPositionAndRotation(
                (PathForward[locoLengthIndeces] + PathForward[0]) / 2,
                Quaternion.LookRotation((PathForward[locoLengthIndeces] - PathForward[0]).normalized)
            );

            while (KeepMoving)
            {
                if (CurPath != PathForward && CurPath != PathBack) CurPath = PathForward;

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
                        yield return new WaitForSeconds(loadTime);

                        //flip loco instantly
                        transform.position = (CurPath[curTargetIdx] + CurPath[0]) / 2;
                        transform.Rotate(0, 180, 0, Space.Self);
                        yield return new WaitForSeconds(1);
                    }
                    else
                    {
                        //dont move
                        curTargetIdx--;
                    }
                }
                else
                {
                    //suportFront is at the same pos as main transform
                    Vector3 nextPt = curPath[curTargetIdx];
                    Vector3 nextBackPt = curPath[curTargetIdx - locoLengthIndeces];
                    Vector3 dir = (nextPt - nextBackPt).normalized;
                    transform.SetPositionAndRotation(
                        position: Vector3.MoveTowards(transform.position, curPath[curTargetIdx], curSpeed * Time.deltaTime),
                        //rotation: Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime)
                        rotation: Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), curSpeed * Time.deltaTime)
                    );

                    yield return new WaitForFixedUpdate();
                }
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

            Gizmos.DrawLine(supportFront.position, supportFront.position + Vector3.up * 10);
            Gizmos.DrawLine(supportBack.position, supportBack.position + Vector3.up * 10);
        }
    }
}
