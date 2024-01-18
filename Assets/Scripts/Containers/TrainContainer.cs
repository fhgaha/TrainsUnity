using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class TrainContainer : MonoBehaviour
    {
        [SerializeField] private GameObject locoPrefab;
        [SerializeField] private StationContainer sc;
        [SerializeField] private RailContainer rc;

        private void Start()
        {

        }

        public void SendTrain(List<Station> stations, List<Vector3> pathForward, List<Vector3> pathBack)
        {
            if (pathForward.Count == 0 || pathBack.Count == 0) return;

            GameObject trainGameObj = Instantiate(locoPrefab, transform);
            Train train = trainGameObj.GetComponent<Train>();
            train.Data.Route = new Route(stations, pathForward, pathBack);
            //train.Data.Cargo = new Cargo();

            trainGameObj.transform.position = pathForward[0];
            LocomotiveMove locoMover = trainGameObj.GetComponent<LocomotiveMove>();
            //locoMover.Points = pathTo.Concat(pathBack).ToList();
            locoMover.PathForward = pathForward;
            locoMover.PathBack = pathBack;


            //locoMover.PathForward = pathForward.Select(v => new Vector3(v.x, v.y + 3, v.z)).ToList();
            //locoMover.PathBack = pathBack.Select(v => new Vector3(v.x, v.y + 3, v.z)).ToList(); 
        }

    }
}
