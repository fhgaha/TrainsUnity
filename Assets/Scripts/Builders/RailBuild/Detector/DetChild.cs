using AYellowpaper.SerializedCollections;
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
        public static event EventHandler<DetChildEventArgs> OnRoadDetected;
        public override string ToString() => $"DetChild {GetInstanceID()}";

        //public Dictionary<RoadSegment, int> DetectedTimes => detected;
        ////to display
        //[SerializeField] private Dictionary<RoadSegment, int> detected = new();
        //to display
        //[SerializeField] 
        private RoadSegment curSegm;

        [SerializeField] private Material blue;
        [SerializeField] private Material red;

        private MeshRenderer meshRend;

        public List<RoadSegment> Detected => detected;
        //to display
        //[SerializeField] 
        private List<RoadSegment> detected;

        private void Awake()
        {
            detected = new List<RoadSegment>();
            name = ToString();
            meshRend = GetComponent<MeshRenderer>();
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
                if (Detected.Contains(rs))
                    throw new Exception($"should not be");
                else
                    Detected.Add(rs);

                OnRoadDetected?.Invoke(this, new DetChildEventArgs(isEnter: true, collidedWith: rs));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<RoadSegment>(out var rs)
                && rs != curSegm)
            {
                if (!Detected.Contains(rs))
                    throw new Exception($"Should not happen!");

                Detected.Remove(rs);

                //Debug.Log($"{this} exit. Collided with {rs}, curSegm {curSegm.ToString()}");
                OnRoadDetected?.Invoke(this, new DetChildEventArgs(isEnter: false, collidedWith: rs));
            }
        }

        public void PaintBlue()
        {
            if (meshRend.material != blue)
                meshRend.material = blue;
        }

        public void PaintRed()
        {
            if (meshRend.material != red)
                meshRend.material = red;
        }
    }
}
