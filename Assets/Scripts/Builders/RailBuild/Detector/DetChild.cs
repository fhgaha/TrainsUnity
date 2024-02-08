using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class DetChildEventArgs : EventArgs
    {
        public bool IsEnter;
        public RoadSegment CollidedWith;

        public DetChildEventArgs(bool isEnter, RoadSegment collidedWith)
        {
            IsEnter = isEnter;
            CollidedWith = collidedWith;
        }
    }

    public class DetChild : MonoBehaviour
    {
        //to display
        [SerializeField] private RoadSegment curSegm;

        public static event EventHandler<DetChildEventArgs> OnRoadDetected;
        public override string ToString() => $"DetChild {GetInstanceID()}";

        private void Awake()
        {
            name = ToString();
        }

        public DetChild Configure(RoadSegment rs)
        {
            curSegm = rs;
            name = ToString();
            return this;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<RoadSegment>(out var rs)
                && rs != curSegm)
            {
                //Debug.Log($"{this} enter. Collided with {rs}, curSegm {curSegm.ToString()}");
                OnRoadDetected?.Invoke(this, new DetChildEventArgs(isEnter: true, collidedWith: rs));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<RoadSegment>(out var rs)
                && rs != curSegm)
            {
                //Debug.Log($"{this} exit. Collided with {rs}, curSegm {curSegm.ToString()}");
                OnRoadDetected?.Invoke(this, new DetChildEventArgs(isEnter: false, collidedWith: rs));
            }
        }
    }
}
