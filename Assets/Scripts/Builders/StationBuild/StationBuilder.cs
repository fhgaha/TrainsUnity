using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class StationBuilder : MonoBehaviour
    {
        public IPlayer Owner
        {
            get { return owner; }
            private set
            {
                ownerName = $"{value.GetType()}, id: {value.Id}";
                owner = value;
            }
        }
        [SerializeField] private string ownerName = "---";
        private IPlayer owner;

        [SerializeField] private Camera cam;
        [SerializeField] private StationContainer stationContainer;
        //[SerializeField] private GameObject stationPrefab;  //prefab imported using this: https://github.com/atteneder/glTFast

        private Vector3 mousePos;
        private Station station;

        public void Configure(IPlayer owner)
        {
            Owner = owner;
            station.SetUpRoadSegment(Owner);
            station.StCollider.gameObject.AddComponent<Rigidbody>().useGravity = false;
            station.ProfitBuildingDetector.gameObject.AddComponent<Rigidbody>().useGravity = false;
            station.StCollider.gameObject.GetComponent<BoxCollider>().isTrigger = true; 
        }

        public bool AssertConfigured()
        {
            if (Owner == null) throw new System.Exception($"Parent should be configured. Parent is {Owner}.");
            return true;
        }

        public Station PlaceStation(Vector3 pos, float angle)
        {
            AssertConfigured();

            station.UpdateRotation(angle);
            station.UpdatePos(pos);
            Station instance = PlaceStation();
            return instance;
        }

        private void Awake()
        {
            station = GetComponentInChildren<Station>();
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            station.ResetVisual();
            station.transform.position = Vector3.zero;
        }

        void Update()
        {
            //mouse movement
            HandleMouseMovement();

            //lmb pressed
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                PlaceStation();
            }

            //rmb pressed is handled by ui
        }

        private Station PlaceStation()
        {
            Station inst = stationContainer.Add(station);
            RouteManager.Instance.RegisterI(inst.Entry1, inst.Entry2, inst.Segment.GetApproxLength(), Owner);
            inst.StCollider.Collider.isTrigger = false;
            inst.IsBlueprint = false;
            inst.TurnOffProfitBuildingDetector();
            Destroy(inst.StCollider.Collider.GetComponent<Rigidbody>());
            Destroy(inst.ProfitBuildingDetector.GetComponent<Rigidbody>());

            inst.ProfitBuildingDetector.Detected.AddRange(station.ProfitBuildingDetector.Detected);
            station.ProfitBuildingDetector.Detected.Clear();

            inst.Segment.PlaceFromStationBuilder();

            return inst;
        }

        private void HandleMouseMovement()
        {
            if (!HitGround(cam, out RaycastHit hit)) return;
            if (mousePos == hit.point) return;

            station.UpdatePos(mousePos);
            mousePos = hit.point;
        }

        private bool HitGround(Camera camera, out RaycastHit hit) =>
            Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 1000f, LayerMask.GetMask("Ground"));
    }
}
