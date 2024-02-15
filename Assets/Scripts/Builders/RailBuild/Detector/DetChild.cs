using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Trains
{
    public class DetChildEventArgs<T> : EventArgs
    {
        public bool IsEnter;
        public T CollidedWith;

        public DetChildEventArgs(bool isEnter, T collidedWith)
        {
            IsEnter = isEnter;
            CollidedWith = collidedWith;
        }
    }

    public class DetChild : MonoBehaviour
    {
        public static event EventHandler<DetChildEventArgs<RoadSegment>> OnRoadDetected;
        public static event EventHandler<DetChildEventArgs<Station>> OnStationDetected;
        public override string ToString() => $"DetChild {GetInstanceID()}";

        [Header("to set")]
        [SerializeField] private Material blue;
        [SerializeField] private Material red;

        [Header("to display")]
        [SerializeField] private RoadSegment curSegm;

        private MeshRenderer meshRend;
        public List<RoadSegment> DetectedRoads => detectedRoads;
        [SerializeField] private List<RoadSegment> detectedRoads;     //should not be serialized

        public List<Station> DetectedStations => detectedStations;
        [SerializeField] private List<Station> detectedStations;     //should not be serialized


        private void Awake()
        {
            detectedRoads = new List<RoadSegment>();
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
            DetectRoad(other);
            DetectStation(other);
        }

        private void OnTriggerExit(Collider other)
        {
            UndetectRoad(other);
            UndetectStation(other);
        }

        private void DetectRoad(Collider other)
        {
            if (other.TryGetComponent<RoadSegment>(out var rs)
                && rs != curSegm)
            {
                Assert.IsTrue(!detectedRoads.Contains(rs));
                detectedRoads.Add(rs);
                OnRoadDetected?.Invoke(this, new DetChildEventArgs<RoadSegment>(isEnter: true, collidedWith: rs));
            }
        }

        private void UndetectRoad(Collider other)
        {
            if (other.TryGetComponent<RoadSegment>(out var rs)
                && rs != curSegm)
            {
                Assert.IsTrue(detectedRoads.Contains(rs));
                detectedRoads.Remove(rs);
                OnRoadDetected?.Invoke(this, new DetChildEventArgs<RoadSegment>(isEnter: false, collidedWith: rs));
            }
        }


        private void DetectStation(Collider other)
        {
            if (other.TryGetComponent<Station>(out var st))
            {
                Assert.IsTrue(!detectedStations.Contains(st));
                detectedStations.Add(st);
                OnStationDetected?.Invoke(this, new DetChildEventArgs<Station>(isEnter: true, collidedWith: st));
            }
        }

        private void UndetectStation(Collider other)
        {
            if (other.TryGetComponent<Station>(out var st))
            {
                Assert.IsTrue(detectedStations.Contains(st));
                detectedStations.Remove(st);
                OnStationDetected?.Invoke(this, new DetChildEventArgs<Station>(isEnter: false, collidedWith: st));
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
