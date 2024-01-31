using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class Station : MonoBehaviour
    {
        [field: SerializeField] public bool IsBlueprint { get; set; } = true;
        [field: SerializeField] public Cargo Cargo { get; set; } = new();
        [field: SerializeField] public Cargo Demand { get; set; } = Cargo.Empty;

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

        [SerializeField] private List<Vector3> originalPoints;
        private StationVisual visual;

        private void Awake()
        {
            Segment = GetComponentInChildren<RoadSegment>();
            visual = GetComponentInChildren<StationVisual>().Configure(this);
            GetComponentInChildren<StationRotator>().Configure(this);
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
            Vector3 p1 = new() { x = 0, y = 0, z = 10 };
            Vector3 p2 = new() { x = 0, y = 0, z = -10 };
            MyMath.CalculateStraightLine(originalPoints, p1, p2, Global.Instance.DriveDistance);

            List<Vector3> segmentPts = new();
            segmentPts.AddRange(originalPoints);
            Segment.GenerateMeshSafely(segmentPts);
        }

        public void CopyInfoFrom(Station original)
        {
            this.Owner = original.Owner;
            this.Segment.CopyPoints(original.Segment);
            this.Segment.Start = original.Segment.Start;
            this.Segment.End = original.Segment.End;
        }

        public void UpdatePos(Vector3 newPos)
        {
            transform.position = newPos;
            UpdatePos();
        }

        public void UpdatePos()
        {
            for (int i = 0; i < Segment.Points.Count; i++)
            {
                Segment.Points[i] = transform.rotation * originalPoints[i] + Segment.transform.position;
            }
            Segment.Start = Segment.Points[0];
            Segment.End = Segment.Points[^1];
        }

        public void UpdateRotation(float yAngle) => transform.rotation = Quaternion.Euler(0, yAngle, 0);

        public void BecomeDefaultColor() => visual.BecomeDefaultColor();

        public void ResetVisual() => visual.ResetColor();

        //public void UnloadCargo(Train train)
        //{
        //    Cargo.Add(train.Data.Cargo);
        //    train.Data.Cargo.Erase();
        //}

        public void LoadCargoTo(Carriage car)
        {
            Dictionary<CargoType, int> maxAmnts = CarriageCargo.MaxAmnts;

            //is station to needs that cargo?
            //load not more than max amnt
        }

        public void UnloadCargoFrom(Carriage car)
        {
            Cargo.Add(car.Cargo);
            car.Cargo.Erase();
        }

        private void OnTriggerEnter(Collider other)
        {
            visual.HandleStatoinEnter(other);
            visual.HandleRoadEnter(other);
            HandleTrainEnter(other);
        }

        private void HandleTrainEnter(Collider collider)
        {
            if (collider.TryGetComponent(out LocomotiveMove locMove))
            {
                //Debug.Log($"{this}: {locMove + "haha"}"); 
            }
        }

        private void OnTriggerExit(Collider other)
        {
            visual.HandleStationExit(other);
            visual.HandleRoadExit(other);
        }


    }
}
