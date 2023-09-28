using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class StationBuilder : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private StationContainer stationContainer;
        //[SerializeField] private GameObject stationPrefab;  //prefab imported using this: https://github.com/atteneder/glTFast
        private Vector3 mousePos;
        private Station station;

        private void Awake()
        {
            station = GetComponentInChildren<Station>();
        }

        private void Start()
        {
            station.SetUpRoadSegment();
            gameObject.SetActive(false);
        }

        void Update()
        {
            //mouse movement
            HandleMouseMovement();

            //lmb pressed
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                stationContainer.Add(station);

                RouteManager.Instance.RegisterI(station.Entry1, station.Entry2, station.segment.GetApproxLength());
                
            }
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
