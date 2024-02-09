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
        public SerializedDictionary<RoadSegment, int> DetectedTimes => detected;
        //to display
        [SerializeField] private SerializedDictionary<RoadSegment, int> detected = new();

        [SerializeField] private Material blue;
        [SerializeField] private Material red;

        //to display
        [SerializeField] private RoadSegment curSegm;

        private MeshRenderer meshRend;

        private void Awake()
        {
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
                //Debug.Log($"{this} enter. Collided with {rs}, curSegm {curSegm.ToString()}");
                if (detected.ContainsKey(rs))
                    detected[rs]++;
                else
                    detected.Add(rs, 1);

                OnRoadDetected?.Invoke(this, new DetChildEventArgs(isEnter: true, collidedWith: rs));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<RoadSegment>(out var rs)
                && rs != curSegm)
            {
                if (!detected.ContainsKey(rs))
                    throw new Exception($"Should not happen!");

                detected[rs]--;
                if (detected[rs] == 0)
                    detected.Remove(rs);

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
