using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class LocomotiveMove : MonoBehaviour, ILocoCarriageMove
    {
        [field: SerializeField] public Transform Front { get; private set; }
        [field: SerializeField] public Transform Back { get; private set; }
        [field: SerializeField] public Transform SupportFront { get; private set; }
        [field: SerializeField] public Transform SupportBack { get; private set; }
        public int LengthIndeces => (int)(Vector3.Distance(Front.position, Back.position) / DubinsMath.driveDistance);
        public int SupportLengthIndeces => (int)(Vector3.Distance(SupportFront.position, SupportBack.position) / DubinsMath.driveDistance);

        public LocomotiveMove Configure(Vector3 startPos, Quaternion rot)
        {
            transform.SetPositionAndRotation(startPos, rot);
            return this;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(SupportFront.position, SupportFront.position + Vector3.up * 5);
            Gizmos.DrawLine(SupportBack.position, SupportBack.position + Vector3.up * 5);
        }
    }
}
