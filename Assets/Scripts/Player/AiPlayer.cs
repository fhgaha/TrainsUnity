using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public interface IPlayer { }

    public class AiPlayer : MonoBehaviour, IPlayer
    {
        [SerializeField] private RailBuilder rb;
        [SerializeField] private StationBuilder sb;
        [SerializeField] private Camera cam;

        private PlayerState state;

        private void Start()
        {
            rb.Parent = this;
            rb.gameObject.SetActive(true);

            BuildRoad();
        }

        [ContextMenu("Build Road")] //works even on disabled gameonject
        public void BuildRoad()
        {
            Vector3 p1 = Vector3.zero;
            Vector3 p2 = new(20, 0, 20);
            Vector3 p3 = new(20, 0, -20);

            StartCoroutine(Build());
            IEnumerator Build()
            {
                yield return BuildRoad(p1, p2);
                yield return BuildRoad(p2, p3);
            }
        }

        private Coroutine BuildRoad(Vector3 p1, Vector3 p2) => StartCoroutine(rb.BuildRoad_Routine(p1, p2));

        private void Update()
        {
            //if (state == PlayerState.BuildingRoads)
            //{
            //    Vector3 from = Vector3.zero;
            //    if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000f, LayerMask.GetMask("Ground")))
            //    {
            //        rb.BuildRoad(from, hit.point);
            //    }
            //}

        }
    }
}
