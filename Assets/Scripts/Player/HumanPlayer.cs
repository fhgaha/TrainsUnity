using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trains
{
    public enum PlayerState
    {
        None, BuildingRoads, BuildingStations
    }

    public class HumanPlayer : MonoBehaviour
    {
        [SerializeField] private RailBuilder rb;
        [SerializeField] private StationBuilder sb;
        [SerializeField] private Camera cam;

        private PlayerState state;

        private void Start()
        {
            EventManager.Instance.OnBuildRailPressed += OnBuildRailPressed;

            state = PlayerState.None;
        }

        private void OnBuildRailPressed(object sender, Toggle e)
        {
            if (e.isOn)
            {
                if (state == PlayerState.BuildingRoads) return;

                rb.gameObject.SetActive(true);
                state = PlayerState.BuildingRoads;
            }
            else
            {
                if (state == PlayerState.None) return;

                rb.gameObject.SetActive(false);
                state = PlayerState.None;
            }
        }

        private void Update()
        {
            if (state == PlayerState.BuildingRoads)
            {
                Vector3 from = Vector3.zero;
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f, LayerMask.GetMask("Ground")))
                {
                    //rb.BuildRoad(from, hit.point);
                }
            }
        }



    }
}
