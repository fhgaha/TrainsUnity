using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class LocomotiveMove : MonoBehaviour
    {
        [field: SerializeField] public List<Vector3> Points { get; set; }
        public bool LoopThroughPoints = true;
        public PathMovementStyle MovementStyle;

        public float SlowSpeed = 2;
        public float MaxSpeed = 50;
        public float curSpeed = 2;

        private int curTargetIdx = 0;

        private void Update()
        {
            if (Points == null || Points.Count == 0) return;

            var distance = Vector3.Distance(transform.position, Points[curTargetIdx]);
            if (distance * distance < 0.1f)
            {
                curTargetIdx++;
                if (curTargetIdx >= Points.Count) curTargetIdx = Points.Count - 1;
                if (LoopThroughPoints) curTargetIdx = 0;
            }
            
            transform.position = MovementStyle switch
            {
                PathMovementStyle.Lerp  => Vector3.Lerp         (transform.position, Points[curTargetIdx], curSpeed * Time.deltaTime),
                PathMovementStyle.Slerp => Vector3.Slerp        (transform.position, Points[curTargetIdx], curSpeed * Time.deltaTime),
                                    _   => Vector3.MoveTowards  (transform.position, Points[curTargetIdx], curSpeed * Time.deltaTime),
            };


            //also increase and decrease speed automatically
        }
        
        private void OnDrawGizmosSelected()
        {
            //ShowGizmos();
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
