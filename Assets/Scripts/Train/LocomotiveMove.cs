using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    //https://gist.github.com/codeimpossible/2704498b7b78240ccb08e5234b6a557c
    public class LocomotiveMove : MonoBehaviour
    {
        [SerializeField] private Transform visual;
        [field: SerializeField] public List<Vector3> Points { get; set; }
        public bool LoopThroughPoints = true;
        public PathMovementStyle MovementStyle;

        public float SlowSpeed = 2;
        public float MaxSpeed = 50;
        public float curSpeed = 40;
        public float rotSpeed = 10;


        private int curTargetIdx = 0;
        private bool goingReverse = false;

        private void Awake()
        {
            visual = transform.GetChild(0);
        }

        private void Update()
        {
            if (Points == null || Points.Count == 0) return;

            var distance = Vector3.Distance(transform.position, Points[curTargetIdx]);
            if (distance * distance < 0.1f)
            {
                curTargetIdx++;
                if (curTargetIdx >= Points.Count)
                {
                    curTargetIdx = 0;
                }
            }

            //UpdateCurTargetIdx_GoBackToStart();

            //visual.rotation = Quaternion.LookRotation(Points[curTargetIdx] - transform.position);
            var dir = Points[curTargetIdx] - transform.position;
            if (dir != Vector3.zero)
                visual.rotation = Quaternion.Lerp(visual.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);

            transform.position = MovementStyle switch
            {
                PathMovementStyle.Lerp => Vector3.Lerp(transform.position, Points[curTargetIdx], curSpeed * Time.deltaTime),
                PathMovementStyle.Slerp => Vector3.Slerp(transform.position, Points[curTargetIdx], curSpeed * Time.deltaTime),
                _ => Vector3.MoveTowards(transform.position, Points[curTargetIdx], curSpeed * Time.deltaTime),
            };


            //also increase and decrease speed automatically
        }

        private void UpdateCurTargetIdx_GoBackToStart()
        {
            var distance = Vector3.Distance(transform.position, Points[curTargetIdx]);
            if (distance * distance < 0.1f)
            {
                curTargetIdx++;
                if (curTargetIdx >= Points.Count)
                {
                    curTargetIdx = LoopThroughPoints ? 0 : Points.Count - 1;
                }
            }
        }

        private void UpdateCurTargetIdx_ReversePoints()
        {
            var distance = Vector3.Distance(transform.position, Points[curTargetIdx]);
            if (distance * distance < 0.1f)
            {
                curTargetIdx++;
                if (curTargetIdx >= Points.Count)
                {
                    curTargetIdx = LoopThroughPoints ? 0 : Points.Count - 1;
                    Points.Reverse();
                }
            }
        }

        private void UpdateCurTargetIdx_IncreaseOrDecreaseIdx()
        {
            var distance = Vector3.Distance(transform.position, Points[curTargetIdx]);
            if (distance * distance < 0.1f)
            {
                curTargetIdx += goingReverse ? -1 : 1;
                bool reachedEnd = curTargetIdx >= Points.Count;
                bool reachedStart = curTargetIdx <= 0;
                if (reachedEnd)
                {
                    curTargetIdx = Points.Count - 1;
                    if (LoopThroughPoints)
                        goingReverse = true;
                }
                if (reachedStart)
                {
                    curTargetIdx = 0;
                    goingReverse = false;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            ShowGizmos();
        }

        private void ShowGizmos()
        {
            if (Points == null || Points.Count == 0) return;

            for (int i = 0; i < Points.Count; i++)
            {
                Gizmos.color = Color.yellow;
                if (i < curTargetIdx) Gizmos.color = Color.red;
                if (i > curTargetIdx) Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(Points[i], 1f);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, Points[curTargetIdx]);
        }
    }
}
