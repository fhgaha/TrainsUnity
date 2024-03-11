using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class Station : MonoBehaviour
    {
        [field: SerializeField] public bool IsBlueprint { get; set; } = true;

        public RoadSegment Segment;
        public Vector3 Entry1 => Segment.Start;
        public Vector3 Entry2 => Segment.End;

        public IPlayer Owner
        {
            get => owner;
            private set
            {
                OwnerName = value == null ? "---" : ((MonoBehaviour)value).name;
                owner = value;
            }
        }
        public string OwnerName = "---";
        private IPlayer owner;
        public override string ToString() => $"Station \"{name}\", id: {GetInstanceID()}, entry1: {Entry1}, entry2: {Entry2}";
        
        public StationCargoHandler CargoHandler => cargoHandler;
        private StationCargoHandler cargoHandler;

        [SerializeField] private List<Vector3> originalPoints;
        private StationVisual visual;
        private StationMovement mover;
        
        public ProfitBuildingDetector ProfitBuildingDetector => profitBuildingDetector;
        private ProfitBuildingDetector profitBuildingDetector;

        public StationCollider StCollider => stCollider;
        private StationCollider stCollider;

        private void Awake()
        {
            mover = GetComponent<StationMovement>();
            cargoHandler = GetComponent<StationCargoHandler>().Configure(this);
            visual = GetComponentInChildren<StationVisual>().Configure(this);
            profitBuildingDetector = GetComponentInChildren<ProfitBuildingDetector>().Configure(this);
            stCollider = transform.GetComponentInChildren<StationCollider>().Configure(this);
            Segment = GetComponentInChildren<RoadSegment>();
            Segment.name = $"Station's {Segment}";
            Segment.Owner = owner;
            mover.Configure(this);
            GetComponentInChildren<MeshCollider>().sharedMesh = Segment.GetMesh();
        }

        private void Start()
        {
            if (IsBlueprint) visual.BecomeGreen();
        }

        public void SetUpRoadSegment(IPlayer owner)
        {
            Owner = owner;

            originalPoints = new();
            Vector3 p1 = new() { x = 0, y = 0, z = 20 };
            Vector3 p2 = new() { x = 0, y = 0, z = -20 };
            MyMath.FillWithStraightLine(originalPoints, p1, p2, Global.Instance.DriveDistance);

            List<Vector3> segmentPts = new();
            segmentPts.AddRange(originalPoints);
            Segment.Owner = owner;
            Segment.UpdateMeshAndCollider(segmentPts);
        }

        public void TurnOnProfitBuildingDetector()
        {
            //ProfitBuildingDetector.gameObject.GetComponent<MeshCollider>().enabled = true;

            if(ProfitBuildingDetector.gameObject.TryGetComponent(out MeshCollider mc))
            {
                mc.enabled = true;
            }
        }

        public void TurnOffProfitBuildingDetector()
        {
            //ProfitBuildingDetector.gameObject.GetComponent<MeshCollider>().enabled = false;

            if (ProfitBuildingDetector.gameObject.TryGetComponent(out MeshCollider mc))
            {
                mc.enabled = false;
            }
        }

        public void CopyInfoFrom(Station original)
        {
            Owner = original.Owner;
            Segment.Owner = original.Owner;
            Segment.CopyPointsByValue(original.Segment);
            Segment.Start = original.Segment.Start;
            Segment.End = original.Segment.End;
        }

        public void UpdatePos(Vector3 newPos) => mover.UpdatePos(newPos, Segment, originalPoints);
        public void UpdateSegmPoints() => Segment.UpdatePoints(transform.rotation, originalPoints);
        public void UpdateRotation(float yAngle) => mover.UpdateRotation(yAngle);
        public void BecomeDefaultColor() => visual.BecomeDefaultColor();
        public void ResetVisual() => visual.ResetColor();
        public void UnloadCargoFrom(Carriage car) => cargoHandler.UnloadCargoFrom(car);

        public void OnColliderTriggerEnter(Collider other)
        {
            visual.HandleStatoinEnter(other);
            visual.HandleRoadEnter(other);
            //visual.HandleBuildingEnter(other);
        }

        public void OnColliderTriggerExit(Collider other)
        {
            visual.HandleStationExit(other);
            visual.HandleRoadExit(other);
            //visual.HandleBuildingExit(other);
        }

    }
}
