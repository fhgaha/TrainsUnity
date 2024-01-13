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

        public void SendTrain(List<Station> stations, List<Vector3> pathTo, List<Vector3> pathBack)
        {
            if (pathTo.Count == 0 || pathBack.Count == 0) return;

            GameObject trainGameObj = Instantiate(locoPrefab, transform);
            Train train = trainGameObj.GetComponent<Train>();
            train.Data.Route = new Route(stations, pathTo, pathBack);
            //train.Data.Cargo = new Cargo();

            trainGameObj.transform.position = pathTo[0];
            LocomotiveMove locoMover = trainGameObj.GetComponent<LocomotiveMove>();
            //locoMover.Points = pathTo.Concat(pathBack).ToList();
            locoMover.PathForward = pathTo;
            locoMover.PathBack = pathBack;
        }

    }
}
