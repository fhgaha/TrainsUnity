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

        [SerializeField] private Transform visual;

        private List<Vector3> curPath;
        private int curTargetIdx = 0;
        private bool isCoroutineRunning = false;

        private void Awake()
        {
            visual = transform.GetChild(0);
        }

        private void Start()
        {
            StartCoroutine(Move_Routine(3));
        }

        private void Update()
        {
            if (KeepMoving && !isCoroutineRunning)
            {
                StartCoroutine(Move_Routine(3));
            }
        }

        public IEnumerator Move_Routine(float seconds)
        {
            if (isCoroutineRunning == true) yield return null;

            isCoroutineRunning = true;
            while (KeepMoving)
            {
                if (CurPath != PathForward && CurPath != PathBack) CurPath = PathForward;

                Vector3 nextPt;
                if (CurPath == PathForward) nextPt = PathForward[curTargetIdx];
                else if (CurPath == PathBack) nextPt = PathBack[curTargetIdx];
                else throw new Exception();

                var distance = Vector3.Distance(transform.position, nextPt);
                if (distance * distance < 0.1f)
                {
                    curTargetIdx++;
                }

                if (curTargetIdx >= CurPath.Count)
                {
                    if (LoopThroughPoints)
                    {
                        curTargetIdx = 0;
                        CurPath = CurPath == PathForward ? PathBack : PathForward;
                        yield return new WaitForSeconds(seconds);
                    }
                    else
                    {
                        curTargetIdx--;
                    }
                }

                var dir = CurPath[curTargetIdx] - transform.position;
                if (dir != Vector3.zero)
                    transform.rotation = Quaternion.Lerp(visual.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);

                transform.position = MovementStyle switch
                {
                    PathMovementStyle.Lerp => Vector3.Lerp(transform.position, CurPath[curTargetIdx], curSpeed * Time.deltaTime),
                    PathMovementStyle.Slerp => Vector3.Slerp(transform.position, CurPath[curTargetIdx], curSpeed * Time.deltaTime),
                    _ => Vector3.MoveTowards(transform.position, CurPath[curTargetIdx], curSpeed * Time.deltaTime),
                };

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
