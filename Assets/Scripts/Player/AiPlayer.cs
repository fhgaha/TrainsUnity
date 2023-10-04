    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class AiPlayer : MonoBehaviour
    {
        [SerializeField] private RailBuilder rb;
        [SerializeField] private StationBuilder sb;
        [SerializeField] private Camera cam;

        private PlayerState state;

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
