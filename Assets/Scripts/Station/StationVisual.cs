using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Trains
{
    public class StationVisual : MonoBehaviour
    {
        [SerializeField] private Material allowedMaterial;
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material forbiddenMaterial;

        private Station parent;
        private MeshRenderer meshRend;
        private List<Station> stationsEntered = new();
        private List<RoadSegment> segmentsEntered = new();
        private List<BuildingToPushAway> buildingsEntered = new();

        public StationVisual Configure(Station station)
        {
            this.parent = station;
            meshRend = GetComponent<MeshRenderer>();
            return this;
        }

        public void BecomeGreen()
        {
            meshRend.material = allowedMaterial;
            parent.Segment.PaintGreen();
        }

        public void BecomeRed()
        {
            meshRend.material = forbiddenMaterial;
            parent.Segment.PaintRed();
        }

        public void BecomeDefaultColor()
        {
            meshRend.material = defaultMaterial;
            parent.Segment.PaintDefaultColor();
        }

        public void HandleStatoinEnter(Collider collider)
        {
            Station other = collider.GetComponent<Station>();
            if (other is null || other == parent) return;

            if (parent.IsBlueprint && !other.IsBlueprint)
            {
                stationsEntered.Add(other);
                BecomeRed();
            }
        }

        public void HandleStationExit(Collider collider)
        {
            Station other = collider.GetComponent<Station>();
            if (other is null || other == parent) return;

            if (parent.IsBlueprint && !other.IsBlueprint)
            {
                if (!stationsEntered.Contains(other))
                    throw new Exception($"Station {other} was not detected by OnTriggerEntered, but was somehow detected by OnTriggerExit");

                stationsEntered.Remove(other);

                if (stationsEntered.Count == 0)
                    BecomeGreen();
            }
        }

        public void HandleRoadEnter(Collider collider)
        {
            RoadSegment other = collider.GetComponent<RoadSegment>();
            if (other is null || other == parent.Segment) return;

            if (parent.IsBlueprint)
            {
                //Debug.Log($"{this}: HandleRoadEnter: {other}");
                segmentsEntered.Add(other);
                BecomeRed();
            }
        }

        public void HandleRoadExit(Collider collider)
        {
            RoadSegment other = collider.GetComponent<RoadSegment>();
            if (other is null || other == parent.Segment) return;

            if (parent.IsBlueprint)
            {
                if (!segmentsEntered.Contains(other))
                    throw new Exception($"Road segment {other} was not detected by OnTriggerEntered, but was somehow detected by OnTriggerExit");

                segmentsEntered.Remove(other);

                if (segmentsEntered.Count == 0)
                    BecomeGreen();
            }
        }

        public void HandleBuildingEnter(Collider collider)
        {
            if (!collider.TryGetComponent(out BuildingToPushAway other)) return;

            if (parent.IsBlueprint && !other.IsBlueprint)
            {
                buildingsEntered.Add(other);
                BecomeRed();
            }
        }

        public void HandleBuildingExit(Collider collider)
        {
            if (!collider.TryGetComponent(out BuildingToPushAway other)) return;

            if (parent.IsBlueprint && !other.IsBlueprint)
            {
                Assert.IsTrue(buildingsEntered.Contains(other), $"Building {other} was not detected by OnTriggerEntered, but was somehow detected by OnTriggerExit");
                buildingsEntered.Remove(other);
                if (buildingsEntered.Count == 0)
                    BecomeGreen();
            }
        }

        public void ResetColor()
        {
            stationsEntered.Clear();
            segmentsEntered.Clear();
            BecomeDefaultColor();
        }
    }
}
