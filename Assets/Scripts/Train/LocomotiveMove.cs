using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class LocomotiveMove : MonoBehaviour
    {
        [field: SerializeField] public Transform SupportFront { get; private set; }
        [field: SerializeField] public Transform SupportBack { get; private set; }
        [field: SerializeField] public Transform Joint { get; set; }
        public int LengthIndeces
        {
            get
            {
                var distBetweenSupports = Vector3.Distance(SupportFront.position, SupportBack.position);
                return (int)(distBetweenSupports / DubinsMath.driveDistance);
            }
        }

        public LocomotiveMove Configure(Vector3 startPos)
        {
            transform.position = startPos;
            return this;
        }

        private void OnDrawGizmosSelected()
        {
            ShowGizmos();
        }

        private void ShowGizmos()
        {
            //if (CurPath == null || CurPath.Count == 0) return;

            //for (int i = 0; i < CurPath.Count; i++)
            //{
            //    Gizmos.color = Color.yellow;
            //    if (i < curTargetIdx) Gizmos.color = Color.red;
            //    if (i > curTargetIdx) Gizmos.color = Color.green;
            //    Gizmos.DrawWireSphere(CurPath[i], 1f);
            //}

            //Gizmos.color = Color.yellow;
            //Gizmos.DrawLine(transform.position, CurPath[curTargetIdx]);

            //Gizmos.DrawLine(supportFront.position, supportFront.position + Vector3.up * 10);
            //Gizmos.DrawLine(supportBack.position, supportBack.position + Vector3.up * 10);
        }
    }
}
